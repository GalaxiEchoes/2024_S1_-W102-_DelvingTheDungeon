using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;
using System;

public class DungeonGenerator : MonoBehaviour
{
    [Serializable]
    public enum CellType
    {
        None,
        Room,
        Hallway,
        Stairs,
        RoomInPath,
        StairEnd,
        StartingRoom,
        EndingRoom
    }

    [Serializable]
    public enum Direction
    {
        North,
        East,
        South,
        West,
        None
    }

    [Serializable]
    public class Room
    {
        public BoundsInt bounds;

        public Room(Vector3Int location, Vector3Int size)
        {
            bounds = new BoundsInt(location, size);
        }

        public static bool Intersect(Room a, Room b)
        {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
                || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y)
                || (a.bounds.position.z >= (b.bounds.position.z + b.bounds.size.z)) || ((a.bounds.position.z + a.bounds.size.z) <= b.bounds.position.z));
        }
    }

    [Serializable]
    public class Stairs
    {
        public Vector3Int Location;
        public Direction StairDirection;

        public Stairs(Vector3Int location, Direction direction)
        {
            this.Location = location;
            this.StairDirection = direction;
        }
    }

    [Header("Spawn Settings")]
    [SerializeField] public Vector3Int size;
    [SerializeField] int roomCount;
    [SerializeField] Vector3Int roomMaxSize;
    [SerializeField] public int bossLevel;

    //Generated Products
    public Grid3D<CellType> grid;
    public List<Room> rooms;
    public List<Stairs> stairs;
    public Vector3Int endRoomPos;
    public Vector3Int startRoomPos;

    //Generation Variables
    Random random;
    Delaunay3D delaunay;
    HashSet<Prim.Edge> selectedEdges;
    public int currentLevel;
    public int seed;

    public void GenerateDungeon()
    {
        random = new Random(seed);
        int maxRegens = 5;
        
        do
        {
            maxRegens--;

            grid = new Grid3D<CellType>(size, Vector3Int.zero);
            rooms = new List<Room>();
            stairs = new List<Stairs>();

            PlaceRooms();
            Triangulate();
            CreateHallways();
            PathfindHallways();
        }while (!ConnectionCheck() && maxRegens > 0) ;

        //Reassigning the start/end enums
        grid[startRoomPos] = CellType.StartingRoom;
        if(currentLevel == bossLevel)
        {
            grid[endRoomPos] = CellType.EndingRoom;
        }
        else
        {
            foreach (var pos in rooms[1].bounds.allPositionsWithin)
            {
                grid[pos] = CellType.EndingRoom;
            }
        }
    }

    bool ConnectionCheck()
    {
        bool startConnected = false;
        bool endConnected = false;

        if(startRoomPos.x > 0 && startRoomPos.x < size.x)
        {
            if (grid[startRoomPos + Vector3Int.left] == CellType.Hallway || grid[startRoomPos + Vector3Int.forward] == CellType.Hallway || grid[startRoomPos + Vector3Int.right] == CellType.Hallway)
            {
                startConnected = true;
            }
        }

        if(currentLevel == bossLevel)
        {
            if (endRoomPos.x > 0 && endRoomPos.x < size.x)
            {
                if (grid[endRoomPos + Vector3Int.left] == CellType.Hallway || grid[endRoomPos + Vector3Int.back] == CellType.Hallway || grid[endRoomPos + Vector3Int.right] == CellType.Hallway)
                {
                    endConnected = true;
                }
            }
        }
        else if((endRoomPos.x > 0 && endRoomPos.x < size.x) && (endRoomPos.z > 0 && endRoomPos.z < size.z))
        {
            Vector3Int position = endRoomPos + Vector3Int.up;

            if(grid[position + Vector3Int.left] == CellType.Hallway || grid[position + Vector3Int.back] == CellType.Hallway ) endConnected = true;

            position += Vector3Int.forward;

            if (grid[position + Vector3Int.left] == CellType.Hallway || grid[position + Vector3Int.forward] == CellType.Hallway) endConnected = true;

            position += Vector3Int.right;

            if (grid[position + Vector3Int.forward] == CellType.Hallway || grid[position + Vector3Int.right] == CellType.Hallway) endConnected = true;

            position += Vector3Int.back;

            if (grid[position + Vector3Int.right] == CellType.Hallway || grid[position + Vector3Int.back] == CellType.Hallway) endConnected = true;
        }
        return startConnected && endConnected;
    }

    void PlaceRooms()
    {
        //Add Starting Room
        Vector3Int startPos = new Vector3Int(random.Next(1, size.x - 1), 1, 0);
        Room startingRoom = new Room(startPos, Vector3Int.one);
        rooms.Add(startingRoom);
        startRoomPos = startPos;
        grid[startPos] = CellType.Room;

        if (currentLevel == bossLevel)
        {
            //Add Boss room
            Vector3Int bossPos = new Vector3Int(random.Next(1, size.x-1), 1, size.z - 1);
            Room bossRoom = new Room(bossPos, Vector3Int.one);
            rooms.Add(bossRoom);
            endRoomPos = bossPos;
            grid[bossPos] = CellType.Room;
        }
        else
        {
            //Add Ending Room
            Room endingRoom = null;
            bool add = false;

            while (!add)
            {

                Vector3Int endLocation = new Vector3Int(
                    random.Next(1, size.x - 1),
                    random.Next(0, size.y-1),
                    random.Next(1, size.z - 1)
                );

                endingRoom = new Room(endLocation, new Vector3Int(2, 2, 2));

                //Checks if bounds are outside of spawning area
                if (endingRoom.bounds.xMin >= 0 || endingRoom.bounds.xMax < size.x
                    || endingRoom.bounds.yMin >= 0 || endingRoom.bounds.yMax < size.y -1
                    || endingRoom.bounds.zMin >= 0 || endingRoom.bounds.zMax < size.z)
                {
                    add = true;
                    endRoomPos = endLocation;
                }
            }

            rooms.Add(endingRoom);

            foreach (var pos in endingRoom.bounds.allPositionsWithin)
            {
                grid[pos] = CellType.Room;
            }
        }

        //Create Each Room
        for (int i = 0; i < roomCount; i++)
        {
            //Random Loacation
            Vector3Int location = new Vector3Int(
                random.Next(0, size.x),
                random.Next(0, size.y),
                random.Next(0, size.z)
            );

            //Random Size
            Vector3Int roomSize = new Vector3Int(
                random.Next(1, roomMaxSize.x + 1),
                random.Next(1, roomMaxSize.y + 1),
                random.Next(1, roomMaxSize.z + 1)
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector3Int(-1, 0, -1), roomSize + new Vector3Int(2, 0, 2));

            //Checks with each existing room for intersection
            foreach (var room in rooms)
            {
                if (Room.Intersect(room, buffer))
                {
                    add = false;
                    break;
                }
            }

            //Checks if bounds are outside of spawming area
            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= size.y
                || newRoom.bounds.zMin < 0 || newRoom.bounds.zMax >= size.z)
            {
                add = false;
            }

            //Adds each room to the list and grid
            if (add)
            {
                if (roomSize.y > 1)
                {
                    Room room1 = new Room(location, roomSize - new Vector3Int(0, 1, 0));
                    Room room2 = new Room(location + new Vector3Int(0, 1, 0), roomSize - new Vector3Int(0, 1, 0));
                    rooms.Add(room1);
                    rooms.Add(room2);
                }
                else //Single Layer Room
                {
                    rooms.Add(newRoom);
                }

                foreach (var pos in newRoom.bounds.allPositionsWithin)
                {
                    grid[pos] = CellType.Room;
                }
            }
        }
    }

    void Triangulate()
    {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms)
        {
            vertices.Add(new Vertex<Room>((Vector3)room.bounds.position + ((Vector3)room.bounds.size) / 2, room));
        }

        delaunay = Delaunay3D.Triangulate(vertices);
    }

    void CreateHallways()
    {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges)
        {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> minimumSpanningTree = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(minimumSpanningTree);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges)
        {
            if (random.NextDouble() < 0.125)
            {
                selectedEdges.Add(edge);
            }
        }
    }

    void PathfindHallways()
    {
        DungeonPathfinder3D aStar = new DungeonPathfinder3D(size);

        foreach (var edge in selectedEdges)
        {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;
            var startPos = new Vector3Int((int)startPosf.x, (int)startPosf.y, (int)startPosf.z);
            var endPos = new Vector3Int((int)endPosf.x, (int)endPosf.y, (int)endPosf.z);

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder3D.Node a, DungeonPathfinder3D.Node b) => {
                var pathCost = new DungeonPathfinder3D.PathCost();

                var delta = b.Position - a.Position;

                if (delta.y == 0)
                {
                    //flat hallway
                    pathCost.cost = Vector3Int.Distance(b.Position, endPos);    //heuristic

                    if (grid[b.Position] == CellType.Stairs || grid[b.Position] == CellType.StairEnd)
                    {
                        return pathCost;
                    }
                    else if (grid[b.Position] == CellType.Room)
                    {
                        pathCost.cost += 5;
                    }
                    else if (grid[b.Position] == CellType.None)
                    {
                        pathCost.cost += 1;
                    }

                    pathCost.traversable = true;
                }
                else
                {
                    //staircase
                    if ((grid[a.Position] != CellType.None && grid[a.Position] != CellType.Hallway)
                        || (grid[b.Position] != CellType.None && grid[b.Position] != CellType.Hallway)) return pathCost;

                    pathCost.cost = 100 + Vector3Int.Distance(b.Position, endPos);    //base cost + heuristic

                    int xDir = Mathf.Clamp(delta.x, -1, 1);
                    int zDir = Mathf.Clamp(delta.z, -1, 1);
                    Vector3Int verticalOffset = new Vector3Int(0, delta.y, 0);
                    Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);

                    if (!grid.InBounds(a.Position + verticalOffset)
                        || !grid.InBounds(a.Position + horizontalOffset)
                        || !grid.InBounds(a.Position + verticalOffset + horizontalOffset))
                    {
                        return pathCost;
                    }

                    if (grid[a.Position + horizontalOffset] != CellType.None
                        || grid[a.Position + horizontalOffset * 2] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset] != CellType.None
                        || grid[a.Position + verticalOffset + horizontalOffset * 2] != CellType.None)
                    {
                        return pathCost;
                    }

                    pathCost.traversable = true;
                    pathCost.isStairs = true;
                }

                return pathCost;
            });

            if (path != null)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    var current = path[i];

                    if (grid[current] == CellType.None)
                    {
                        grid[current] = CellType.Hallway;
                    }
                    else if (grid[current] == CellType.Room)
                    {
                        grid[current] = CellType.RoomInPath;
                    }

                    if (i > 0)
                    {
                        var prev = path[i - 1];

                        var delta = current - prev;

                        if (delta.y != 0)
                        {
                            int xDir = Mathf.Clamp(delta.x, -1, 1);
                            int zDir = Mathf.Clamp(delta.z, -1, 1);
                            Vector3Int verticalOffset = new Vector3Int(0, delta.y, 0);
                            Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);

                            Vector3Int stairEndOne = prev + horizontalOffset;
                            Vector3Int stairEndTwo = prev + verticalOffset + horizontalOffset * 2;

                            grid[stairEndOne] = CellType.StairEnd;
                            grid[prev + horizontalOffset * 2] = CellType.Stairs;
                            grid[prev + verticalOffset + horizontalOffset] = CellType.Stairs;
                            grid[stairEndTwo] = CellType.StairEnd;

                            //Adds stairs to list
                            if (stairEndOne.y > stairEndTwo.y)
                            {
                                stairs.Add(new Stairs(stairEndTwo, DetermineStairDirection(stairEndTwo, stairEndOne)));
                            }
                            else
                            {
                                stairs.Add(new Stairs(stairEndOne, DetermineStairDirection(stairEndOne, stairEndTwo)));
                            }
                        }

                        Debug.DrawLine(prev + new Vector3(0.5f, 0.5f, 0.5f), current + new Vector3(0.5f, 0.5f, 0.5f), Color.blue, 100, false);
                    }
                }
            }
        }
    }

    private Direction DetermineStairDirection(Vector3Int startLocation, Vector3Int endLocation)
    {
        Vector3Int directionVector = endLocation - startLocation;

        // Determine stair direction 
        if (directionVector == new Vector3Int(0, 1, 1)) return Direction.North;
        else if (directionVector == new Vector3Int(1, 1, 0)) return Direction.East;
        else if (directionVector == new Vector3Int(0, 1, -1)) return Direction.South;
        else if (directionVector == new Vector3Int(-1, 1, 0)) return Direction.West;

        return Direction.None;
    }
}

