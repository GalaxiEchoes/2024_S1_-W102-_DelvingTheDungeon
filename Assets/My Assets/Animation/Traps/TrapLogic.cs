using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapLogic : MonoBehaviour
{
    bool isActive;
    Animator animator;
    DamageDealer damageDealer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        damageDealer = GetComponent<DamageDealer>();
        isActive = true;
        animator.SetBool("IsActive", isActive);
    }

    public void DisarmTrap()
    {
        isActive = false;
        animator.SetBool("IsActive", isActive);
        animator.SetTrigger("DisarmTrap");
    }

    public void ArmTrap()
    {
        isActive = true;
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
