using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
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
        damageDealerGameObject = new GameObject();
        damageDealer = damageDealerGameObject.AddComponent<DamageDealer>();

        audioGameObject = new GameObject();
        audioGameObject.AddComponent<AudioSource>();
        audioGameObject.tag = "Audio";

        damageDealer.clipOne = AudioClip.Create("ClipOne", 1, 1, 44100, false);
        damageDealer.clipTwo = AudioClip.Create("ClipTwo", 1, 1, 44100, false);
    }

    [Test]
    public void StartDealDamageTest()
    {

    }

    public void EndDealDamageTest()
    {
        damageDealer.EndDealDamage();

        //Assert.IsFalse(damageDealer.canDealDamage);
    }




        // A Test behaves as an ordinary method
        [Test]
    public void DamageDealerTestSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator DamageDealerTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
