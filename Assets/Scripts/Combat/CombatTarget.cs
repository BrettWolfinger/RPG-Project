using UnityEngine;
using RPG.Attributes;
using RPG.Control;
using RPG.UI;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Fighter>().Attack(gameObject);
            }
            return true;
        }

        public CursorType_SO GetCursorType(PlayerController callingController)
        {
            return callingController.cursors.CombatCursor;
        }
    }
}