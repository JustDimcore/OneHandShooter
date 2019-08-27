using UnityEngine;
using UnityEngine.AI;

public class Chicken : Enemy
{
    private NavMeshAgent _navMeshAgent;

    protected override void StartImpl()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected void Update()
    {
        if (!_navMeshAgent.pathPending && !_navMeshAgent.hasPath)
        {
            ChangeDirection();
        }
    }

    protected void ChangeDirection()
    {
        Vector3 pos;
        pos = transform.position;
        pos.x += Random.Range(-2, 2);
        pos.z += Random.Range(-2, 2);

        NavMeshHit hit;
        NavMesh.SamplePosition(pos, out hit, 2, NavMesh.AllAreas);
        _navMeshAgent.destination = hit.position;
        _navMeshAgent.isStopped = false;
    }
}