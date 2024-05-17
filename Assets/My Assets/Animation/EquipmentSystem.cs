using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSystem : MonoBehaviour
{
    [SerializeField] GameObject weaponHolder;
    [SerializeField] GameObject weapon;
    [SerializeField] GameObject weaponSheath;
    [SerializeField] Player player;

    GameObject currentWeaponInHand;
    GameObject currentWeaponInSheath;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        currentWeaponInSheath = Instantiate(weapon, weaponSheath.transform);
    }

    public void Update()
    {
        if(currentWeaponInHand != null)
        {
            DamageDealer damageDealer = currentWeaponInHand.GetComponentInChildren<DamageDealer>();
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

        DamageDealer damageDealer = currentWeaponInHand.GetComponentInChildren<DamageDealer>();
        if(damageDealer != null && player != null)
        {
            damageDealer.weaponDamage = player.attack;
        }

        Destroy(currentWeaponInSheath);
    }

    public void SheathWeapon()
    {
        currentWeaponInSheath = Instantiate (weapon, weaponSheath.transform);
        Destroy(currentWeaponInHand);
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
