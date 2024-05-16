using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AnimationHandlerTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void AnimationHandlerTestSimplePasses()
    {
        // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator AnimationHandlerTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}

/*
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

public class AnimationHandlerTest
{
    private AnimationHandler animationHandler;

    [SetUp]
    public void Setup()
    {
        GameObject gameObject = new GameObject();
        animationHandler = gameObject.AddComponent<AnimationHandler>();

    }

    [Test]
    public void TestChangeVelocity()
    {
        //Set up test conditions
        //Call the method being tested
        // verify the expected behaviour with Assert.AreEqual(,);
    }

    [Test]
    public void TestLockResetVelocity()
    {

    }

    [Test]
    public void TestUpdate()
    {
        // Arrange: Set up test conditions
        // (Assume bool inputs are set correctly)

        // Act: Call the method being tested (Update)
        animationHandler.Update();

        // Assert: Verify the expected behavior
        // (Check if animator parameters are set correctly)
    }
}
 */
