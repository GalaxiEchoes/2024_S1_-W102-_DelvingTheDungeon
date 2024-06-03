using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public AudioSource audioSource;
    public AudioClip hitClip;
    public int maxHealth = 100;
    public int currentHealth;
    private EnemyHealth enemyHealth;

    private void Awake()
    {
        enemyHealth = GetComponentInChildren<EnemyHealth>();
    }

    public void Start()
    {
        currentHealth = maxHealth;
        enemyHealth.UpdateHealthBar(currentHealth, maxHealth);

        if (audioSource == null)
        {
            audioSource = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
        }
    }

    public void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Damage(int damageAmount)
    {
        if (audioSource != null && hitClip != null)
        {
            audioSource.PlayOneShot(hitClip);
        }

        Debug.Log("Damage: " + damageAmount);
        currentHealth -= damageAmount;
        enemyHealth.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
