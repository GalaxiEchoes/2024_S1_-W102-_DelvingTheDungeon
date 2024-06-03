using NUnit.Framework;
using UnityEngine;

public class PlayerMoneyTest
{
    private Player player;

    [SetUp]
    public void SetUp()
    {
        // Create a new instance of the Player class for each test case
        GameObject playerObject = new GameObject("Player");
        player = playerObject.AddComponent<Player>();
    }

    [Test]
    [TestCase(50)]
    [TestCase(10)]
    [TestCase(0)]
    [TestCase(20)]
    public void AddPositiveMoneyTest(int amountToAdd)
    {
        int initialMoney = player.money;

        player.addMoney(amountToAdd);

        // Check player money reflects added money
        Assert.AreEqual(initialMoney + amountToAdd, player.money);
    }

    [Test]
    [TestCase(50)]
    [TestCase(10)]
    [TestCase(0)]
    [TestCase(20)]
    public void MinusPositiveMoneyTest(int amountToRemove)
    {
        int initialMoney = player.money;

        player.minusMoney(amountToRemove);

        // Check player money reflects removed money
        Assert.AreEqual(initialMoney - amountToRemove, player.money);
    }

    [Test]
    [TestCase(-50)]
    [TestCase(-10)]
    [TestCase(-20)]
    public void AddNegativeMoneyTest(int amountToAdd)
    {
        int initialMoney = player.money;

        player.addMoney(amountToAdd);

        // Check player money remained the same
        Assert.AreEqual(initialMoney, player.money);
    }

    [Test]
    [TestCase(-50)]
    [TestCase(-10)]
    [TestCase(-20)]
    public void MinusNegativeMoneyTest(int amountToRemove)
    {
        int initialMoney = player.money;

        player.minusMoney(amountToRemove);

        // Check player money remained the same
        Assert.AreEqual(initialMoney, player.money);
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(player.gameObject);
    }
}
