using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EquipmentSystemTest
{
    GameObject equipmentSystemObject;
    EquipmentSystem equipmentSystem;
    Vector3 weaponHolderPos;
    GameObject weapon;
    Vector3 weaponSheathPos;
    GameObject playerObject;

    [SetUp]
    public void Setup()
    {
        //Equipment System Setup
        equipmentSystemObject = new GameObject();
        equipmentSystem = equipmentSystemObject.AddComponent<EquipmentSystem>();
        equipmentSystem.weaponHolder = new GameObject();
        equipmentSystem.weaponHolder.transform.localPosition = Vector3.one;
        weaponHolderPos = Vector3.one;
        weapon = new GameObject();
        equipmentSystem.weapon = weapon;
        equipmentSystem.weaponSheath = new GameObject();
        equipmentSystem.weaponSheath.transform.localPosition = Vector3.one * 2;
        weaponSheathPos = Vector3.one * 2;

        //Player Setup
        playerObject = new GameObject();
        playerObject.AddComponent<Player>();
        playerObject.tag = "Player";
        playerObject.GetComponent<Player>().attack = 50;
    }

    [Test]
    public void StartTest()
    {
        equipmentSystem.Start();

        Assert.IsNotNull(equipmentSystem.player);
        Assert.IsNotNull(equipmentSystem.currentWeaponInSheath);
    }

    [Test]
    public void UpdateTest()
    {
        //Setup
        equipmentSystem.Start();
        DamageDealer damageDealer = equipmentSystem.weapon.AddComponent<DamageDealer>();
        equipmentSystem.DrawWeapon();

        equipmentSystem.Update();

        //Test to see if correct members are not null and attack was assigned
        Assert.IsNotNull(equipmentSystem.player);
        Assert.IsNotNull(equipmentSystem.weapon);
        Assert.IsNotNull(equipmentSystem.currentWeaponInHand);
        damageDealer = equipmentSystem.currentWeaponInHand.GetComponent<DamageDealer>();
        Assert.IsNotNull(damageDealer);
        Assert.AreEqual(equipmentSystem.player.attack, damageDealer.weaponDamage);
    }

    [UnityTest]
    public IEnumerator DrawWeaponTestNoDamageDealer()
    {
        //Setup
        equipmentSystem.Start();

        equipmentSystem.DrawWeapon();
        yield return null;

        //Testing for correct values
        Assert.IsNotNull(equipmentSystem.currentWeaponInHand);
        Assert.AreEqual(weaponHolderPos, equipmentSystem.currentWeaponInHand.GetComponentInParent<Transform>().position);
        Assert.That(equipmentSystem.currentWeaponInSheath == null || equipmentSystem.currentWeaponInSheath.ToString() == "<null>");
    }

    [UnityTest]
    public IEnumerator DrawWeaponTestWithDamageDealer()
    {
        //Setup
        equipmentSystem.Start();
        equipmentSystem.weapon.AddComponent<DamageDealer>();

        equipmentSystem.DrawWeapon();
        yield return null;

        //Testing for correct values
        DamageDealer damageDealer = equipmentSystem.currentWeaponInHand.GetComponent<DamageDealer>();
        Assert.AreEqual(equipmentSystem.player.attack, damageDealer.weaponDamage);
        Assert.IsNotNull(equipmentSystem.currentWeaponInHand);
        Assert.AreEqual(weaponHolderPos, equipmentSystem.currentWeaponInHand.GetComponentInParent<Transform>().position);
        Assert.That(equipmentSystem.currentWeaponInSheath == null || equipmentSystem.currentWeaponInSheath.ToString() == "<null>");
    }

    [UnityTest]
    public IEnumerator SheathWeaponTest()
    {
        //Setup
        equipmentSystem.Start();
        equipmentSystem.DrawWeapon();
        yield return null;

        equipmentSystem.SheathWeapon();
        yield return null;

        //Testing for correct values
        Assert.IsNotNull(equipmentSystem.currentWeaponInSheath);
        Assert.AreEqual(weaponSheathPos, equipmentSystem.currentWeaponInSheath.transform.position);
        Assert.That(equipmentSystem.currentWeaponInHand == null || equipmentSystem.currentWeaponInHand.ToString() == "<null>");
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(equipmentSystem);
        Object.Destroy(playerObject);
        Object.Destroy(weapon);
        Object.Destroy(equipmentSystemObject);
    }
}
