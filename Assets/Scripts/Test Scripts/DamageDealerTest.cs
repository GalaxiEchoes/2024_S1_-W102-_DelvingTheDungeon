using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class DamageDealerTest
{
    private GameObject damageDealerGameObject;
    private DamageDealer damageDealer;
    private GameObject audioGameObject;

    [SetUp]
    public void Setup()
    {
        //Setup Damage Dealer GameObject
        damageDealerGameObject = new GameObject();
        damageDealer = damageDealerGameObject.AddComponent<DamageDealer>();
        damageDealer.clipOne = AudioClip.Create("ClipOne", 1, 1, 44100, false);
        damageDealer.clipTwo = AudioClip.Create("ClipTwo", 1, 1, 44100, false);

        //Setting Up Audio GameObject
        audioGameObject = new GameObject();
        audioGameObject.AddComponent<AudioSource>();
        audioGameObject.AddComponent<AudioListener>();
        audioGameObject.tag = "Audio";
    }

    [Test]
    public void StartTest()
    {
        //Running Start
        damageDealer.Start();

        //Testing if it has set/found the proper states
        Assert.IsFalse(damageDealer.canDealDamage);
        Assert.IsNotNull(damageDealer.hasDealtDamage);
        Assert.IsNotNull(damageDealer.audioSource);
    }

    [Test]
    public void StartDealDamageTest()
    {
        //Setup for test
        damageDealer.Start();
        damageDealer.audioSource.enabled = true;
        int initialClipTracker = damageDealer.clipTracker;

        damageDealer.StartDealDamage();
        
        //Checking if the correct clips are playing and if the dealdamage state set
        Assert.IsTrue(damageDealer.audioSource.isPlaying);
        if (initialClipTracker % 2 == 0)
        {
            Assert.AreEqual(damageDealer.clipOne, damageDealer.audioSource.clip);
        }
        else
        {
            Assert.AreEqual(damageDealer.clipTwo, damageDealer.audioSource.clip);
        }

        Assert.IsTrue(damageDealer.canDealDamage);
    }

    [Test]
    public void EndDealDamageTest()
    {
        //Setup for test
        damageDealer.Start();

        damageDealer.EndDealDamage();

        //Check if it correctly reset state
        Assert.IsFalse(damageDealer.canDealDamage);
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(damageDealer);
        Object.Destroy(damageDealerGameObject);
        Object.Destroy(audioGameObject);
    }
}
