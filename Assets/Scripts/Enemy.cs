using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public AudioSource audioSource;
    public AudioClip hitClip;
    int health = 100;
    int playerXP = 10;

    public void Start()
    {
        audioSource = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (health <= 0)
        {
            Die();
        }
    }

    public void Damage(int damageAmount)
    {
        //This is where we would modify the enemy health
        audioSource.PlayOneShot(hitClip);
        Debug.Log("Damage: " + damageAmount);
        health -= damageAmount;
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
