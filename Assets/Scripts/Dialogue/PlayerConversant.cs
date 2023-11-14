using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Server;
using RPG.Dialogue;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        Dialogue currentDialogue;
        DialogueNode currentNode = null;
        bool isChoosing = false;
        AIConversant currentConversent = null;

        public event Action onConversationUpdated;

        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            currentConversent = newConversant;
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            TriggerEnterAction();
            onConversationUpdated();
        }

        public void Quit()
        {
            currentDialogue = null;
            TriggerExitAction();
            currentNode = null;
            isChoosing = false;
            currentConversent = null;
            onConversationUpdated();
        }

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        public string GetText()
        {
            if (currentNode == null)
            {
                return "";
            }

            return currentNode.GetDialogueText();
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return currentDialogue.GetPlayerChildren(currentNode);
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            isChoosing = false;
            Next();
        }

        public void Next()
        {
            int numPlayerResponses = currentDialogue.GetPlayerChildren(currentNode).Count();
            if (numPlayerResponses > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated();
                return;
            }

            DialogueNode[] children = currentDialogue.GetAIChildren(currentNode).ToArray();
            int randIdx = UnityEngine.Random.Range(0,children.Count());
            TriggerExitAction();
            currentNode = children[randIdx];
            TriggerEnterAction();
            onConversationUpdated();
        }

        public bool HasNext()
        {
            return currentDialogue.GetAllChildren(currentNode).Count() > 0;
        }

        private void TriggerEnterAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnEnterAction());
            }
        }

        private void TriggerExitAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnExitAction());
            }
        }

        private void TriggerAction(IEnumerable<DialogueNode.DialogueAction> actions)
        {
            DialogueTrigger dialogueTrigger = currentConversent.GetComponent<DialogueTrigger>();
            
            foreach (DialogueNode.DialogueAction action in actions)
            {
                if(action == DialogueNode.DialogueAction.None)
                    continue;
        
                foreach (var trigger in dialogueTrigger.Triggers)
                    trigger.TriggerAction(action);
            }
        }

        public string GetCurrentSpeakerName()
        {
            if(isChoosing)
            {
                return "Player";
            }
            else
            {
                return currentConversent.GetName();
            }
        }
    }
}