using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System;

namespace RPG.Control{
    public class AIController : MonoBehaviour 
    {
        GameObject player;
        Health health;
        Fighter fighter;
        Mover mover;
        ActionScheduler scheduler;
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 2f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 2f;
        [SerializeField] float patrolSpeed = 3f;
        [SerializeField] float chaseSpeed = 4.5f;
        
        Vector3 guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSpentDwelling = 0;
        int waypointIndex = 0;

        private void Awake() {
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            scheduler = GetComponent<ActionScheduler>();
            guardPosition = transform.position;
        }

        private void Update()
        {
            if(health.IsDead()) return;
            if(GetIsInChaseRange())
            {
                AttackBehavior();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehavior();
            }
            else
            {
                PatrolBehavior();
            }

            timeSinceLastSawPlayer += Time.deltaTime;
        }

        private void AttackBehavior()
        {
            timeSinceLastSawPlayer = 0;
            mover.SetSpeed(chaseSpeed);
            fighter.Attack(player);
        }

        private void SuspicionBehavior()
        {
            scheduler.CancelCurrentAction();
        }

        private void PatrolBehavior()
        {
            mover.SetSpeed(patrolSpeed);
            Vector3 nextPosition = guardPosition;
            if(patrolPath != null)
            {
                if(AtWaypoint())
                {
                    timeSpentDwelling += Time.deltaTime;
                    if(timeSpentDwelling > waypointDwellTime)
                    {
                        timeSpentDwelling = 0;
                        CycleNextWaypoint();
                    }
                }
                nextPosition = GetCurrentWaypoint();
            }

            mover.StartMoveAction(nextPosition);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position,GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleNextWaypoint()
        {
            waypointIndex = patrolPath.GetNextIndex(waypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(waypointIndex);
        }

        private bool GetIsInChaseRange()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance;
        }

        //called by unity
        private void OnDrawGizmosSelected() {
            //set gizmo colors to blue
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position,chaseDistance);
        }
    }
}
