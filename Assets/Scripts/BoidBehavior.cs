using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidBehavior : MonoBehaviour
{
    private Vector3 velocity = default;
    private Vector3 acceleration = default;
    private GameObject orbitalCenter = default;

    [SerializeField]
    private float visibilityRadius = 5.0f;

    [SerializeField]
    private float maxForce = 1.0f;

    [SerializeField]
    private float maxSpeed = 4.0f;

    [SerializeField]
    private float orbitMinRadius = 50.0f;

    [SerializeField]
    private float alignmentWeight = 1.0f;

    [SerializeField]
    private float cohesionWeight = 1.0f;

    [SerializeField]
    private float separationWeight = 1.0f;

    [SerializeField]
    private float orbitWeight = 1.0f;

    public Vector3 Velocity { get => velocity; set => velocity = value; }

    // Start is called before the first frame update
    void Start()
    {
        //Velocity = transform.forward;
        orbitalCenter = GameObject.FindGameObjectWithTag("Orbital Center");

    }

    void Update()
    {
        Flock();
    }

    private void Move()
    {
        Velocity += acceleration * Time.deltaTime;
        Velocity = Vector3.ClampMagnitude(Velocity, maxSpeed);
        transform.position += Velocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(Velocity);

        acceleration = Vector3.zero;
    }

    private List<Collider> GetNearbyBoids()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, visibilityRadius);
        List<Collider> nearbyBoids = new List<Collider>();
        foreach (Collider nearbyObject in nearbyObjects)
        {
            if (nearbyObject.gameObject.tag == "Boid")
            {
                nearbyBoids.Add(nearbyObject);
            }
        }

        return nearbyBoids;
    }

    private Vector3 GetAlignmentVector(List<Collider> boids) 
    {
        Vector3 alignmentVector = Vector3.zero;

        if (boids.Count > 0)
        {
            Vector3 sumVector = Vector3.zero;

            foreach (Collider boid in boids)
            {
                sumVector += boid.GetComponent<BoidBehavior>().Velocity;
            }

            alignmentVector = (sumVector / boids.Count).normalized;
        }

        return alignmentVector;
    }

    private Vector3 GetCohesionVector(List<Collider> boids)
    {
        Vector3 avgPosition = Vector3.zero;

        if (boids.Count > 0)
        {
            Vector3 sumVector = Vector3.zero;

            foreach (Collider boid in boids)
            {
                sumVector += boid.transform.position;
            }

            avgPosition = sumVector / boids.Count;
        }

        return (avgPosition - transform.position);
    }

    private Vector3 GetAverageEvasiveVector(List<Collider> boids)
    {
        Vector3 avgEvasiveVector = Vector3.zero;

        if (boids.Count > 0)
        {
            Vector3 sumVector = Vector3.zero;

            foreach (Collider boid in boids)
            {
                Vector3 evasiveVector = transform.position - boid.transform.position;
                evasiveVector = (visibilityRadius - evasiveVector.magnitude) * evasiveVector.normalized;
                sumVector += evasiveVector;
            }

            avgEvasiveVector = sumVector / boids.Count;
        }

        return avgEvasiveVector;
    }

    private Vector3 GetOrbitalVector()
    {
        Vector3 orbitalVector = orbitalCenter.transform.position - transform.position;
        if (orbitalVector.magnitude > orbitMinRadius)
        {
            orbitalVector = orbitalVector.normalized * (orbitalVector.magnitude - orbitMinRadius);
        }
        else
        {
            orbitalVector = Vector2.zero;
        }

        return orbitalVector;
    }

    //private Vector3[] CalculateFlockVectors(List<Collider> boids)
    //{
    //    Vector3 avgAlignmentVector = Vector3.zero;
    //    Vector3 avgPosition = Vector3.zero;
    //    Vector3 avgEvasiveVector = Vector3.zero;

    //    if (boids.Count > 0)
    //    {
    //        Vector3 alignmentSumVector = Vector3.zero;
    //        Vector3 cohesionSumVector = Vector3.zero;
    //        Vector3 evasiveSumVector = Vector3.zero;

    //        foreach (Collider boid in boids)
    //        {
    //            // Alignment
    //            alignmentSumVector += boid.GetComponent<BoidBehavior>().Velocity;

    //            // Cohesion
    //            cohesionSumVector += boid.transform.position;

    //            // Evasion
    //            Vector3 evasiveVector = transform.position - boid.transform.position;
    //            evasiveVector = (visibilityRadius - evasiveVector.magnitude) * evasiveVector.normalized;
    //            evasiveSumVector += evasiveVector;
    //        }

    //        avgAlignmentVector = (alignmentSumVector / boids.Count).normalized;
    //        avgPosition = cohesionSumVector / boids.Count;
    //        avgEvasiveVector = evasiveSumVector / boids.Count;
    //    }

    //    Vector3[] vectors = new Vector3[3];
    //    vectors[0] = avgAlignmentVector;
    //    vectors[1] = avgPosition;
    //    vectors[2] = avgEvasiveVector;

    //    return vectors;
    //}

    //private void SteerToPosition(Transform target)
    //{
    //    SteerToPosition(target.position);
    //}

    //private void SteerToPosition(Vector3 target)
    //{
    //    Vector3 desired = (target - transform.position).normalized * maxSpeed;
    //    Steer(desired);
    //}

    private void Steer(Vector3 desiredVector)
    {
        Vector3 steeringForce = desiredVector - Velocity;
        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
        acceleration += steeringForce;
    }

    private void Orbit()
    {
        Vector3 orbitalVector = GetOrbitalVector();
        Vector3 desiredVelocity = orbitalVector * maxSpeed;
        Steer(desiredVelocity);
    }

    private void Align(List<Collider> boids)
    {
        Vector3 alignmentVector = GetAlignmentVector(boids);
        Vector3 desiredVelocity = Vector3.ClampMagnitude(alignmentVector.normalized * alignmentWeight, maxSpeed);
        Steer(desiredVelocity);
    }

    private void Cohesion(List<Collider> boids)
    {
        Vector3 cohesionVector = GetCohesionVector(boids);
        Vector3 desiredVelocity = Vector3.ClampMagnitude(cohesionVector.normalized * cohesionWeight, maxSpeed);
        Steer(desiredVelocity);
    }

    private void Separation(List<Collider> boids)
    {
        Vector3 evasiveVector = GetAverageEvasiveVector(boids);
        Vector3 desiredVelocity = Vector3.ClampMagnitude(evasiveVector.normalized * separationWeight, maxSpeed);
        Steer(desiredVelocity);
    }

    private void Flock()
    {
        List<Collider> boids = GetNearbyBoids();
        //Vector3[] vectors = CalculateFlockVectors(boids);
        //foreach (Vector3 vector in vectors)
        //{
        //    Steer(vector);
        //}

        Orbit();
        Align(boids);
        Cohesion(boids);
        Separation(boids);
        Move();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, visibilityRadius);
    }
}
