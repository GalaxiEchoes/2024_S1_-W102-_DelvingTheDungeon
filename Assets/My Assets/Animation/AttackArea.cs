using System.Collections.Generic;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    public List<IDamageable> Damageables { get; } = new List<IDamageable>();

    public void OnTriggerEnter(Collider other)
    {
        var damageable = other.GetComponent<IDamageable>();
        if(damageable != null)
        {
            Damageables.Add(damageable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var damageable = other.GetComponent<IDamageable>();
        if(damageable != null && Damageables.Contains(damageable))
        {
            Damageables.Remove(damageable);
        }
    }
}
