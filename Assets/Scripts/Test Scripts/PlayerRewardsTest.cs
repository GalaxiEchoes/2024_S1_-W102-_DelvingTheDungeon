using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerRewardsTest
{
    private GameObject playerGameObject;
    private Player player;
    private XPManager xpManager;

    [SetUp]
    public void Setup()
    {
        // Setting up Player
        playerGameObject = new GameObject();
        player = playerGameObject.AddComponent<Player>();
        xpManager = playerGameObject.AddComponent<XPManager>();
    }

    [Test]
    public void GainXPTest()
    {
        // Arrange
        int initialXP = xpManager.totalXP;
        int enemyXP = 20; // XP gained from defeating enemy

        // Act
        player.gainXP(enemyXP);

        // Assert
        Assert.AreEqual(initialXP + enemyXP, xpManager.totalXP + enemyXP);
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
