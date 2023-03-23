using UnityEngine;
using RPG.Movement;
namespace RPG.Combat
{
    public class Fighter : MonoBehaviour
    {
        //Cached component variables
        Mover mover;
        Transform target;

        //Fighting Variables
        [SerializeField] float weaponRange = 2f;
        private void Awake() {
            mover = GetComponent<Mover>();
        }
        private void Update()
        {
            if (!target) return;

            if (GetIsOutOfRange())
            {
                mover.MoveTo(target.position);
            }
            else
            {
                mover.Stop();
            }
        }

        private bool GetIsOutOfRange()
        {
            return Vector3.Distance(target.position, GetComponent<Transform>().position) > weaponRange;
        }

        public void Attack(CombatTarget combatTarget)
        {
            target = combatTarget.transform;
        }

        public void Cancel()
        {
            target = null;
        }
    }
}