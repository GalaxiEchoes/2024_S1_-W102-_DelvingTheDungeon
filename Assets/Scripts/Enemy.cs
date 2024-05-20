using UnityEngine;

public class Enemy : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hitClip;

    public void Start()
    {
        audioSource = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
    }

    public void Damage(int damageAmount)
    {
        //This is where we would modify the enemy health
        audioSource.PlayOneShot(hitClip);
        Debug.Log("Damage: " + damageAmount);
    }
}
