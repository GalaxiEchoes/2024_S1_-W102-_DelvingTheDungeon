using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitClip;
    public AudioClip deathNoise;
    
    [Header("GameObject References")]
    private EnemyHealth enemyHealth;
    private Animator animator;
    private GameObject player;
    private NavMeshAgent agent;
    [SerializeField] GameObject ragdoll;
    
    XPManager xpManager;

    [Header("Enemy Stats")]
    int playerXP = 10;
    int money = 50;
    public int maxHealth = 200;
    public int currentHealth;

    [Header("Combat")]
    [SerializeField] float attackCD = 3f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float aggroRange = 4f;
    [SerializeField] float rotationSpeed = 5f;
    float timePassed;
    float newDestinationCD = 0.5f;

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

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        audioSource = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
        if (player != null)
        {
            xpManager = player.GetComponentInChildren<Player>().xpManager;
        }
    }

    public void Update()
    {
        if(animator != null && player!= null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude / agent.speed);

            if (timePassed >= attackCD)
            {
                if (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
                {
                    animator.SetTrigger("Attack");
                    timePassed = 0;
                }
            }
            timePassed += Time.deltaTime;

            if (newDestinationCD <= 0 && Vector3.Distance(player.transform.position, transform.position) <= aggroRange)
            {
                newDestinationCD = 0.5f;
                agent.SetDestination(player.transform.position);
            }
            newDestinationCD -= Time.deltaTime;
        }
        

        var targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

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
        if (animator != null) animator.SetTrigger("WasDamaged");
        enemyHealth.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (ragdoll != null) 
            Instantiate(ragdoll, transform.position, transform.rotation);

        //Rewards
        if (xpManager != null)
        {
            xpManager.addXP(playerXP);
        }
        if (player != null)
        {
            player.GetComponentInChildren<Player>().gainMoney(money);
        }
        audioSource.PlayOneShot(deathNoise);

        Destroy(gameObject);
    }

    public void StartDealDamage()
    {
        GetComponentInChildren<DamageDealer>().StartDealDamage();
    }

    public void EndDealDamage()
    {
        GetComponentInChildren<DamageDealer>().EndDealDamage();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
