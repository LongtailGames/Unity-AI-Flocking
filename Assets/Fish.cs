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

    void Start()
    {
        speed = RandomSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        ApplyRules();
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
        Vector3 lookDirection = (averagePosition + avoidanceVector) - transform.position;
        RotateTowards(lookDirection);
    }

    private GameObject[] GetFishInGroup()
    {
        GameObject[] closeEnough = flockManager.flock.Where(o =>
            Vector3.Distance(o.transform.position, transform.position) < flockManager.neighborDistance).ToArray();

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
            if (Vector3.Distance(o.transform.position, transform.position) < avoidThreshold)
            {
                avoidDistanceSum += transform.position - o.transform.position;
            }
        }

        return avoidDistanceSum / myGroup.Length;
    }

    private float GetGroupSpeed(GameObject[] myGroup)
    {
        float sum = myGroup.Sum(fish => fish.GetComponent<Fish>().speed);
        return sum / myGroup.Length;
    }


    private void RotateTowards(Vector3 lookDirection)
    {
//TODO: What if look rotation is zero?
        transform.rotation =
            Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime);
    }
}
