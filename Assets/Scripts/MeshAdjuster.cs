using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class MeshAdjuster : MonoBehaviour
{
    [SerializeField] private NavMeshSurface surface;

    // Start is called before the first frame update
    void Start()
    {
        surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
        StartCoroutine(WaitToGenerate());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitToGenerate()
    {
        yield return new WaitForSeconds(4);
        surface.BuildNavMesh();
    }
}
