using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPManagerTests
{
    private XPManager xpManager;
    AnimationCurve XPCurve;
    private TextMeshProUGUI levelText;
    private Image XPFill;


    [SetUp]
    public void SetUp()
    {
        GameObject xpManagerObject = new GameObject("XPManager");
        xpManager = xpManagerObject.AddComponent<XPManager>();

        XPCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 100));
        xpManager.XPCurve = XPCurve;

        xpManager.currentLevel = 0;

        // Create mock TextMeshProUGUI and Image objects
        GameObject textObject = new GameObject("LevelText");
        levelText = textObject.AddComponent<TextMeshProUGUI>();
        xpManager.levelText = levelText;

        GameObject imageObject = new GameObject("XPFill");
        XPFill = imageObject.AddComponent<Image>();
        xpManager.XPFill = XPFill;
    }

    [Test]
    public void TestAddXP()
    {
        // Arrange
        int initialTotalXP = xpManager.totalXP;
        int amount = 10;

        // Act
        xpManager.addXP(amount);

        // Assert
        Assert.AreEqual(initialTotalXP + amount, xpManager.totalXP);
    }

    [Test]
    public void TestCheckForLevelUp()
    {
        // Arrange
        xpManager.totalXP = 150;
        xpManager.nextLevelXP = 100;

        // Act
        xpManager.checkForLevelUp();

        // Assert
        Assert.AreEqual(1, xpManager.currentLevel);
    }

    [Test]
    public void TestUpdateLevel()
    {
        // Arrange
        xpManager.currentLevel = 0;

        // Act
        xpManager.updateLevel();

        // Assert
        Assert.AreEqual(0, xpManager.prevLevelXP);
        Assert.AreEqual(100, xpManager.nextLevelXP);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up created GameObjects
        GameObject.Destroy(xpManager.gameObject);
        GameObject.Destroy(levelText.gameObject);
        GameObject.Destroy(XPFill.gameObject);
    }
}
