using NUnit.Framework;
using UnityEngine;

public class XPManagerTests
{
    private XPManager xpManager;
    private int currentLevel;
    private int totalXP;
    private int prevLevelXP;
    private int nextLevelXP;
    [SerializeField] AnimationCurve XPCurve;


    [SetUp]
    public void SetUp()
    {
        GameObject xpManagerObject = new GameObject("XPManager");
        xpManager = xpManagerObject.AddComponent<XPManager>();
    }

    [Test]
    public void TestAddXP(int amount)
    {
        totalXP += amount;
        TestCheckForLevelUp();
    }

    [Test]
    public void TestCheckForLevelUp()
    {
        if (totalXP >= nextLevelXP)
        {
            currentLevel++;
            TestUpdateLevel();
        }
    }

    [Test]
    public void TestUpdateLevel()
    {
        prevLevelXP = (int)XPCurve.Evaluate(currentLevel);
        nextLevelXP = (int)XPCurve.Evaluate(currentLevel + 1);
    }
}
