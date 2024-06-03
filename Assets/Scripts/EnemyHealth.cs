using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public Slider enemySlider;

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (enemySlider != null)
        {
            enemySlider.value = (float)currentHealth / maxHealth;
        }
    }

    private void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        var targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5.0f * Time.deltaTime);
    }
}