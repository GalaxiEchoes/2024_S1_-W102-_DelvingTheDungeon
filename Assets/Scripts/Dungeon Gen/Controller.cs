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

        Debug.Log("At Start");
        LoadGame();
    }

    public void LoadNextLevel()
    {
        SaveGame();
        persistenceManager.IncreaseCurrentLevel();

        Debug.Log("Load Next Level");

        int newSeed = UnityEngine.Random.Range(0, int.MaxValue);
        UnityEngine.Random.InitState(newSeed);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadPrevLevel()
    {
        SaveGame();
        persistenceManager.DecreaseCurrentLevel();

        Debug.Log("Load Prev Level");
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
            Debug.Log("Now I should end up here!");
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
            Debug.Log("I shouldnt end up here!");
        }
    }

    public void DeleteGame()
    {
        persistenceManager.DeleteWorldStates();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}