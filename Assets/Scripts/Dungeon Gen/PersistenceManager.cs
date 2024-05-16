using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static DungeonGenerator;
using static CameraStyleManager;
using static FurnitureSpawner;

public class PersistenceManager : MonoBehaviour
{
    [Serializable]
    public class WorldState
    {
        public int seed;
        public Grid3D<CellType> grid;
        public Vector3Int gridSize;
        public List<Stairs> stairs;
        public List<Room> rooms;
        public int bossLevel;
        public Vector3Int endRoomPos;
        public Vector3Int startRoomPos;
        public Vector3 playerPos;
        public Vector3 camPos;
        public Vector3 spawnLocation;
        public CameraStyle currentStyle;
        public List<Furniture> furnitureList;
    }

    [Serializable]
    public class CurrentLevel
    {
        public int currentLevel;
    }

    //Save Data
    public WorldState worldState;
    public CurrentLevel levelTracker;
    [SerializeField] GameObject player;
    [SerializeField] GameObject playerCamera;
    [SerializeField] CameraStyleManager cameraSwitcher;

    Controller controller;
    DungeonGenerator dungeonGenerator;
    DungeonSpawner dungeonSpawner;
    FurnitureSpawner furnitureSpawner;

    private void Start()
    {
        levelTracker = new CurrentLevel();
        levelTracker.currentLevel = -1;
        if (worldState == null) worldState = new WorldState();
        controller = gameObject.GetComponent<Controller>();
        dungeonSpawner = gameObject.GetComponent<DungeonSpawner>();
        dungeonGenerator = gameObject.GetComponent<DungeonGenerator>();
        furnitureSpawner = gameObject.GetComponent<FurnitureSpawner>();

        string directoryPath = Application.dataPath + "/Saves";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    public void SaveWorldState()
    {
        DownloadDependencies();
        SaveCurrentLevel();
        string json = JsonUtility.ToJson(worldState);
        string directoryPath = Application.dataPath + "/Saves";
        string filePath = directoryPath + "/" + levelTracker.currentLevel + "world_state.json";
        File.WriteAllText(filePath, json);
    }

    public bool LoadWorldState()
    {
        LoadCurrentLevel();
        Debug.Log(levelTracker.currentLevel);
        string directoryPath = Application.dataPath + "/Saves";
        string filePath = directoryPath + "/" + levelTracker.currentLevel + "world_state.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            worldState = JsonUtility.FromJson<WorldState>(json);
            SetPlayerToLastPos();
            UploadDependencies();

            return true;
        }
        return false;
    }

    public void DeleteWorldStates()
    {
        for (int currentLevel = 0; currentLevel <= worldState.bossLevel; currentLevel++)
        {
            string directoryPath = Application.dataPath + "/Saves";
            string filePath = directoryPath + "/" + currentLevel + "world_state.json";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

        }
        string levelTrackerPath = Application.dataPath + "/Saves" + "/level_tracker.json";
        if (File.Exists(levelTrackerPath))
        {
            File.Delete(levelTrackerPath);
        }

        string levelTrackerPathMeta = Application.dataPath + "/Saves" + "/level_tracker.json.meta";
        if (File.Exists(levelTrackerPathMeta))
        {
            File.Delete(levelTrackerPathMeta);
        }
    }

    private void UploadDependencies()
    {
        //Controller
        controller.randomSeed = worldState.seed;

        //DungeonGenerator
        dungeonGenerator.size = worldState.gridSize;
        dungeonGenerator.bossLevel = worldState.bossLevel;
        dungeonGenerator.grid = worldState.grid;
        dungeonGenerator.rooms = worldState.rooms;
        dungeonGenerator.stairs = worldState.stairs;
        dungeonGenerator.endRoomPos = worldState.endRoomPos;
        dungeonGenerator.startRoomPos = worldState.startRoomPos;
        dungeonGenerator.currentLevel = levelTracker.currentLevel;
        dungeonGenerator.seed = worldState.seed;

        //DungeonSpawner
        dungeonSpawner.gridSize = worldState.gridSize;
        dungeonSpawner.grid = worldState.grid;
        dungeonSpawner.stairs = worldState.stairs;
        dungeonSpawner.seed = worldState.seed;
        dungeonSpawner.currentLevel = levelTracker.currentLevel;
        dungeonSpawner.bossLevel = worldState.bossLevel;
        dungeonSpawner.spawnLocation = worldState.spawnLocation;

        furnitureSpawner.seed = worldState.seed;
        furnitureSpawner.grid = worldState.grid;
        furnitureSpawner.rooms = worldState.rooms;
        furnitureSpawner.gridSize = worldState.gridSize;
        furnitureSpawner.furnitureList = worldState.furnitureList;

        cameraSwitcher.currentStyle = worldState.currentStyle;
    }

    private void DownloadDependencies()
    {
        //Controller
        worldState.seed = controller.randomSeed;

        //DungeonGenerator
        worldState.grid = dungeonGenerator.grid;
        worldState.gridSize = dungeonGenerator.size;
        worldState.stairs = dungeonGenerator.stairs;
        worldState.rooms = dungeonGenerator.rooms;
        worldState.bossLevel = dungeonGenerator.bossLevel;
        worldState.endRoomPos = dungeonGenerator.endRoomPos;
        worldState.startRoomPos = dungeonGenerator.startRoomPos;

        //Other
        Rigidbody rb = player.transform.GetComponent<Rigidbody>();
        worldState.playerPos = rb.position;
        worldState.camPos = playerCamera.transform.position;
        worldState.spawnLocation = dungeonSpawner.spawnLocation;
        worldState.currentStyle = cameraSwitcher.currentStyle;
        worldState.furnitureList = furnitureSpawner.furnitureList;
    }

    public void UpdateWorldState()
    {
        DownloadDependencies();
        UploadDependencies();
    }

    public void SetPlayerToSpawn()
    {
        Rigidbody rb = player.transform.GetComponent<Rigidbody>();
        rb.position = worldState.spawnLocation;
        playerCamera.transform.position = worldState.spawnLocation;
    }

    public void SetPlayerToLastPos()
    {
        Rigidbody rb = player.transform.GetComponent<Rigidbody>();
        rb.position = worldState.playerPos;
        playerCamera.transform.position = worldState.camPos;
    }

    private void SaveCurrentLevel()
    {
        string json = JsonUtility.ToJson(levelTracker);
        string levelTrackerPath = Application.dataPath + "/Saves" + "/level_tracker.json";
        File.WriteAllText(levelTrackerPath, json);
    }

    private void LoadCurrentLevel()
    {
        string levelTrackerPath = Application.dataPath + "/Saves" + "/level_tracker.json";
        if (File.Exists(levelTrackerPath))
        {
            string json = File.ReadAllText(levelTrackerPath);
            levelTracker = JsonUtility.FromJson<CurrentLevel>(json);
        }
        else if (levelTracker.currentLevel == -1)
        {
            levelTracker.currentLevel = 0;
            SaveCurrentLevel();
        }
    }

    public void IncreaseCurrentLevel()
    {
        levelTracker.currentLevel++;
        SaveCurrentLevel();
    }

    public void DecreaseCurrentLevel()
    {
        if (levelTracker.currentLevel > 0)
        {
            levelTracker.currentLevel--;
            SaveCurrentLevel();
        }
    }

    public void ClearLoginStatus()
    {
        PlayerPrefs.DeleteKey("IsLoggedIn");
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        ClearLoginStatus();
    }

    private void OnDisable()
    {
        if (!Application.isPlaying)
        {
            ClearLoginStatus();
        }
    }
}
