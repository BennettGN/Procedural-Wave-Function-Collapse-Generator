using UnityEngine;
using UnityEngine.AI;

public class AgentRider : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Movement Settings")]
    public float lookAheadDistance = 20f;
    public float updateInterval = 0.1f;
    private float nextUpdateTime;

    [Header("NavMesh Area Restriction")]
    public string allowedAreaName = "Drivable";
    private int allowedAreaMask;

    [Header("Random Turning")]
    public float turnCheckInterval = 1f;
    public float turnChance = 0.3f;
    private float nextTurnCheckTime;

    [Header("Turnaround Settings")]
    public float turnaroundSearchDistance = 15f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Set allowed area mask for NavMesh sampling/pathfinding
        int area = NavMesh.GetAreaFromName(allowedAreaName);
        allowedAreaMask = 1 << area;
        agent.areaMask = allowedAreaMask;

        // Snap to NavMesh
        if (TryFindNavPosition(transform.position, 10f, out NavMeshHit hit))
        {
            agent.Warp(hit.position);
            SetForwardDestination();
        }
    }

    void Update()
    {
        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        // If not on allowed area → try recovery
        if (!TryFindNavPosition(transform.position, 0.5f, out _))
        {
            TryTurnAround();
            return;
        }

        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateInterval;
            SetForwardDestination();
        }
    }


    void SetForwardDestination()
    {
        //Give up if fell off nav mesh/road
        if (!agent.isOnNavMesh) return;

        // Consider random turn
        if (Time.time >= nextTurnCheckTime && Random.value < turnChance)
        {
            nextTurnCheckTime = Time.time + turnCheckInterval;
            if (TryRandomTurn()) return;
        }

        // Move forward
        Vector3 forward = transform.position + transform.forward * lookAheadDistance;

        //Continue moving in path of nav mesh if possible
        if (NavMesh.SamplePosition(forward, out NavMeshHit hit, lookAheadDistance, allowedAreaMask))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
        }
        else
        {
            TryTurnAround();
        }
    }

    bool TryRandomTurn()
    {
        float turnAngle = (Random.value > 0.5f) ? 90f : -90f;
        Vector3 dir = Quaternion.Euler(0, turnAngle, 0) * transform.forward;
        Vector3 target = transform.position + dir * lookAheadDistance;

        if (TryFindNavPosition(target, lookAheadDistance, out NavMeshHit hit))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
            return true;
        }

        return false;
    }

    // Attempts to turn in all directions when stuck
    void TryTurnAround()
    {
        float[] angles = { 180f, 135f, -135f, 90f, -90f, 45f, -45f };

        foreach (float angle in angles)
        {
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
            Vector3 target = transform.position + dir * turnaroundSearchDistance;

            if (TryValidPath(target, out Vector3 validPoint))
            {
                agent.isStopped = false;
                agent.SetDestination(validPoint);
                return;
            }
        }

        // Last-ditch recovery
        if (TryFindNavPosition(transform.position, turnaroundSearchDistance * 2, out NavMeshHit nearHit))
        {
            agent.SetDestination(nearHit.position);
        }
        else
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    bool TryFindNavPosition(Vector3 point, float range, out NavMeshHit hit)
    {
        return NavMesh.SamplePosition(point, out hit, range, allowedAreaMask);
    }

    //Attempts to find a valid path to the target position
    bool TryValidPath(Vector3 target, out Vector3 result)
    {
        result = Vector3.zero;
        if (!TryFindNavPosition(target, turnaroundSearchDistance, out NavMeshHit hit))
            return false;

        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            result = hit.position;
            return true;
        }

        return false;
    }
}
