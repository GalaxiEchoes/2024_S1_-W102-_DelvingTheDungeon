using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerRewardsTest
{
    private GameObject playerGameObject;
    private Player player;

    [SetUp]
    public void Setup()
    {
        // Setting up Player
        playerGameObject = new GameObject();
        player = playerGameObject.AddComponent<Player>();
    }

    [Test]
    public void GainXPTest()
    {
        // Arrange
        int initialXP = player.xpManager.currentXP;
        int enemyXP = 20; // XP gained from defeating enemy

        // Act
        player.gainXP(enemyXP);

        // Assert
        Assert.AreEqual(initialXP + enemyXP, initialXP);
    }

    [Test]
    public void GainMoneyTest()
    {
        // Arrange
        int initialMoney = player.money;
        int enemyMoney = 50; // Money gained from defeating enemy

        // Act
        player.gainMoney(enemyMoney);

        // Assert
        Assert.AreEqual(initialMoney + enemyMoney, player.money);
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(playerGameObject);
    }
}
