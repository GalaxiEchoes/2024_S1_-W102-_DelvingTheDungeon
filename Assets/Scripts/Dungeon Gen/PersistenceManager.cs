using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static DungeonGenerator;
using static CameraStyleManager;
using static Cinemachine.DocumentationSortingAttribute;
using static PersistenceManager;
using static FurnitureSpawner;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine.SceneManagement;

public class PersistenceManager : MonoBehaviour
{
    /*
     * This includes everything that needs to be tracked on a single dungeon level.
     * Manually add here and then update in the download dependencies/ upload dependencies functions.
     */
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

        //Enemies spawned
        //interactables + states??
        //Shop per level
    }

    /*
     * This includes everything that persists between levels but gets deleted on death
     * Manually add here and then update in the download dependencies/ upload dependencies functions.
     */
    [Serializable]
    public class GameData
    {
        public int currentLevel;
        public InventorySystem InventorySystem;
        public EquipmentInventorySystem EquipmentInventorySystem;
        public int maxLevelReached;
    }

    /*
     * All things that persist between runs need to be manually added here
     * and then updated in the download dependencies/ upload dependencies functions.
    */
    [Serializable]
    public class PermanentGameData
    {
        public int currentLevel;
        public int totalXP;
        public int prevLevelXP;
        public int nextLevelXP;
        public int money;
        public int health;
        public float stamina;
        public int attack;
        public int defense;
        public int maxHealth;
        public float maxStamina;
    }

    //Save Data
    public WorldState worldState;
    public GameData gameData;
    public PermanentGameData permanentGameData;

    //Objects to retrieve/set data
    [SerializeField] GameObject playerGameObject;
    [SerializeField] GameObject playerCamera;
    [SerializeField] CameraStyleManager cameraSwitcher;

    Controller controller;
    DungeonGenerator dungeonGenerator;
    DungeonSpawner dungeonSpawner;
    FurnitureSpawner furnitureSpawner;

    private void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name.CompareTo("DelvingTheDungeon") == 0)
        {
            permanentGameData = new PermanentGameData();
            gameData = new GameData();
            gameData.currentLevel = -1;
            gameData.InventorySystem = new InventorySystem(39);
            gameData.EquipmentInventorySystem = new EquipmentInventorySystem(4);
            gameData.maxLevelReached = -1;

            if (worldState == null) worldState = new WorldState();
            controller = gameObject.GetComponent<Controller>();
            dungeonSpawner = gameObject.GetComponent<DungeonSpawner>();
            dungeonGenerator = gameObject.GetComponent<DungeonGenerator>();
            furnitureSpawner = gameObject.GetComponent<FurnitureSpawner>();

            //Creates Save folder if it doesn't exist
            string directoryPath = Application.dataPath + "/Saves";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

    }

    public void SaveWorldState()
    {
        DownloadDependencies();
        string json = JsonUtility.ToJson(worldState);
        string directoryPath = Application.dataPath + "/Saves";
        string filePath = directoryPath + "/" + gameData.currentLevel + "world_state.json";
        File.WriteAllText(filePath, json);
    }

    public bool LoadWorldState()
    {
        LoadCurrentLevel();

        string directoryPath = Application.dataPath + "/Saves";
        string filePath = directoryPath + "/" + gameData.currentLevel + "world_state.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            worldState = JsonUtility.FromJson<WorldState>(json);
            SetPlayerToLastPos();
            LoadCurrentStats();
            UploadDependencies();

            return true;
        }
        else
        {
            LoadCurrentStats();
        }
        return false;
    }

    //Deletes all data
    public void StartNewGame()
    {
        //Wipes Save folder squeaky clean
        string dirPath = Application.dataPath + "/Saves";
        DirectoryInfo directory = new DirectoryInfo(dirPath);

        foreach (FileInfo file in directory.GetFiles())
        {
            file.Delete();
        }
    }

    //Deletes world and level persistent data but not permanant data
    public void StartNewRun()
    {
        string dirPath = Application.dataPath + "/Saves";
        string worldStateData = "world_State.json";
        string gameData = "game_data";

        //Finds world data to delete
        DirectoryInfo directory = new DirectoryInfo(dirPath);

        FileInfo[] filesToDelete = directory.GetFiles()
            .Where(file => file.Name.Contains(worldStateData))
            .ToArray();

        foreach (FileInfo file in filesToDelete)
        {
            file.Delete();
        }

        //Finds game data to delete
        filesToDelete = directory.GetFiles()
            .Where(file => file.Name.Contains(gameData))
            .ToArray();

        foreach (FileInfo file in filesToDelete)
        {
            file.Delete();
        }
    }

    private void UploadDependencies()
    {
        LoadCurrentLevel();
        LoadCurrentStats();

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
        dungeonGenerator.currentLevel = gameData.currentLevel;
        dungeonGenerator.seed = worldState.seed;

        //DungeonSpawner
        dungeonSpawner.gridSize = worldState.gridSize;
        dungeonSpawner.grid = worldState.grid;
        dungeonSpawner.stairs = worldState.stairs;
        dungeonSpawner.seed = worldState.seed;
        dungeonSpawner.currentLevel = gameData.currentLevel;
        dungeonSpawner.bossLevel = worldState.bossLevel;
        dungeonSpawner.spawnLocation = worldState.spawnLocation;

        //Furniture Spawner
        furnitureSpawner.seed = worldState.seed;
        furnitureSpawner.grid = worldState.grid;
        furnitureSpawner.rooms = worldState.rooms;
        furnitureSpawner.gridSize = worldState.gridSize;
        furnitureSpawner.furnitureList = worldState.furnitureList;

        cameraSwitcher.currentStyle = worldState.currentStyle;

        //Player Inventory
        InventoryHolder target = playerGameObject.GetComponent<InventoryHolder>();

        if (target != null)
        {
            target.EquipmentInventorySystem = gameData.EquipmentInventorySystem;
            target.InventorySystem = gameData.InventorySystem;
        }
    }

    private void DownloadDependencies()
    {
        SaveCurrentLevel();
        SaveCurrentStats();

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
        Rigidbody rb = playerGameObject.transform.GetComponent<Rigidbody>();
        worldState.playerPos = rb.position;
        worldState.camPos = playerCamera.transform.position;
        worldState.spawnLocation = dungeonSpawner.spawnLocation;
        worldState.currentStyle = cameraSwitcher.currentStyle;
        furnitureSpawner.SaveFurniture();
        worldState.furnitureList = furnitureSpawner.furnitureList;

        //PlayerInventory
        InventoryHolder current = playerGameObject.GetComponent<InventoryHolder>();
        gameData.InventorySystem = current.InventorySystem;
        gameData.EquipmentInventorySystem = current.EquipmentInventorySystem;
    }

    public void UpdateWorldState()
    {
        DownloadDependencies();
        UploadDependencies();
    }

    public void SetPlayerToSpawn()
    {
        Rigidbody rb = playerGameObject.transform.GetComponent<Rigidbody>();
        rb.position = worldState.spawnLocation;
        playerCamera.transform.position = worldState.spawnLocation;
    }

    public void SetPlayerToLastPos()
    {
        Rigidbody rb = playerGameObject.transform.GetComponent<Rigidbody>();
        rb.position = worldState.playerPos;
        playerCamera.transform.position = worldState.camPos;
    }

    private void SaveCurrentLevel()
    {
        string json = JsonUtility.ToJson(gameData);
        string gameDataPath = Application.dataPath + "/Saves" + "/game_data.json";
        File.WriteAllText(gameDataPath, json);
    }

    private void LoadCurrentLevel()
    {
        string gameDataPath = Application.dataPath + "/Saves" + "/game_data.json";
        if (File.Exists(gameDataPath))
        {
            string json = File.ReadAllText(gameDataPath);
            gameData = JsonUtility.FromJson<GameData>(json);
        }
        else if (gameData.currentLevel == -1)
        {
            gameData.currentLevel = 0;
            gameData.maxLevelReached = 0;
            SaveCurrentLevel();
        }
    }

    private void SaveCurrentStats()
    {
        //Player
        Player current = playerGameObject.GetComponent<Player>();
        permanentGameData.currentLevel = current.xpManager.currentLevel;
        permanentGameData.totalXP = current.xpManager.totalXP;
        permanentGameData.prevLevelXP = current.xpManager.prevLevelXP;
        permanentGameData.nextLevelXP = current.xpManager.nextLevelXP;
        permanentGameData.money = current.money;
        permanentGameData.health = current.health;
        permanentGameData.stamina = current.stamina;
        permanentGameData.attack = current.attack;
        permanentGameData.defense = current.defense;
        permanentGameData.maxHealth = current.maxHealth;
        permanentGameData.maxStamina = current.maxStamina;

        string directoryPath = Application.dataPath + "/Saves";
        string dataJson = JsonUtility.ToJson(permanentGameData);
        string filePath = directoryPath + "/permanent_data";
        File.WriteAllText(filePath, dataJson);
    }

    private void LoadCurrentStats()
    {
        string directoryPath = Application.dataPath + "/Saves";
        string filePath = directoryPath + "/permanent_data";

        if (File.Exists(filePath))
        {
            String json = File.ReadAllText(filePath);
            permanentGameData = JsonUtility.FromJson<PermanentGameData>(json);

            //Player Stats
            Player original = playerGameObject.GetComponent<Player>();

            original.xpManager.currentLevel = permanentGameData.currentLevel;
            original.xpManager.totalXP = permanentGameData.totalXP;
            original.xpManager.prevLevelXP = permanentGameData.prevLevelXP;
            original.xpManager.nextLevelXP = permanentGameData.nextLevelXP;
            original.money = permanentGameData.money;
            original.health = permanentGameData.health;
            original.stamina = permanentGameData.stamina;
            original.attack = permanentGameData.attack;
            original.defense = permanentGameData.defense;
            original.maxHealth = permanentGameData.maxHealth;
            original.maxStamina = permanentGameData.maxStamina;

            if (original.stamina < original.maxStamina)
            {
                InputSystemPlayerMovement movement = playerGameObject.GetComponent<InputSystemPlayerMovement>();
                if (movement != null)
                {
                    movement.recharge = StartCoroutine(movement.RechargeStamina());
                }
            }
        }
    }


    public void IncreaseCurrentLevel()
    {
        gameData.currentLevel++;
        if (gameData.currentLevel > gameData.maxLevelReached)
        {
            gameData.maxLevelReached = gameData.currentLevel;
            // Increase player money
            Player player = playerGameObject.GetComponent<Player>();
            player.addMoney(25);
            SaveCurrentStats(); // Save the updated player stats
        }
        SaveCurrentLevel();
    }

    public void DecreaseCurrentLevel()
    {
        if (gameData.currentLevel > 0)
        {
            gameData.currentLevel--;
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