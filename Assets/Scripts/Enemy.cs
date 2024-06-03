using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public AudioSource audioSource;
    public AudioClip hitClip;
    int health = 100;
    int playerXP = 10;
    int money = 50;
    
    XPManager xpManager;
    MoneyTracker moneyTracker;

    public void Start()
    {
        audioSource = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Player>();
        if(player != null )
        {
            xpManager = player.xpManager;
        }
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
        if(xpManager != null)
        {
            xpManager.addXP(playerXP);
            moneyTracker.setMoney(money);
        }
        Destroy(gameObject);
    }
}
