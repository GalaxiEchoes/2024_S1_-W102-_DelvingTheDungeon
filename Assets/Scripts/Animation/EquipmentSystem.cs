using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    public GameObject weaponHolder;
    public GameObject weapon;
    public GameObject weaponSheath;
    public Player player { get; private set; }

    //Weapon positions
    public GameObject currentWeaponInHand { get; private set; }
    public GameObject currentWeaponInSheath { get; private set; }

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        currentWeaponInSheath = Instantiate(weapon, weaponSheath.transform);
    }

    public void Update()
    {
        if(currentWeaponInHand != null)
        {
            DamageDealer damageDealer = currentWeaponInHand.GetComponent<DamageDealer>();
            if(damageDealer == null)
            {
                damageDealer = currentWeaponInHand.GetComponentInChildren<DamageDealer>();
            }

            if (damageDealer != null && player != null)
            {
               if( damageDealer.weaponDamage != player.attack)
               {
                    damageDealer.weaponDamage = player.attack;
               }
            }
        }
    }

    public void DrawWeapon()
    {
        currentWeaponInHand = Instantiate(weapon, weaponHolder.transform);
        Destroy(currentWeaponInSheath.gameObject);

        DamageDealer damageDealer = currentWeaponInHand.GetComponent<DamageDealer>();
        if (damageDealer == null)
        {
            damageDealer = currentWeaponInHand.GetComponentInChildren<DamageDealer>();
        }

        if (damageDealer != null && player != null)
        {
            damageDealer.weaponDamage = player.attack;
        }
    }

    public void SheathWeapon()
    {
        currentWeaponInSheath = Instantiate (weapon, weaponSheath.transform);
        Destroy(currentWeaponInHand.gameObject);
    }

    public void StartDealDamage()
    {
        currentWeaponInHand.GetComponentInChildren<DamageDealer>().StartDealDamage();
    }

    public void EndDealDamage()
    {
        currentWeaponInHand.GetComponentInChildren<DamageDealer>().EndDealDamage();
    }
}
