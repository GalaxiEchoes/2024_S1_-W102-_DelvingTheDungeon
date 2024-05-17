using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage variables")]
    bool canDealDamage;
    List<GameObject> hasDealtDamage;
    [SerializeField] float weaponLength;
    [SerializeField] public float weaponDamage;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clipOne;
    public AudioClip clipTwo;
    int clipTracker;

    void Start()
    {
        canDealDamage = false;
        hasDealtDamage = new List<GameObject>();
        audioSource = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
    }

    void Update()
    {
        if(canDealDamage)
        {
            RaycastHit hit;

            int layerMask = 1 << 9;
            if(Physics.Raycast(transform.position, -transform.up, out hit, weaponLength, layerMask))
            {
                if(!hasDealtDamage.Contains(hit.transform.gameObject))
                {
                    Enemy enemy = hit.transform.gameObject.GetComponent<Enemy>();

                    if(enemy != null)
                    {
                        enemy.Damage((int) weaponDamage);
                    }

                    hasDealtDamage.Add(hit.transform.gameObject);
                }
            }
        }
    }

    public void StartDealDamage()
    {
        if(clipTracker % 2 == 0)
        {
            clipTracker = 0;
            audioSource.PlayOneShot(clipOne);
        }
        else
        {
            audioSource.PlayOneShot(clipTwo);
        }
        clipTracker++;

        canDealDamage = true;
        hasDealtDamage.Clear();
    }

    public void EndDealDamage()
    {
        canDealDamage=false;
    }
}
