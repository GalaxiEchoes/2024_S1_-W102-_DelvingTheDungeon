using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class Controller : MonoBehaviour
{
    public int randomSeed;

    PersistenceManager persistenceManager;
    DungeonSpawner dungeonSpawner;
    DungeonGenerator dungeonGenerator;
    FurnitureSpawner furnitureSpawner;
    Random rand;

    private void Start()
    {
        persistenceManager = gameObject.GetComponent<PersistenceManager>();
        dungeonSpawner = gameObject.GetComponent<DungeonSpawner>();
        dungeonGenerator = gameObject.GetComponent<DungeonGenerator>();
        furnitureSpawner = gameObject.GetComponent<FurnitureSpawner>();

        int seed = (int)DateTime.Now.Ticks;
        rand = new Random(seed);

        LoadGame();
    }

    public void LoadNextLevel()
    {
        SaveGame();
        persistenceManager.IncreaseCurrentLevel();

        int newSeed = UnityEngine.Random.Range(0, int.MaxValue);
        UnityEngine.Random.InitState(newSeed);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadPrevLevel()
    {
        SaveGame();
        persistenceManager.DecreaseCurrentLevel();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SaveGame()
    {
        persistenceManager.SaveWorldState();
    }

    public void LoadGame()
    {
        if (persistenceManager.LoadWorldState())
        {
            dungeonSpawner.SpawnDungeonRooms();
            furnitureSpawner.LoadFurniture();
        }
        else
        {
            //Generate new world
            randomSeed = rand.Next(0, int.MaxValue);
            Debug.Log("RandomSeed " + randomSeed);
            persistenceManager.UpdateWorldState();

            dungeonGenerator.GenerateDungeon();
            persistenceManager.UpdateWorldState();

            dungeonSpawner.SpawnDungeonRooms();
            persistenceManager.UpdateWorldState();

            furnitureSpawner.GenerateFurniture();
            persistenceManager.SetPlayerToSpawn();

            SaveGame();
        }
    }
}