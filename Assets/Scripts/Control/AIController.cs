using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;

namespace RPG.Control{
    public class AIController : MonoBehaviour 
    {
        GameObject player;
        Health health;
        Fighter fighter;
        Mover mover;
        [SerializeField] float chaseDistance = 5f;
        
        
        bool isInCombat;
        Vector3 guardLocation;

        private void Awake() {
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            guardLocation = transform.position;
        }

        private void Update()
        {
            if(health.IsDead()) return;
            if(GetIsInChaseRange())
            {
                fighter.Attack(player);
                isInCombat = true;
            }
            else if(isInCombat)
            {
                mover.StartMoveAction(guardLocation);
            }
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
