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
        enemyGameObject = new GameObject();
        enemy = enemyGameObject.AddComponent<Enemy>();
        enemyGameObject.layer = 1 << 9;

        audioGameObject = new GameObject();
        audioGameObject.AddComponent<AudioSource>();
        audioGameObject.tag = "Audio";
    }

    [Test]
    public void StartTest()
    {
        enemy.Start();
        Assert.IsNotNull(enemy.audioSource);
    }

    [Test]
    public void DamageTest()
    {
        int damageAmount = 10;
        AudioClip hitClip = AudioClip.Create("HitClip", 1, 1, 44100, false);
        enemy.hitClip = hitClip;

        LogAssert.Expect(LogType.Log, "Damage: " + damageAmount);
        enemy.Damage(damageAmount);
        Assert.IsTrue(enemy.audioSource.isPlaying);
    }
}
