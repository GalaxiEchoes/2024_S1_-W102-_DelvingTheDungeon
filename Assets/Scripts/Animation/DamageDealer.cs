using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageDealer : MonoBehaviour
{
    public enum WeaponHolder
    {
        PlayerWeapon,
        EnemyWeapon,
        TrapWeapon
    }

    [Header("Damage variables")]
    [SerializeField] public float weaponLength;
    [SerializeField] public float weaponDamage;
    [SerializeField] private Slider enemyHealthSlider;
    public List<GameObject> hasDealtDamage { get; private set; }
    public bool canDealDamage;
    public WeaponHolder weaponHolder = WeaponHolder.PlayerWeapon;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clipOne;
    public AudioClip clipTwo;
    public int clipTracker { get; private set; }

    public void Start()
    {
        canDealDamage = false;
        hasDealtDamage = new List<GameObject>();
        GameObject audio = GameObject.FindGameObjectWithTag("Audio");
        if (audio != null)
        {
            audioSource = audio.GetComponentInChildren<AudioSource>();
        }
    }

    private void Update()
    {
        // Checks if the weapon can deal damage
        if (canDealDamage)
        {
            RaycastHit hit;
            int layerMask = 0;

            //Determines if it deals damage to player or everyone
            switch (weaponHolder)
            {
                case WeaponHolder.PlayerWeapon: //Only hurts Enemys
                    layerMask = (1 << 9);
                    break;
                case WeaponHolder.EnemyWeapon: //Only hurts Players
                    layerMask = (1 << 8);
                    break;
                case WeaponHolder.TrapWeapon: //Hurts everything
                    layerMask = (1 << 9) | (1 << 8);
                    break;
            }

            // Finds any enemies along the weapon's length
            if (weaponLength > 0)
            {
                if (Physics.Raycast(transform.position, -transform.up, out hit, weaponLength, layerMask))
                {
                    // Checks if we have already dealt damage to this object
                    if (!hasDealtDamage.Contains(hit.transform.gameObject))
                    {
                        IDamageable enemy = hit.transform.gameObject.GetComponentInChildren<IDamageable>();
                        // If the object has an enemy script we can invoke deal damage
                        if (enemy != null)
                        {
                            enemy.Damage((int)weaponDamage);
                        }

                        hasDealtDamage.Add(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if (Physics.Raycast(transform.position, transform.up, out hit, Mathf.Abs(weaponLength), layerMask))
                {
                    // Checks if we have already dealt damage to this object
                    if (!hasDealtDamage.Contains(hit.transform.gameObject))
                    {
                        IDamageable enemy = hit.transform.gameObject.GetComponentInChildren<IDamageable>();
                        // If the object has an enemy script we can invoke deal damage
                        if (enemy != null)
                        {
                            enemy.Damage((int)weaponDamage);
                        }

                        hasDealtDamage.Add(hit.transform.gameObject);
                    }
                }
            }
        }
    }

    public void StartDealDamage()
    {
        // Switches between the two swing sound clips
        if (audioSource != null && clipOne != null && clipTwo != null)
        {
            if (clipTracker % 2 == 0)
            {
                clipTracker = 0;
                audioSource.clip = clipOne;
                audioSource.PlayOneShot(clipOne);
            }
            else
            {
                audioSource.clip = clipTwo;
                audioSource.PlayOneShot(clipTwo);
            }
            clipTracker++;
        }

        // Starts dealing damage and empties last swing
        canDealDamage = true;
        hasDealtDamage.Clear();
    }

    public void EndDealDamage()
    {
        // Stops dealing damage
        canDealDamage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLength);
    }
}