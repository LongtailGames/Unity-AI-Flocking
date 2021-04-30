using System;
using UnityEngine;
using Random = System.Random;

public class FlockManager : GOPool
{
    [SerializeField] private Vector3 swimBounds = Vector3.one*5;
    
    public GameObject[] flock;
    [Header("Flock Settings")]
    [Range(0,5)]
    public float minSpeed = 1;
    [Range(0,5)]
    public float maxSpeed = 5;
    [Range(1,10)]
    public float neighborDistance = 5;

    public float avoidanceThershold = 1;

    protected override void Start()
    {
        base.Start();

        InitializeFlock();
    }

    private void InitializeFlock()
    {
        flock = pool.GetPoolAsArray();
        SetRandomPosition();
        EnableAllFish();
        SetFlockManagerOfFish();
    }

    private void SetFlockManagerOfFish()
    {
        foreach (GameObject fisGameObject in flock)
        {
            fisGameObject.GetComponent<Fish>().flockManager = this;
        }
    }

    private void SetRandomPosition()
    {
        foreach (GameObject fish in flock)
        {
            fish.transform.position = RandomFishStartLocation();
        }
    }

    private Vector3 RandomFishStartLocation()
    {
        return new Vector3(randomBetween(swimBounds.x), randomBetween(swimBounds.y), randomBetween(swimBounds.z));
    }

    private float randomBetween(float max)
    {
        return UnityEngine.Random.Range(-max,max);
    }

    private void EnableAllFish()
    {
        foreach (GameObject fish in flock)
        {
            fish.SetActive(true);
        }
    }

}
