using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DungeonGenerator;
using Random = System.Random;
using Direction = DungeonGenerator.Direction;

public class FurnitureSpawner : MonoBehaviour
{
    [Header("Furniture Prefabs")]
    [SerializeField] Furniture[] hallwayTiles;
    [SerializeField] Furniture[] roomTiles;
    [SerializeField] Furniture[] lightTiles;
    [SerializeField] Transform GeneratedDungeonParent;

    //Dependencies
    public int seed;
    public Grid3D<CellType> grid;
    public Vector3Int gridSize;
    public List<Room> rooms;

    //Generated Items
    private Random rand;
    public Grid3D<Occupied> spawnGrid;
    [SerializeField] public List<Furniture> furnitureList;

    //Temp Variables
    private List<Vector3Int> positions;
    private List<Furniture> possibleFurniture;
    private List<Style> areaStyles;

    public enum HallType
    {
        Wall,
        Hall,
        Corner,
        Nook,
        Room
    }

    public enum Occupied
    {
        Empty,
        Wall,
        Full
    }

    public enum Style
    {
        Dungeon,
        Barracks,
        Nature,
        Dining,
        Storage,
        None
    }

    [Serializable]
    public class Furniture
    {
        [SerializeField] public GameObject prefab;
        [SerializeField] public Vector3Int size;
        [SerializeField] public Style style;
        [SerializeField] public HallType hallType;
        public int arrayIndex;
        public bool isHallway;
        public bool isLights;

        public Vector3Int pos;
        public float angle;
        public Vector3Int scale;

        Furniture(GameObject pre, Vector3Int size, Style style, Vector3Int pos, float angle, Vector3Int scale, int arrayIndex, bool isHallway, bool isLights)
        {
            this.prefab = pre;
            this.size = size;
            this.style = style;
            this.pos = pos;
            this.angle = angle;
            this.scale = scale;
            this.arrayIndex = arrayIndex;
            this.isHallway = isHallway;
            this.isLights = isLights;
        }

        public Furniture DeepCopy()
        {
            GameObject newPrefab = this.prefab;
            Vector3Int newSize = new Vector3Int(this.size.x, this.size.y, this.size.z);
            Style newStyle = this.style;
            return new Furniture(newPrefab, newSize, newStyle, new Vector3Int(0, 0, 0), 0f, new Vector3Int(1, 1, 1), this.arrayIndex, this.isHallway, this.isLights);
        }
    }

