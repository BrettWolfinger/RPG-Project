using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.UI;
using UnityEngine;


namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] string npcName;
        [SerializeField] Dialogue npcDialogue = null;

        public CursorType_SO GetCursorType(PlayerController callingController)
        {
            return callingController.cursors.DialogueCursor;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if(npcDialogue == null)
            {
                return false;
            }
            if(Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<PlayerConversant>().StartDialogue(this, npcDialogue);
            }

            return true;
        }

        public string GetName()
        {
            return npcName;
        }
    }
}