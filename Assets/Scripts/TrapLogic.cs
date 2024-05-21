using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapLogic : MonoBehaviour
{
    public bool isActive;
    [SerializeField] GameObject trapWeapon;
    Animator animator;
    DamageDealer damageDealer;

    // Start is called before the first frame update
    void Start()
    {
        animator = trapWeapon.GetComponentInChildren<Animator>();
        damageDealer = trapWeapon.GetComponentInChildren<DamageDealer>();
        isActive = true;
        animator.SetBool("IsActive", isActive);
    }

    public void Update()
    {
        if (isActive && !animator.GetBool("IsActive"))
        {
            ArmTrap();
        }
        else if (!isActive && animator.GetBool("IsActive"))
        {
            DisarmTrap();
        }
    }

    public void DisarmTrap()
    {
        animator.SetBool("IsActive", isActive);
        animator.SetTrigger("DisarmTrap");
    }

    public void ArmTrap()
    {
        animator.SetBool("IsActive", isActive);
        animator.SetTrigger("ArmTrap");
    }

    private void StartDealDamage()
    {
        damageDealer.StartDealDamage();
    }

    private void EndDealDamage()
    {
        damageDealer.EndDealDamage();
    }
}
