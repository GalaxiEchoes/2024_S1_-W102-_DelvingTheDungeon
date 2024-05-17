using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        audioSource.PlayOneShot(hitClip);
        Debug.Log("Damage: " + damageAmount);
    }
}
