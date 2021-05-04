using System;
using UnityEngine;
using Random = System.Random;

public class FlockManager : GOPool
{
    [SerializeField] public Vector3 swimBounds = Vector3.one * 5;

    public GameObject[] flock;

    [Header("Flock Settings")] [Range(0, 5)]
    public float minSpeed = 1;

    [Range(0, 5)] public float maxSpeed = 5;
    [Range(0, 10)] public float neighborDistance = 5;
    [Range(0, 5)] public float turnSpeed = 1;
    public float avoidanceThershold = 1;
    public Vector3 GoalPosition { get; private set; }
    [Range(0, 10)] public float goalFloating = 1;

    protected override void Start()
    {
        base.Start();

        InitializeFlock();
    }

    private void Update()
    {
        if (UnityEngine.Random.Range(0, 100) < 10)
        {
            GoalPosition = transform.position + RandomLocation(goalFloating);
        }
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

    private Vector3 RandomLocation(float max)
    {
        return new Vector3(randomBetween(max), randomBetween(max), randomBetween(max));
    }

    private float randomBetween(float max)
    {
        return UnityEngine.Random.Range(-max, max);
    }

    private void EnableAllFish()
    {
        foreach (GameObject fish in flock)
        {
            fish.SetActive(true);
        }
    }
}