using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehavior : MonoBehaviour
{
    [SerializeField]
    private Transform boidPrefab;

    [SerializeField]
    private int spawnCount = 10;

    [SerializeField]
    private int spawnRadius = 10;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < spawnCount; ++i)
        {
            Transform boid = Instantiate(boidPrefab, transform.position + Random.onUnitSphere * spawnRadius, Random.rotation);
            //boid.position = ;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
