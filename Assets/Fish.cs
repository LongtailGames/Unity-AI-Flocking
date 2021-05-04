using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Fish : MonoBehaviour
{
    public FlockManager flockManager;
    public float speed;
    public float RandomSpeed => UnityEngine.Random.Range(flockManager.maxSpeed, flockManager.minSpeed);
    private Vector3 myPosition;

    void Start()
    {
        speed = RandomSpeed;
    }

    void Update()
    {
        myPosition = transform.position;

        bool obstacle = Physics.Raycast(myPosition, transform.forward * 1, out RaycastHit hit);
        if (obstacle)
        {
            Debug.DrawRay(myPosition, transform.forward * 1, Color.red);
            Vector3 awayFromObstacle = Vector3.Reflect(transform.forward, hit.normal);
            RotateTowards(awayFromObstacle, flockManager.turnSpeed);
            transform.Translate(Vector3.forward * (speed * Time.deltaTime));
            return;
        }

        if (IsOutSideBounds())
        {
            Vector3 toManager = flockManager.transform.position - myPosition;
            RotateTowards(toManager, flockManager.turnSpeed);
            transform.Translate(Vector3.forward * (speed * Time.deltaTime));
            return;
        }

        if (PercentageChance(20))
        {
            ApplyRules();
        }

        if (PercentageChance(10))
        {
            speed = RandomSpeed;
        }
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
    }

    bool IsOutSideBounds()
    {
        Bounds b = new Bounds(flockManager.transform.position, flockManager.swimBounds * 2);
        return !b.Contains(myPosition);
    }

    private static bool PercentageChance(int percent)
    {
        return UnityEngine.Random.Range(0, 100) < percent;
    }

    private void ApplyRules()
    {
        GameObject[] myGroup = GetFishInGroup();
        if (myGroup.Length == 0)
        {
            return;
        }
//TODO: So many calls to transform.position, I may have to cache it.

        Vector3 averagePosition = GroupAveragePosition(myGroup);
        Vector3 avoidanceVector = GetAvoidanceVector(myGroup);
        speed = GetGroupSpeed(myGroup);
        Vector3 toGoal = flockManager.GoalPosition - myPosition;
        Vector3 lookDirection = (averagePosition + avoidanceVector + toGoal) - myPosition;
        float turnSpeed = flockManager.turnSpeed;
        RotateTowards(lookDirection, turnSpeed);
    }

    private GameObject[] GetFishInGroup()
    {
        GameObject[] closeEnough = flockManager.flock.Where(o =>
            Vector3.Distance(o.transform.position, myPosition) < flockManager.neighborDistance).ToArray();

        return closeEnough;
    }

    private Vector3 GroupAveragePosition(GameObject[] myGroup)
    {
        Vector3 sum =
            myGroup.Aggregate(new Vector3(),
                (current, o) => current + o.transform.position);

        return sum / myGroup.Length;
    }

    private Vector3 GetAvoidanceVector(GameObject[] myGroup)
    {
        Vector3 avoidDistanceSum = new Vector3();
        float avoidThreshold = flockManager.avoidanceThershold;
        foreach (GameObject o in myGroup)
        {
            if (Vector3.Distance(o.transform.position, myPosition) < avoidThreshold)
            {
                avoidDistanceSum += myPosition - o.transform.position;
            }
        }

        return avoidDistanceSum;
    }

    private float GetGroupSpeed(GameObject[] myGroup)
    {
        float sum = myGroup.Sum(fish => fish.GetComponent<Fish>().speed);
        return sum / myGroup.Length;
    }


    private void RotateTowards(Vector3 lookDirection, float turnSpeed)
    {
//TODO: What if look rotation is zero?
        transform.rotation =
            Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), turnSpeed * Time.deltaTime);
    }
}