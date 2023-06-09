using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using RPG.UI;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour {
        
        Mover mover;
        Fighter fighter;
        Health health;

        [System.Serializable]
        public struct Cursors
        {
            public CursorType_SO NoneCursor;
            public CursorType_SO UICursor;
            public CursorType_SO MovementCursor;
            public CursorType_SO CombatCursor;
            public CursorType_SO PickupCursor;
        }
        [SerializeField] public Cursors cursors;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float raycastRadius = 1f;


        void Awake()
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
        }
        void Update()
        {
            if(InteractWithUI()) return;
            if(health.IsDead()) 
            {
                cursors.NoneCursor.SetCursor();
                return;
            }

            if(InteractWithComponent()) return;
            if(InteractWithMovement()) return;

            cursors.NoneCursor.SetCursor();
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach(IRaycastable raycastable in raycastables)
                {
                    if(raycastable.HandleRaycast(this))
                    {
                        raycastable.GetCursorType(this).SetCursor();
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];
            for(int i=0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances,hits);

            return hits;
        }

        private bool InteractWithUI()
        {
            if(EventSystem.current.IsPointerOverGameObject())
            {
                cursors.UICursor.SetCursor();
                return true;
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            //RaycastHit hit;
            //bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if(!mover.CanMoveTo(target)) return false;

                if (Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(target);
                }
                cursors.MovementCursor.SetCursor();
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit)
            {
                NavMeshHit navMeshHit;
                bool hasCastToNavMesh = NavMesh.SamplePosition(
                    hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
                if(!hasCastToNavMesh) return false;

                target = navMeshHit.position;
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}