using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyTest
{
    private GameObject enemyGameObject;
    private GameObject audioGameObject;
    private Enemy enemy;

    [SetUp]
    public void Setup()
    {
        //Setting Up Enemy
        enemyGameObject = new GameObject();
        enemy = enemyGameObject.AddComponent<Enemy>();
        enemyGameObject.layer = 9; //Enemy layer

        //Creating Audio GameObject/Source
        audioGameObject = new GameObject();
        audioGameObject.AddComponent<AudioSource>();
        audioGameObject.GetComponent<AudioSource>().enabled = true;
        audioGameObject.AddComponent<AudioListener>();
        audioGameObject.tag = "Audio";
    }

    [Test]
    public void StartTest()
    {
        //Testing the start function
        enemy.Start();

        Assert.IsNotNull(enemy.audioSource);
    }

    [Test]
    public void DamageTest()
    {
        //Test Setup
        enemy.Start();
        int damageAmount = 10;
        AudioClip hitClip = AudioClip.Create("HitClip", 1, 1, 44100, false);
        enemy.hitClip = hitClip;

        //Setting up assert for log
        LogAssert.Expect(LogType.Log, "Damage: " + damageAmount);

        enemy.Damage(damageAmount);

        //Testing to see if an audioclip is playing
        Assert.IsTrue(enemy.audioSource.isPlaying);
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(enemyGameObject);
        Object.Destroy(audioGameObject);
        Object.Destroy(enemy);
    }
}
