using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using GameDevTV.Utils;
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
        [SerializeField] float aggroCooldownTime = 5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 2f;
        [SerializeField] float patrolSpeed = 3f;
        [SerializeField] float chaseSpeed = 4.5f;
        [SerializeField] float shoutDistance = 5f;
        
        LazyValue<Vector3> _guardPosition;
        public Vector3 guardPosition
        {
            get {return _guardPosition.value;}
            set {_guardPosition.value=value;}
        }
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceAggro = Mathf.Infinity;
        float timeSpentDwelling = 0;
        bool hasBeenAggroedRecently = false;
        int waypointIndex = 0;

        private void Awake() {
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            scheduler = GetComponent<ActionScheduler>();
            _guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Start() 
        {
            _guardPosition.ForceInit();
        }

        private void Update()
        {
            if(health.IsDead()) return;
            if(IsAggravated() && fighter.CanAttack(player))
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

            UpdateTimers();

            if (timeSinceAggro >= aggroCooldownTime && 
            timeSinceLastSawPlayer >= suspicionTime)
            {
                hasBeenAggroedRecently = false;
            }
        }

        public void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceAggro += Time.deltaTime;
        }

        public void Aggravate()
        {
            timeSinceAggro = 0;
        }

        public void AggroAllies()
        {
            if (hasBeenAggroedRecently == true) {return;}
 
            if (hasBeenAggroedRecently == false)
            {
                timeSinceAggro = 0f;
                timeSinceLastSawPlayer = 0f;
                hasBeenAggroedRecently = true;
            }
        }

        private void AttackBehavior()
        {
            timeSinceLastSawPlayer = 0;
            mover.SetSpeed(chaseSpeed);
            fighter.Attack(player);

            AggroNearbyEnemies();
        }

        private void AggroNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                AIController ai = hit.collider.GetComponent<AIController>();
                
                if(ai == null || ai == this) continue;
                
                ai.AggroAllies();
            }
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

        private bool IsAggravated()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer < chaseDistance || timeSinceAggro < aggroCooldownTime;
        }

        //called by unity
        private void OnDrawGizmosSelected() {
            //set gizmo colors to blue
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position,chaseDistance);
        }
    }
}
