using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage variables")]
    [SerializeField] public float weaponLength;
    [SerializeField] public float weaponDamage;
    public List<GameObject> hasDealtDamage { get; private set; }
    public bool canDealDamage;
    public bool IsPlayerWeapon;

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
        if(audio != null)
        {
            audioSource = audio.GetComponentInChildren<AudioSource>();
        }
    }

    private void Update()
    {
        //Checks if the weapon can deal damage 
        if (canDealDamage)
        {
            RaycastHit hit;
            int layerMask;

            //Determines if it deals damage to player or everyone
            if (IsPlayerWeapon)
            {
                layerMask = (1 << 9);
            }
            else
            {
                layerMask = (1 << 9) | (1 << 8);
            }

            //Finds any enemy's along the swords length
            if(weaponLength > 0)
            {
                if (Physics.Raycast(transform.position, -transform.up, out hit, weaponLength, layerMask))
                {
                    //Checks if we have already dealt damage to this object
                    if (!hasDealtDamage.Contains(hit.transform.gameObject))
                    {
                        IDamageable enemy = hit.transform.gameObject.GetComponentInChildren<IDamageable>();
                        //If the object has an enemy script we can invoke deal damage
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
                    //Checks if we have already dealt damage to this object
                    if (!hasDealtDamage.Contains(hit.transform.gameObject))
                    {
                        IDamageable enemy = hit.transform.gameObject.GetComponentInChildren<IDamageable>();
                        //If the object has an enemy script we can invoke deal damage
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
        //Switches between the two swing sound clips
        if(audioSource != null && clipOne != null && clipTwo != null)
        {
            if(clipTracker % 2 == 0)
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

        //Starts dealing damage and empties last swing
        canDealDamage = true;
        hasDealtDamage.Clear();
    }

    public void EndDealDamage()
    {
        //Stops Dealing damage
        canDealDamage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLength);
    }
}
