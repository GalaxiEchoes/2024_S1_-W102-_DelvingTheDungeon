using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static DungeonGenerator;
using static UnityEditor.FilePathAttribute;
using static UnityEditor.PlayerSettings;
using Direction = DungeonGenerator.Direction;
using Random = System.Random;

public class DungeonSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] Transform GeneratedDungeonParent;
    [SerializeField] GameObject wallPreFab;
    [SerializeField] GameObject wallPostFab;
    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject doorWayPrefab;
    [SerializeField] GameObject stairPrefab;
    [SerializeField] GameObject railPrefab;
    [SerializeField] GameObject startPrefab;
    [SerializeField] GameObject endPrefab;
    [SerializeField] GameObject bossPrefab;

    [Header("Game Settings")]
    public Vector3Int gridSize;
    public Grid3D<CellType> grid;
    public List<Stairs> stairs;
    public int seed;
    public int currentLevel;
    public int bossLevel;
    public Vector3 spawnLocation;

    Random random;
    bool endHandled = false;

    public void SpawnDungeonRooms()
    {
        random = new Random(seed);
        IterateGrid();
    }

    public void SpawnDungeonRooms(Vector3Int gridSize, Grid3D<CellType> grid, List<Stairs> stairs, int seed, int currentLevel, int bossLevel)
    {
        random = new Random(seed);
        this.gridSize = gridSize;
        this.grid = grid;
        this.stairs = stairs;
        this.seed = seed;
        this.currentLevel = currentLevel;
        this.bossLevel = bossLevel;
        IterateGrid();
    }

    private void IterateGrid()
    {
        for (int z = 0; z < gridSize.z; z++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);

                    switch (grid[pos])
                    {
                        case CellType.None:
                            HandleNone(pos);
                            break;
                        case CellType.Room:
                            HandleRoom(pos);
                            break;
                        case CellType.Hallway:
                            HandleHallway(pos);
                            break;
                        case CellType.RoomInPath:
                            HandleRoomInPath(pos);
                            break;
                        case CellType.StairEnd:
                            if (GetStairDirection(pos) != Direction.None) HandleStairs(pos);
                            break;
                        case CellType.StartingRoom:
                            HandleStartRoom(pos);
                            break;
                        case CellType.EndingRoom:
                            if (currentLevel != bossLevel) HandleEndingRoom(pos);
                            else HandleBossRoom(pos);
                            break;
                    }

                    HandleBorder(pos);
                }
            }
        }
    }

    void HandleStartRoom(Vector3Int pos)
    {
        PlaceFloor(pos);
        PlaceFloor(pos + Vector3Int.up);
        PlacePillar(pos, Direction.North);
        if (IsWithinGridBounds(pos + Vector3Int.forward) && grid[pos + Vector3Int.forward] != CellType.Hallway) PlaceWall(pos, Direction.North, wallPreFab);
        if (IsWithinGridBounds(pos + Vector3Int.right) && grid[pos + Vector3Int.right] != CellType.Hallway) PlaceWall(pos, Direction.East, wallPreFab);
        PlaceStart(new Vector3Int(pos.x, pos.y, -1));

        if (currentLevel == 0)
        {
            PlaceWall(new(pos.x, pos.y, -4), Direction.South, wallPreFab);
            spawnLocation = new Vector3(((pos.x - 5) * 6.5f), (pos.y * 5f) + 2, ((pos.z - 3) * 6.5f));
        }
        else
        {
            PlaceWall(new(pos.x - 2, pos.y, -3), Direction.East, wallPreFab);
            spawnLocation = new Vector3((pos.x * 6.5f), (pos.y * 5f) + 2, ((pos.z - 5) * 6.5f));
        }
    }

    void HandleBossRoom(Vector3Int pos)
    {
        PlaceFloor(pos);
        PlaceFloor(pos + Vector3Int.up);
        PlacePillar(pos, Direction.North);
        if (IsWithinGridBounds(pos + Vector3Int.right) && grid[pos + Vector3Int.right] != CellType.Hallway) PlaceWall(pos, Direction.East, wallPreFab);
        PlaceBossRoom(new Vector3Int(pos.x, pos.y, gridSize.z));
    }

    void HandleEndingRoom(Vector3Int pos)
    {
        Vector3Int northPos = pos + Vector3Int.forward;
        Vector3Int eastPos = pos + Vector3Int.right;

        //Pillar
        PlacePillar(pos, Direction.North);

        //Walls
        if (IsWithinGridBounds(northPos))
        {
            if (grid[northPos] != CellType.Hallway && grid[northPos] != CellType.EndingRoom) PlaceWall(pos, Direction.North, wallPreFab);
        }
        else
        {
            PlaceWall(pos, Direction.North, wallPreFab);
        }
        if (IsWithinGridBounds(eastPos))
        {
            if (grid[eastPos] != CellType.Hallway && grid[eastPos] != CellType.EndingRoom) PlaceWall(pos, Direction.East, wallPreFab);
        }
        else
        {
            PlaceWall(pos, Direction.East, wallPreFab);
        }

        //Placing room interior
        if (!endHandled)
        {
            Vector3Int[] posToCheck = {
                new(0, 1, 2),
                new(1, 1, 2),
                new(2, 1, 1),
                new(2, 1, 0),
                new(1, 1, -1),
                new(0, 1, -1),
                new(-1, 1, 0),
                new(-1, 1, 1)
            };

            List<Vector3Int> locations = new List<Vector3Int>();

            //Checking each position it could connect
            foreach (Vector3Int checkPos in posToCheck)
            {
                if (IsWithinGridBounds(pos + checkPos) && grid[pos + checkPos] == CellType.Hallway)
                    locations.Add(checkPos);
            }

            //Randomly choosing a position

            if (locations.Count != 0)
            {
                Vector3Int connectPoint = locations[random.Next(0, locations.Count)];
                Direction direction = Direction.None;
                int scale = 1;

                if (connectPoint == posToCheck[1] || connectPoint == posToCheck[3] || connectPoint == posToCheck[5] || connectPoint == posToCheck[7]) scale = -1;

                //Getting direction
                if (connectPoint == posToCheck[0] || connectPoint == posToCheck[1]) direction = Direction.North;
                else if (connectPoint == posToCheck[2] || connectPoint == posToCheck[3]) direction = Direction.East;
                else if (connectPoint == posToCheck[4] || connectPoint == posToCheck[5]) direction = Direction.South;
                else direction = Direction.West;

                PlaceEnd(pos, direction, scale);

                //Handle Walls
                switch (direction)
                {
                    case Direction.North:

                        if (scale == 1)
                        {
                            grid[pos + Vector3Int.up + Vector3Int.right] = CellType.Hallway;
                            grid[pos + Vector3Int.up + Vector3Int.right + Vector3Int.forward] = CellType.Hallway;
                            if (grid[pos + posToCheck[5]] == CellType.Hallway) PlaceWall(pos + Vector3Int.up + Vector3Int.back, Direction.North, wallPreFab);
                            if (grid[pos + posToCheck[6]] == CellType.Hallway) PlaceWall(pos + posToCheck[6], Direction.East, wallPreFab);
                            if (grid[pos + posToCheck[7]] == CellType.Hallway) PlaceWall(pos + posToCheck[7], Direction.East, wallPreFab);
                        }
                        else
                        {

                            grid[pos + Vector3Int.up] = CellType.Hallway;
                            grid[pos + Vector3Int.up + Vector3Int.forward] = CellType.Hallway;
                            if (grid[pos + posToCheck[2]] == CellType.Hallway) PlaceWall(pos + Vector3Int.up + Vector3Int.forward + Vector3Int.right, Direction.East, wallPreFab);
                            if (grid[pos + posToCheck[3]] == CellType.Hallway) PlaceWall(pos + Vector3Int.right + Vector3Int.up, Direction.East, wallPreFab);
                            if (grid[pos + posToCheck[4]] == CellType.Hallway) PlaceWall(pos + Vector3Int.up + Vector3Int.back + Vector3Int.right, Direction.North, wallPreFab);
                        }
                        break;
                    case Direction.East:
                        if (scale == 1)
                        {
                            grid[pos + Vector3Int.up] = CellType.Hallway;
                            grid[pos + Vector3Int.up + Vector3Int.right] = CellType.Hallway;
                            if (grid[pos + posToCheck[7]] == CellType.Hallway) PlaceWall(pos + posToCheck[7], Direction.East, wallPreFab);
                            if (grid[pos + posToCheck[0]] == CellType.Hallway) PlaceWall(pos + Vector3Int.forward + Vector3Int.up, Direction.North, wallPreFab);
                            if (grid[pos + posToCheck[1]] == CellType.Hallway) PlaceWall(pos + Vector3Int.up + Vector3Int.forward + Vector3Int.right, Direction.North, wallPreFab);
                        }
                        else
                        {
                            grid[pos + Vector3Int.up + Vector3Int.forward] = CellType.Hallway;

                            grid[pos + Vector3Int.up + Vector3Int.forward + Vector3Int.right] = CellType.Hallway;
                            if (grid[pos + posToCheck[4]] == CellType.Hallway) PlaceWall(pos + Vector3Int.up + Vector3Int.back + Vector3Int.right, Direction.North, wallPreFab);
                            if (grid[pos + posToCheck[5]] == CellType.Hallway) PlaceWall(pos + Vector3Int.up + Vector3Int.back, Direction.North, wallPreFab);
                            if (grid[pos + posToCheck[6]] == CellType.Hallway) PlaceWall(pos + posToCheck[6], Direction.East, wallPreFab);
                        }
                        break;
                    case Direction.South:
                        if (scale == 1)
                        {
                            grid[pos + Vector3Int.up] = CellType.Hallway;
                            grid[pos + Vector3Int.up + Vector3Int.forward] = CellType.Hallway;
                            if (grid[pos + posToCheck[1]] == CellType.Hallway) PlaceWall(pos + Vector3Int.up + Vector3Int.forward + Vector3Int.right, Direction.North, wallPreFab);
                            if (grid[pos + posToCheck[2]] == CellType.Hallway) PlaceWall(pos + Vector3Int.forward + Vector3Int.up + Vector3Int.right, Direction.East, wallPreFab);
                            if (grid[pos + posToCheck[3]] == CellType.Hallway) PlaceWall(pos + Vector3Int.up + Vector3Int.right, Direction.East, wallPreFab);
                        }
                        else
                        {
                            grid[pos + Vector3Int.up + Vector3Int.right] = CellType.Hallway;
                            grid[pos + Vector3Int.up + Vector3Int.forward + Vector3Int.right] = CellType.Hallway;
                            if (grid[pos + posToCheck[6]] == CellType.Hallway) PlaceWall(pos + posToCheck[6], Direction.East, wallPreFab);
                            if (grid[pos + posToCheck[7]] == CellType.Hallway) PlaceWall(pos + posToCheck[7], Direction.East, wallPreFab);
                            if (grid[pos + posToCheck[0]] == CellType.Hallway) PlaceWall(pos + Vector3Int.forward + Vector3Int.up, Direction.North, wallPreFab);
                        }
                        break;
                    case Direction.West:
                        if (scale == 1)
                        {

                            grid[pos + Vector3Int.up + Vector3Int.right + Vector3Int.forward] = CellType.Hallway;
                            grid[pos + Vector3Int.up + Vector3Int.forward] = CellType.Hallway;
                            if (grid[pos + posToCheck[3]] == CellType.Hallway) PlaceWall(pos + Vector3Int.up + Vector3Int.right, Direction.East, wallPreFab);
                            if (grid[pos + posToCheck[4]] == CellType.Hallway) PlaceWall(pos + Vector3Int.back + Vector3Int.up + Vector3Int.right, Direction.North, wallPreFab);
                            if (grid[pos + posToCheck[5]] == CellType.Hallway) PlaceWall(pos + Vector3Int.up + Vector3Int.back, Direction.North, wallPreFab);
                        }
                        else
                        {
                            grid[pos + Vector3Int.up] = CellType.Hallway;
                            grid[pos + Vector3Int.up + Vector3Int.right] = CellType.Hallway;
                            if (grid[pos + posToCheck[0]] == CellType.Hallway) PlaceWall(pos + Vector3Int.up + Vector3Int.forward, Direction.North, wallPreFab);
                            if (grid[pos + posToCheck[1]] == CellType.Hallway) PlaceWall(pos + Vector3Int.right + Vector3Int.up + Vector3Int.forward, Direction.North, wallPreFab);
                            if (grid[pos + posToCheck[2]] == CellType.Hallway) PlaceWall(pos + Vector3Int.forward + Vector3Int.up + Vector3Int.right, Direction.East, wallPreFab);
                        }
                        break;
                }

                if (grid[pos + posToCheck[0] + Vector3Int.down] == CellType.Hallway) PlaceWall(pos  + Vector3Int.forward, Direction.North, wallPreFab);
                if (grid[pos + posToCheck[1] + Vector3Int.down] == CellType.Hallway) PlaceWall(pos + Vector3Int.right + Vector3Int.forward, Direction.North, wallPreFab);
                if (grid[pos + posToCheck[2] + Vector3Int.down] == CellType.Hallway) PlaceWall(pos + Vector3Int.forward +  Vector3Int.right, Direction.East, wallPreFab);
                if (grid[pos + posToCheck[3] + Vector3Int.down] == CellType.Hallway) PlaceWall(pos +  Vector3Int.right, Direction.East, wallPreFab);
                if (grid[pos + posToCheck[4] + Vector3Int.down] == CellType.Hallway) PlaceWall(pos +  Vector3Int.back + Vector3Int.right, Direction.North, wallPreFab);
                if (grid[pos + posToCheck[5] + Vector3Int.down] == CellType.Hallway) PlaceWall(pos +  Vector3Int.back, Direction.North, wallPreFab);
                if (grid[pos + posToCheck[6] + Vector3Int.down] == CellType.Hallway) PlaceWall(pos + posToCheck[6] + Vector3Int.down, Direction.East, wallPreFab);
                if (grid[pos + posToCheck[7] + Vector3Int.down] == CellType.Hallway) PlaceWall(pos + posToCheck[7] + Vector3Int.down, Direction.East, wallPreFab);

                endHandled = true;
            }
        }
    }

    void HandleBorder(Vector3Int pos)
    {
        if (!IsWithinGridBounds(pos + Vector3Int.left) || !IsWithinGridBounds(pos + Vector3Int.back)) //On Boundary
        {
            if (pos.x == 0 && pos.z == 0)
            {
                if (grid[pos] == CellType.StartingRoom)
                {
                    PlacePillar(pos + new Vector3Int(-1, 0, -1), Direction.North);
                    PlaceWall(pos + Vector3Int.left, Direction.East, wallPreFab);
                }
                else if (grid[pos] != CellType.None)
                {
                    PlacePillar(pos + new Vector3Int(-1, 0, -1), Direction.North);
                    PlaceWall(pos + Vector3Int.back, Direction.North, wallPreFab);
                    PlaceWall(pos + Vector3Int.left, Direction.East, wallPreFab);
                }
            }
            else if (pos.z == 0) //South border
            {
                if (grid[pos + Vector3Int.left] != CellType.None || grid[pos] != CellType.None)
                {
                    PlacePillar(pos + new Vector3Int(-1, 0, -1), Direction.North);
                }
                if (grid[pos] != CellType.None && grid[pos] != CellType.StartingRoom) PlaceWall(pos + Vector3Int.back, Direction.North, wallPreFab);
            }
            else if (pos.x == 0) //West Border
            {
                if (grid[pos + Vector3Int.back] != CellType.None || grid[pos] != CellType.None)
                {
                    PlacePillar(pos + new Vector3Int(-1, 0, -1), Direction.North);
                }

                if (!IsWithinGridBounds(pos + Vector3Int.forward) && grid[pos] != CellType.None)
                {
                    PlacePillar(pos + new Vector3Int(-1, 0, 0), Direction.North);
                }
                if (grid[pos] != CellType.None) PlaceWall(pos + Vector3Int.left, Direction.East, wallPreFab);
            }
        }
        else if (grid[pos] != CellType.None && grid[pos + Vector3Int.left] == CellType.None && grid[pos + Vector3Int.back] == CellType.None) //Corners
        {
            PlacePillar(pos + new Vector3Int(-1, 0, -1), Direction.North);
        }
    }

    void HandleHallway(Vector3Int pos)
    {
        Vector3Int northPos = pos + Vector3Int.forward;
        Vector3Int eastPos = pos + Vector3Int.right;

        PlacePillar(pos, Direction.North);
        PlaceFloor(pos);
        PlaceFloor(pos + Vector3Int.up);

        //Walls
        if (IsWithinGridBounds(northPos))
        {
            if (grid[northPos] == CellType.Room || grid[northPos] == CellType.None) PlaceWall(pos, Direction.North, wallPreFab);
            if (grid[northPos] == CellType.RoomInPath) PlaceWall(pos, Direction.North, doorWayPrefab);
        }
        else
        {
            PlaceWall(pos, Direction.North, wallPreFab);
        }

        if (IsWithinGridBounds(eastPos))
        {
            if (grid[eastPos] == CellType.Room || grid[eastPos] == CellType.None) PlaceWall(pos, Direction.East, wallPreFab);
            if (grid[eastPos] == CellType.RoomInPath) PlaceWall(pos, Direction.East, doorWayPrefab);
        }
        else
        {
            PlaceWall(pos, Direction.East, wallPreFab);
        }
    }

    void HandleNone(Vector3Int pos)
    {
        Vector3Int northPos = pos + Vector3Int.forward;
        Vector3Int eastPos = pos + Vector3Int.right;
        bool createPillar = false;

        if (IsWithinGridBounds(northPos) && grid[northPos] != CellType.None)
        {
            PlaceWall(pos, Direction.North, wallPreFab);
            createPillar = true;
        }

        if (IsWithinGridBounds(eastPos) && grid[eastPos] != CellType.None)
        {
            PlaceWall(pos, Direction.East, wallPreFab);
            createPillar = true;
        }

        if (createPillar) PlacePillar(pos, Direction.North);
    }

    void HandleRoom(Vector3Int pos)
    {
        Vector3Int northPos = pos + Vector3Int.forward;
        Vector3Int eastPos = pos + Vector3Int.right;

        //Floor
        if (IsWithinGridBounds(pos + Vector3Int.down) && (grid[pos + Vector3Int.down] != CellType.Room && grid[pos + Vector3Int.down] != CellType.RoomInPath))
        {
            PlaceFloor(pos);
        }
        else if (!IsWithinGridBounds(pos + Vector3Int.down))
        {
            PlaceFloor(pos);
        }

        //Roof
        if (!IsWithinGridBounds(pos + Vector3Int.up)) PlaceFloor(pos + Vector3Int.up);
        else if (grid[pos + Vector3Int.up] != CellType.Room && grid[pos + Vector3Int.up] != CellType.RoomInPath) PlaceFloor(pos + Vector3Int.up);

        //Walls
        if (IsWithinGridBounds(northPos))
        {
            if (grid[northPos] != CellType.Room && grid[northPos] != CellType.RoomInPath) PlaceWall(pos, Direction.North, wallPreFab);
        }
        else
        {
            PlaceWall(pos, Direction.North, wallPreFab);
        }

        if (IsWithinGridBounds(eastPos))
        {
            if (grid[eastPos] != CellType.Room && grid[eastPos] != CellType.RoomInPath) PlaceWall(pos, Direction.East, wallPreFab);
        }
        else
        {
            PlaceWall(pos, Direction.East, wallPreFab);
        }

        //Pillar
        if ((!IsWithinGridBounds(northPos) || !IsWithinGridBounds(eastPos)))
        {
            PlacePillar(pos, Direction.North);
        }
        else
        {
            if ((grid[northPos] != CellType.Room && grid[northPos] != CellType.RoomInPath) || (grid[eastPos] != CellType.Room && grid[eastPos] != CellType.RoomInPath))
            {
                PlacePillar(pos, Direction.North);
            }
        }

        //Railing
        if (IsWithinGridBounds(pos + Vector3Int.forward) && IsWithinGridBounds(pos + Vector3Int.down))
        {
            if (grid[pos + Vector3Int.forward] == CellType.Room && IsRoom(pos + Vector3Int.forward + Vector3Int.down) && !IsRoom(pos + Vector3Int.down))
                PlaceRail(pos, Direction.North);
            else if (grid[pos + Vector3Int.forward] == CellType.RoomInPath && IsRoom(pos + Vector3Int.down) && IsRoom(pos + Vector3Int.down + Vector3Int.forward))
                PlaceRail(pos, Direction.North);
            else if (IsRoom(pos + Vector3Int.forward) && IsRoom(pos + Vector3Int.down) && !IsRoom(pos + Vector3Int.forward + Vector3Int.down))
                PlaceRail(pos, Direction.North);
        }
        if (IsWithinGridBounds(pos + Vector3Int.right) && IsWithinGridBounds(pos + Vector3Int.down))
        {
            if (grid[pos + Vector3Int.right] == CellType.Room && IsRoom(pos + Vector3Int.right + Vector3Int.down) && !IsRoom(pos + Vector3Int.down))
                PlaceRail(pos, Direction.East);
            else if (grid[pos + Vector3Int.right] == CellType.RoomInPath && IsRoom(pos + Vector3Int.down) && IsRoom(pos + Vector3Int.down + Vector3Int.right))
                PlaceRail(pos, Direction.East);
            else if (IsRoom(pos + Vector3Int.right) && IsRoom(pos + Vector3Int.down) && !IsRoom(pos + Vector3Int.right + Vector3Int.down))
                PlaceRail(pos, Direction.East);
        }

    }

    void HandleRoomInPath(Vector3Int pos)
    {
        Vector3Int northPos = pos + Vector3Int.forward;
        Vector3Int eastPos = pos + Vector3Int.right;

        //Floor
        PlaceFloor(pos);

        //Roof
        if (!IsWithinGridBounds(pos + Vector3Int.up)) PlaceFloor(pos + Vector3Int.up);
        else if (grid[pos + Vector3Int.up] != CellType.Room && grid[pos + Vector3Int.up] != CellType.RoomInPath) PlaceFloor(pos + Vector3Int.up);

        if (IsWithinGridBounds(northPos))
        {
            if (grid[northPos] != CellType.Room && grid[northPos] != CellType.Hallway && grid[northPos] != CellType.RoomInPath) PlaceWall(pos, Direction.North, wallPreFab);
            if (grid[northPos] == CellType.Hallway) PlaceWall(pos, Direction.North, doorWayPrefab);
        }
        else
        {
            PlaceWall(pos, Direction.North, wallPreFab);
        }

        if (IsWithinGridBounds(eastPos))
        {
            if (grid[eastPos] != CellType.Room && grid[eastPos] != CellType.Hallway && grid[eastPos] != CellType.RoomInPath) PlaceWall(pos, Direction.East, wallPreFab);
            if (grid[eastPos] == CellType.Hallway) PlaceWall(pos, Direction.East, doorWayPrefab);
        }
        else
        {
            PlaceWall(pos, Direction.East, wallPreFab);
        }

        //Pillar
        if ((!IsWithinGridBounds(northPos) || !IsWithinGridBounds(eastPos)))
        {
            PlacePillar(pos, Direction.North);
        }
        else
        {
            if ((grid[northPos] != CellType.Room && grid[northPos] != CellType.RoomInPath) || (grid[eastPos] != CellType.Room && grid[eastPos] != CellType.RoomInPath))
            {
                PlacePillar(pos, Direction.North);
            }
        }

        //Railing
        if (IsWithinGridBounds(pos + Vector3Int.forward) && IsWithinGridBounds(pos + Vector3Int.forward + Vector3Int.down))
        {
            if ((grid[pos + Vector3Int.forward] == CellType.Room) && IsRoom(pos + Vector3Int.forward + Vector3Int.down))
            {
                PlaceRail(pos, Direction.North);
            }
        }
        if (IsWithinGridBounds(pos + Vector3Int.right) && IsWithinGridBounds(pos + Vector3Int.right + Vector3Int.down))
        {
            if ((grid[pos + Vector3Int.right] == CellType.Room) && IsRoom(pos + Vector3Int.right + Vector3Int.down))
            {
                PlaceRail(pos, Direction.East);
            }
        }
    }

    void HandleStairs(Vector3Int pos)
    {
        Direction direction = GetStairDirection(pos);
        Vector3Int secondStair = pos;
        bool placeWallOne = false;

        switch (direction)
        {
            case Direction.North:
                secondStair = secondStair + Vector3Int.forward;
                if (IsWithinGridBounds(pos + Vector3Int.right) && GetStairDirection(pos + Vector3Int.right) != direction) placeWallOne = true;
                else if (!IsWithinGridBounds(pos + Vector3Int.right)) placeWallOne = true;
                break;
            case Direction.East:
                secondStair = secondStair + Vector3Int.right;
                if (IsWithinGridBounds(pos + Vector3Int.forward) && GetStairDirection(pos + Vector3Int.forward) != direction) placeWallOne = true;
                else if (!IsWithinGridBounds(pos + Vector3Int.forward)) placeWallOne = true;
                break;
            case Direction.South:
                secondStair = secondStair + Vector3Int.back;
                if (IsWithinGridBounds(pos + Vector3Int.right) && GetStairDirection(pos + Vector3Int.right) != direction) placeWallOne = true;
                else if (!IsWithinGridBounds(pos + Vector3Int.right)) placeWallOne = true;
                break;
            default:
                secondStair = secondStair + Vector3Int.left;
                if (IsWithinGridBounds(pos + Vector3Int.forward) && GetStairDirection(pos + Vector3Int.forward) != direction) placeWallOne = true;
                else if (!IsWithinGridBounds(pos + Vector3Int.forward)) placeWallOne = true;
                break;
        }

        //Floor
        PlaceFloor(pos);
        PlaceFloor(secondStair);

        //Roof
        PlaceFloor(pos + Vector3Int.up * 2);
        PlaceFloor(secondStair + Vector3Int.up * 2);

        //Place Stairs
        PlaceStairs(pos, direction);

        //Each position in the stairway
        Vector3Int[] locations = { pos, secondStair, pos + Vector3Int.up, secondStair + Vector3Int.up };

        foreach (Vector3Int location in locations)
        {
            //Walls
            if (direction == Direction.North || direction == Direction.South)
            {
                if (!IsWithinGridBounds(location + Vector3Int.right)) PlaceWall(location, Direction.East, wallPreFab);
                else if (placeWallOne) PlaceWall(location, Direction.East, wallPreFab);

                if (IsWithinGridBounds(location + Vector3Int.left) && grid[location + Vector3Int.left] == CellType.Hallway) PlaceWall(location, Direction.West, wallPreFab);
            }
            else
            {
                if (!IsWithinGridBounds(location + Vector3Int.forward)) PlaceWall(location, Direction.North, wallPreFab);
                else if (placeWallOne) PlaceWall(location, Direction.North, wallPreFab);

                if (IsWithinGridBounds(location + Vector3Int.back) && grid[location + Vector3Int.back] == CellType.Hallway) PlaceWall(location, Direction.South, wallPreFab);
            }

            //Pillar
            if (placeWallOne) PlacePillar(location, Direction.North);
            else if (direction == Direction.West) PlacePillar(locations[2], Direction.North);
        }

        //Handles Stair ends
        switch (direction)
        {
            case Direction.North:
                if (IsWithinGridBounds(locations[2] + Vector3Int.back) && grid[locations[2] + Vector3Int.back] == CellType.Hallway) PlaceWall(locations[2], Direction.South, wallPreFab);
                PlaceWall(locations[1], Direction.North, wallPreFab);
                break;
            case Direction.East:
                if (IsWithinGridBounds(locations[2] + Vector3Int.left) && grid[locations[2] + Vector3Int.left] == CellType.Hallway) PlaceWall(locations[2], Direction.West, wallPreFab);
                PlaceWall(locations[1], Direction.East, wallPreFab);
                break;
            case Direction.South:
                if (IsWithinGridBounds(locations[1] + Vector3Int.back) && grid[locations[1] + Vector3Int.back] == CellType.Hallway) PlaceWall(locations[1], Direction.South, wallPreFab);
                PlaceWall(locations[2], Direction.North, wallPreFab);
                break;
            default:
                if (IsWithinGridBounds(locations[1] + Vector3Int.left) && grid[locations[1] + Vector3Int.left] == CellType.Hallway) PlaceWall(locations[1], Direction.West, wallPreFab);
                PlaceWall(locations[2], Direction.East, wallPreFab);
                break;
        }
    }

    //Util Functions
    Direction GetStairDirection(Vector3Int pos)
    {
        foreach (Stairs stair in stairs)
        {
            if (stair.Location == pos)
            {
                return stair.StairDirection;
            }
        }
        return Direction.None;
    }

    bool IsRoom(Vector3Int pos)
    {
        return (grid[pos] == CellType.Room || grid[pos] == CellType.RoomInPath);
    }

    private bool IsWithinGridBounds(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < gridSize.x &&
               pos.y >= 0 && pos.y < gridSize.y &&
               pos.z >= 0 && pos.z < gridSize.z;
    }

    //Placement Functions
    void PlacePillar(Vector3Int pos, Direction direction)
    {
        Vector3 position = new Vector3((pos.x * 6.5f), (pos.y * 5f), (pos.z * 6.5f));

        GameObject go = Instantiate(wallPostFab, position, Quaternion.Euler(0, ((float)direction * 90), 0));
        go.GetComponent<Transform>().localScale = new Vector3(1.2f, 1.1f, 1.2f);
        Transform pillars = GeneratedDungeonParent.Find("Pillars");
        go.transform.SetParent(pillars);
    }

    void PlaceWall(Vector3Int pos, Direction direction, GameObject prefab)
    {
        Vector3 position = new Vector3((pos.x * 6.5f), (pos.y * 5f), (pos.z * 6.5f));

        GameObject go = Instantiate(prefab, position, Quaternion.Euler(0, ((float)direction * 90), 0));
        go.GetComponent<Transform>().localScale = new Vector3(1.2f, 1.1f, 1.2f);
        Transform walls = GeneratedDungeonParent.Find("Walls");
        go.transform.SetParent(walls);
    }

    void PlaceRail(Vector3Int pos, Direction direction)
    {
        Vector3 position = new Vector3((pos.x * 6.5f), (pos.y * 5f), (pos.z * 6.5f));

        GameObject go = Instantiate(railPrefab, position, Quaternion.Euler(0, ((float)direction * 90), 0));
        go.GetComponent<Transform>().localScale = new Vector3(1f, 0.8f, 1f);
        Transform rails = GeneratedDungeonParent.Find("Rails");
        go.transform.SetParent(rails);
    }

    private void PlaceFloor(Vector3Int pos)
    {
        Vector3 position = new Vector3((pos.x * 6.5f), (pos.y * 5f), (pos.z * 6.5f));
        GameObject go = Instantiate(floorPrefab, position, Quaternion.Euler(0, 0, 180));
        go.transform.localScale = new Vector3(1.35f, 0.5f, 1.35f);
        Transform floorsAndRoofs = GeneratedDungeonParent.Find("FloorsAndRoofs");
        go.transform.SetParent(floorsAndRoofs);
    }

    private void PlaceStairs(Vector3Int pos, Direction direction)
    {
        Vector3 position = new Vector3((pos.x * 6.5f), (pos.y * 5f) - 0.15f, (pos.z * 6.5f));

        GameObject go = Instantiate(stairPrefab, position, Quaternion.Euler(0, ((float)direction * 90), 0));
        go.GetComponent<Transform>().localScale = new Vector3(2.1f, 2.094f, 2.7f);
        Transform stairs = GeneratedDungeonParent.Find("Stairs");
        go.transform.SetParent(stairs);
    }

    void PlaceStart(Vector3Int pos)
    {
        Vector3 position = new Vector3((pos.x * 6.5f), (pos.y * 5f), (pos.z * 6.5f));

        GameObject go = Instantiate(startPrefab, position, Quaternion.identity);
        go.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
        Transform other = GeneratedDungeonParent.Find("Other");
        go.transform.SetParent(other);
    }

    void PlaceEnd(Vector3Int pos, Direction direction, int scale)
    {
        Vector3 position = new Vector3((pos.x * 6.5f) + 3.25f, (pos.y * 5f), (pos.z * 6.5f) + 3.25f);

        GameObject go = Instantiate(endPrefab, position, Quaternion.Euler(0, ((float)direction * 90), 0));
        go.GetComponent<Transform>().localScale = new Vector3(scale, 1f, 1f);
        Transform other = GeneratedDungeonParent.Find("Other");
        go.transform.SetParent(other);

    }

    void PlaceBossRoom(Vector3Int pos)
    {
        Vector3 position = new Vector3((pos.x * 6.5f), (pos.y * 5f), (pos.z * 6.5f));

        GameObject go = Instantiate(bossPrefab, position, Quaternion.identity);
        go.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
        Transform other = GeneratedDungeonParent.Find("Other");
        go.transform.SetParent(other);
    }
}
