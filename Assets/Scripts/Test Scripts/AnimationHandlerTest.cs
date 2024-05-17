using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AnimationHandlerTest
{
    private GameObject handlerObject;
    private AnimationHandler handler;
    private Animator animator;
    private InputManager inputManager;

    [SetUp]
    public void SetUp()
    {
        /*
         isDrawn = false;
        readyToJump = true;
        animator = Player.GetComponentInChildren<Animator>();
        PlayerMovement = Player.GetComponent<InputSystemPlayerMovement>();
        prevHeight = Player.transform.position.y;
         */
        handlerObject = new GameObject();
        handler = handlerObject.AddComponent<AnimationHandler>();
        animator = handlerObject.AddComponent<Animator>();
        inputManager = new InputManager();

    }

    [Test]
    public void StartTest()
    {

    }

    [Test]
    public void UpdateTest()
    {

    }

    [Test]
    public void HandleAttackExitTest()
    {

    }

    [TearDown]
    public void TearDown()
    {

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