    public void GenerateFurniture()
    {
        rand = new Random(seed);
        spawnGrid = new Grid3D<Occupied>(gridSize, Vector3Int.zero);
        areaStyles = new List<Style>();

        HandleRooms();

        //Hallway Styles
        while (areaStyles.Count < 4)
        {
            Style newStyle = (Style)rand.Next(Enum.GetValues(typeof(Style)).Length);
            if (!areaStyles.Contains(newStyle))
            {
                areaStyles.Add(newStyle);
            }
        }


        for (int z = 0; z < gridSize.z; z++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    bool chance = rand.NextDouble() <= 0.8;
                    Vector3Int pos = new Vector3Int(x, y, z);

                    //Hallways
                    if (grid[pos] == CellType.Hallway && chance)
                    {
                        Style currentStyle = GetHallStyle(pos);
                        HallType currentType = GetHallType(pos);

                        Direction currentDirection = GetHallFurnitureDirection(pos, currentType);
                        possibleFurniture = GetPossibleHallwayFurniture(currentStyle, currentType);
                        if (possibleFurniture.Count > 0)
                        {
                            Furniture selected = possibleFurniture[rand.Next(0, possibleFurniture.Count)];
                            Furniture newFurniture = selected.DeepCopy();

                            newFurniture.isHallway = true;
                            newFurniture.pos = pos;
                            newFurniture.angle = (float)currentDirection * 90f;
                            furnitureList.Add(newFurniture);
                        }
                    }

                    chance = rand.NextDouble() <= 0.8;
                    //Lighting
                    if (chance)
                    {
                        if (CheckIfLightPlacable(pos, Direction.North))
                        {
                            Furniture newFurniture = lightTiles[0].DeepCopy();
                            newFurniture.isLights = true;
                            newFurniture.pos = pos;
                            furnitureList.Add(newFurniture);
                        }
                        if (CheckIfLightPlacable(pos, Direction.East))
                        {
                            Furniture newFurniture = lightTiles[1].DeepCopy();
                            newFurniture.isLights = true;
                            newFurniture.pos = pos;
                            furnitureList.Add(newFurniture);
                        }
                        if (CheckIfLightPlacable(pos, Direction.South))
                        {
                            Furniture newFurniture = lightTiles[2].DeepCopy();
                            newFurniture.isLights = true;
                            newFurniture.pos = pos;
                            furnitureList.Add(newFurniture);
                        }
                        if (CheckIfLightPlacable(pos, Direction.West))
                        {
                            Furniture newFurniture = lightTiles[3].DeepCopy();
                            newFurniture.isLights = true;
                            newFurniture.pos = pos;
                            furnitureList.Add(newFurniture);
                        }
                    }
                }
            }
        }

        LoadFurniture();
    }

    bool CheckIfLightPlacable(Vector3Int pos, Direction dir)
    {
        Vector3Int location = pos;
        if (dir == Direction.North) location += Vector3Int.forward;
        else if (dir == Direction.East) location += Vector3Int.right;
        else if (dir == Direction.South) location += Vector3Int.back;
        else location += Vector3Int.left;

        if (IsWithinGridBounds(location))
        {
            switch (grid[pos])
            {
                case CellType.Room:
                    Vector3Int roomPosTwo = pos;
                    if (dir == Direction.North) roomPosTwo += Vector3Int.left;
                    else if (dir == Direction.East) roomPosTwo += Vector3Int.forward;
                    else if (dir == Direction.South) roomPosTwo += Vector3Int.right;
                    else roomPosTwo += Vector3Int.back;

                    if (IsWithinGridBounds(roomPosTwo))
                    {
                        if ((grid[location] == CellType.Room || grid[location] == CellType.RoomInPath) && (grid[roomPosTwo] != CellType.Room && grid[roomPosTwo] != CellType.RoomInPath)) return true;
                    }
                    else if ((grid[location] == CellType.Room || grid[location] == CellType.RoomInPath))
                    {
                        return true;
                    }

                    break;
                case CellType.Hallway:
                    if (grid[location] == CellType.Hallway) return true;
                    break;
                case CellType.RoomInPath:

                    Vector3Int ripPostwo = pos;
                    if (dir == Direction.North) ripPostwo += Vector3Int.left;
                    else if (dir == Direction.East) ripPostwo += Vector3Int.forward;
                    else if (dir == Direction.South) ripPostwo += Vector3Int.right;
                    else ripPostwo += Vector3Int.back;

                    if (IsWithinGridBounds(ripPostwo))
                    {
                        if ((grid[location] == CellType.Room || grid[location] == CellType.RoomInPath) && (grid[ripPostwo] != CellType.Room && grid[ripPostwo] != CellType.RoomInPath)) return true;
                    }
                    else if ((grid[location] == CellType.Room || grid[location] == CellType.RoomInPath))
                    {
                        return true;
                    }
                    break;
                case CellType.Stairs:
                    break;
                case CellType.StairEnd:
                    break;
                case CellType.StartingRoom:
                    if (grid[location] == CellType.Hallway) return true;
                    break;
                case CellType.EndingRoom:
                    break;
            }
        }
        return false;
    }

    //Hallway Handling
    Style GetHallStyle(Vector3Int pos)
    {
        if (pos.x >= (gridSize.x / 2))
        {
            if (pos.y >= (gridSize.y / 2))
            {
                return areaStyles[3];
            }
            else
            {
                return areaStyles[1];
            }
        }
        else
        {
            if (pos.y >= (gridSize.y / 2))
            {
                return areaStyles[2];
            }
            else
            {
                return areaStyles[0];
            }
        }
    }

    HallType GetHallType(Vector3Int pos)
    {

        bool[] isWall = new bool[4];

        if ((!IsWithinGridBounds(pos + Vector3Int.forward))
            || (IsWithinGridBounds(pos + Vector3Int.forward) && (grid[pos + Vector3Int.forward] == CellType.None || grid[pos + Vector3Int.forward] == CellType.Room)))
        {
            isWall[0] = true;
        }
        if ((!IsWithinGridBounds(pos + Vector3Int.right))
            || (IsWithinGridBounds(pos + Vector3Int.right) && (grid[pos + Vector3Int.right] == CellType.None || grid[pos + Vector3Int.right] == CellType.Room)))
        {
            isWall[1] = true;
        }
        if ((!IsWithinGridBounds(pos + Vector3Int.back))
            || (IsWithinGridBounds(pos + Vector3Int.back) && (grid[pos + Vector3Int.back] == CellType.None || grid[pos + Vector3Int.back] == CellType.Room)))
        {
            isWall[2] = true;
        }
        if ((!IsWithinGridBounds(pos + Vector3Int.left))
            || (IsWithinGridBounds(pos + Vector3Int.left) && (grid[pos + Vector3Int.left] == CellType.None || grid[pos + Vector3Int.left] == CellType.Room)))
        {
            isWall[3] = true;
        }

        int sum = isWall.Count(b => b);

        if (sum == 0) return HallType.Room;
        else if (sum == 1) return HallType.Wall;
        else if (sum == 2)
        {
            if ((isWall[0] && isWall[2]) || (isWall[1] && isWall[3])) return HallType.Hall;
            else return HallType.Corner;
        }
        else return HallType.Nook;
    }

    Direction GetHallFurnitureDirection(Vector3Int pos, HallType type)
    {
        bool[] isWall = new bool[4];

        if ((!IsWithinGridBounds(pos + Vector3Int.forward))
            || (IsWithinGridBounds(pos + Vector3Int.forward) && (grid[pos + Vector3Int.forward] == CellType.None || grid[pos + Vector3Int.forward] == CellType.Room)))
        {
            isWall[0] = true;
        }
        if ((!IsWithinGridBounds(pos + Vector3Int.right))
            || (IsWithinGridBounds(pos + Vector3Int.right) && (grid[pos + Vector3Int.right] == CellType.None || grid[pos + Vector3Int.right] == CellType.Room)))
        {
            isWall[1] = true;
        }
        if ((!IsWithinGridBounds(pos + Vector3Int.back))
            || (IsWithinGridBounds(pos + Vector3Int.back) && (grid[pos + Vector3Int.back] == CellType.None || grid[pos + Vector3Int.back] == CellType.Room)))
        {
            isWall[2] = true;
        }
        if ((!IsWithinGridBounds(pos + Vector3Int.left))
            || (IsWithinGridBounds(pos + Vector3Int.left) && (grid[pos + Vector3Int.left] == CellType.None || grid[pos + Vector3Int.left] == CellType.Room)))
        {
            isWall[3] = true;
        }

        switch (type)
        {
            case HallType.Wall:
                if (isWall[0]) return Direction.North;
                else if (isWall[1]) return Direction.East;
                else if (isWall[2]) return Direction.South;
                else return Direction.West;
            case HallType.Hall:
                if (isWall[0] && isWall[2])
                {
                    bool decider = rand.Next(0, 2) == 0;
                    if (decider) return Direction.North;
                    else return Direction.South;
                }
                else
                {
                    bool decider = rand.Next(0, 2) == 0;
                    if (decider) return Direction.East;
                    else return Direction.West;
                }
            case HallType.Corner:
                if (isWall[0] && isWall[1]) return Direction.North;
                else if (isWall[1] && isWall[2]) return Direction.East;
                else if (isWall[2] && isWall[3]) return Direction.South;
                else return Direction.West;
            case HallType.Nook:
                if (!isWall[2]) return Direction.North;
                else if (!isWall[3]) return Direction.East;
                else if (!isWall[0]) return Direction.South;
                else return Direction.West;

        }
        return Direction.None;
    }

    List<Furniture> GetPossibleHallwayFurniture(Style style, HallType type)
    {
        List<Furniture> furnitures = new List<Furniture>();

        foreach (Furniture furniture in hallwayTiles)
        {
            if (furniture.style == style && furniture.hallType == type)
            {
                furnitures.Add(furniture);
            }
        }
        return furnitures;
    }

    //Room Handling 
    void HandleRooms()
    {
        //Going through each room
        foreach (var room in rooms)
        {
            positions = new List<Vector3Int>();
            possibleFurniture = new List<Furniture>();

            //Gets all viable positions where we can place stuff
            foreach (var pos in room.bounds.allPositionsWithin)
            {
                if (CheckIfOccupied(pos) != Occupied.Full)
                {
                    positions.Add(pos);
                }
            }

            Style style = getRoomStyle(positions.Count);
            possibleFurniture = GetPossibleFurniture(style, (room.bounds.size.x > room.bounds.size.z) ? room.bounds.size.x : room.bounds.size.z);

            int groups = 0;

            do
            {
                List<Furniture> group = new List<Furniture>();
                groups = GetFurnitureGroup(group, possibleFurniture);

                FillRoom(group, positions.Count / groups, positions);

            } while (groups > 1);
        }
    }

    int GetFurnitureGroup(List<Furniture> furnitureGroup, List<Furniture> possibleFurniture)
    {
        int count = 0;
        Vector3Int maxSize = Vector3Int.zero;

        foreach (Furniture current in possibleFurniture)
        {
            if (IsVectorGreater(maxSize, current.size) == 0)
            {
                //Same size
                furnitureGroup.Add(current);
            }
            else if (IsVectorGreater(maxSize, current.size) == -1)
            {
                maxSize = current.size;
                count++;
                furnitureGroup.Clear();
                furnitureGroup.Add(current);
            }
        }

        foreach (Furniture current in furnitureGroup)
        {
            possibleFurniture.Remove(current);
        }

        return count;
    }

    void FillRoom(List<Furniture> furniture, int maxPlacements, List<Vector3Int> positions)
    {
        int timesPlaced = 0;

        List<Vector3Int> currentPosList = new List<Vector3Int>();
        foreach (Vector3Int pos in positions) currentPosList.Add(pos);

        Furniture currentFurniture = furniture[0];

        while (currentPosList.Count > 0 && timesPlaced < maxPlacements)
        {
            currentFurniture = furniture[rand.Next(0, furniture.Count)];

            Vector3Int pos = currentPosList[rand.Next(0, currentPosList.Count)];
            currentPosList.Remove(pos);

            if (CheckIfFits(pos, currentFurniture)) timesPlaced++;
        }
    }

    bool CheckIfFits(Vector3Int pos, Furniture furniture)
    {
        if (furniture.size.x == 1 && furniture.size.z == 1)
        {
            if (CheckIfOccupied(pos) != Occupied.Full)
            {
                spawnGrid[pos] = Occupied.Full;

                bool xFlipped = rand.Next(0, 2) == 0;
                bool zFlipped = rand.Next(0, 2) == 0;
                Direction direction = (Direction)rand.Next(0, 4);

                Furniture newFurniture = furniture.DeepCopy();

                if (xFlipped && zFlipped) newFurniture.scale = new Vector3Int(-1, 1, -1);
                else if (xFlipped) newFurniture.scale = new Vector3Int(-1, 1, 1);
                else if (zFlipped) newFurniture.scale = new Vector3Int(1, 1, -1);

                newFurniture.pos = pos;
                newFurniture.angle = 90f * (float)direction;
                furnitureList.Add(newFurniture);

                return true;
            }
        }
        else
        {
            List<BoundsInt> boundPossibilities = new List<BoundsInt>();

            boundPossibilities.Add(GetBoundsInDirection(pos, furniture, Direction.North));
            boundPossibilities.Add(GetBoundsInDirection(pos, furniture, Direction.East));
            boundPossibilities.Add(GetBoundsInDirection(pos, furniture, Direction.South));
            boundPossibilities.Add(GetBoundsInDirection(pos, furniture, Direction.West));

            //Go through each possibility
            while (boundPossibilities.Count > 0)
            {
                //Get a random direction
                BoundsInt boundsInt = boundPossibilities[rand.Next(0, boundPossibilities.Count)];
                boundPossibilities.Remove(boundsInt);

                bool boundsCheck = true;

                //Going through each position in bounds to check if occupied
                foreach (Vector3Int currentPos in boundsInt.allPositionsWithin)
                {
                    if (CheckIfOccupied(currentPos) == Occupied.Full)
                    {
                        boundsCheck = false;
                    }
                }

                //If all positions were empty
                if (boundsCheck)
                {
                    UpdateOccupied(boundsInt);
                    Furniture newFurniture = furniture.DeepCopy();
                    float angle = 0f;

                    if (CompareBoundsInt(boundsInt, GetBoundsInDirection(pos, furniture, Direction.North))) angle = 0f;
                    else if (CompareBoundsInt(boundsInt, GetBoundsInDirection(pos, furniture, Direction.East))) angle = 90f;
                    else if (CompareBoundsInt(boundsInt, GetBoundsInDirection(pos, furniture, Direction.South))) angle = 180f;
                    else if (CompareBoundsInt(boundsInt, GetBoundsInDirection(pos, furniture, Direction.West))) angle = 270f;

                    newFurniture.pos = pos;

                    angle = Mathf.Repeat(angle, 360f);
                    newFurniture.angle = angle;
                    furnitureList.Add(newFurniture);

                    return true;
                }

            }
        }

        return false;
    }

    bool CompareBoundsInt(BoundsInt boundsInt, BoundsInt boundsIntTwo)
    {
        if (boundsInt.position.x == boundsIntTwo.position.x && boundsInt.position.z == boundsIntTwo.position.z && boundsInt.max.x == boundsIntTwo.max.x && boundsInt.max.z == boundsIntTwo.max.z) { return true; }
        return false;
    }

    BoundsInt GetBoundsInDirection(Vector3Int pos, Furniture furniture, Direction direction)
    {
        BoundsInt bounds = new BoundsInt();
        switch (direction)
        {
            case Direction.North:
                bounds = new BoundsInt(pos, furniture.size);
                break;
            case Direction.East:
                bounds = new BoundsInt(new(pos.x, pos.y, pos.z - (furniture.size.x - 1)), new Vector3Int(furniture.size.z, 1, furniture.size.x));
                break;
            case Direction.South:
                bounds = new BoundsInt(new(pos.x - (furniture.size.x - 1), pos.y, pos.z - (furniture.size.z - 1)), furniture.size);
                break;
            default:
                bounds = new BoundsInt(new(pos.x - (furniture.size.z - 1), pos.y, pos.z), new Vector3Int(furniture.size.z, 1, furniture.size.x));
                break;

        }
        return bounds;
    }

    int IsVectorGreater(Vector3Int posOne, Vector3Int posTwo)
    {
        int sumOne = posOne.x + posOne.z;
        int sumTwo = posTwo.x + posTwo.z;

        if (sumOne < sumTwo) return -1;
        else if (sumOne == sumTwo) return 0;
        else return 1;
    }

    List<Furniture> GetPossibleFurniture(Style style, int maxSide)
    {
        List<Furniture> furnitures = new List<Furniture>();

        foreach (Furniture furniture in roomTiles)
        {
            if (furniture.style == style && furniture.size.x <= maxSide && furniture.size.z <= maxSide)
            {
                furnitures.Add(furniture);
            }
        }
        return furnitures;
    }

    void UpdateOccupied(BoundsInt bounds)
    {
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            spawnGrid[pos] = Occupied.Full;
        }
    }

    Occupied CheckIfOccupied(Vector3Int pos)
    {
        if (!IsWithinGridBounds(pos)) return Occupied.Full;
        if (grid[pos] != CellType.Room)
        {
            spawnGrid[pos] = Occupied.Full;
        }
        else if (IsWithinGridBounds(pos + Vector3Int.down))
        {
            if (grid[pos + Vector3Int.down] == CellType.Room || grid[pos + Vector3Int.down] == CellType.RoomInPath)
            {
                spawnGrid[pos] = Occupied.Full;
            }
        }
        return spawnGrid[pos];
    }

    Style getRoomStyle(int viablePositions)
    {
        if (viablePositions > 20)
        {
            return Style.Dining;
        }
        else if (viablePositions > 15)
        {
            return Style.Barracks;
        }
        else if (viablePositions > 10)
        {
            return Style.Dungeon;
        }
        else if (viablePositions > 5)
        {
            return Style.Nature;
        }
        else if (viablePositions > 1)
        {
            return Style.Storage;
        }
        return Style.None;
    }

    private bool IsWithinGridBounds(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize.x &&
               pos.y >= 0 && pos.y < gridSize.y &&
               pos.z >= 0 && pos.z < gridSize.z;
    }

    void PlaceFurnitureFromLoad(Furniture furn)
    {
        GameObject prefab;
        if (furn.isHallway)
        {
            prefab = hallwayTiles[furn.arrayIndex].prefab;
        }
        else if (furn.isLights)
        {
            prefab = lightTiles[furn.arrayIndex].prefab;
        }
        else
        {
            prefab = roomTiles[furn.arrayIndex].prefab;
        }

        Vector3 position = new Vector3((furn.pos.x * 6.5f), (furn.pos.y * 5f), (furn.pos.z * 6.5f));
        Quaternion rot = Quaternion.Euler(0, furn.angle, 0);
        GameObject go = Instantiate(prefab, position, rot);
        go.GetComponent<Transform>().localScale = furn.scale;
        Transform furnitureParent = GeneratedDungeonParent.Find("Furniture");
        go.transform.SetParent(furnitureParent);
    }

    public void LoadFurniture()
    {
        foreach (var furniture in furnitureList)
        {
            PlaceFurnitureFromLoad(furniture);
        }
    }
}
