using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public void Damage(int damageAmount)
    {
        Debug.Log("Damage: " + damageAmount);
    }


    /*public int health = 150;
    public Material red;
    public Material green;

    private Coroutine AnimationCoroutine;
    private bool IsRunning;

    public void TakeDamage(int damage)
    {
        health -= damage;

        if(!IsRunning)
        {
            IsRunning = true;
            AnimationCoroutine = StartCoroutine(DoColourSwap());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0) 
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DoColourSwap()
    {
        GameObject go = this.gameObject;
        float time = 0;
        while (time < 0.5)
        {
            go.GetComponent<MeshRenderer>().material = red;
            yield return null;
            time += Time.deltaTime;
        }
        go.GetComponent<Renderer>().material = green;

        IsRunning = false;
    }*/

}
