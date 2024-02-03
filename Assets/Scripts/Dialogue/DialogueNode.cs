using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField]
        bool isPlayerSpeaking = false;
        [SerializeField]
        private string dialogueText;
        [SerializeField]
        private List<string> childrenNodes = new List<string>();
        [SerializeField]
        private Rect rect = new Rect(0,0,200,100);

        public enum DialogueAction
        {
            None,
            TestRootEntry,
            TestSelection,
            TestAIExit,
            Attacking,
            GiveQuest,
            CompleteObjective
        }
        [SerializeField] private DialogueAction[] onEnterAction;
        [SerializeField] private DialogueAction[] onExitAction;

        public string GetDialogueText()
        {
            return dialogueText;
        }

        public List<String> GetChildrenNodes()
        {
            return childrenNodes;
        }

        public Rect GetRect()
        {
            return rect;
        }
        public bool IsPlayerSpeaking()
        {
            return isPlayerSpeaking;
        }

        public DialogueAction[] GetOnEnterAction()
        {
            return onEnterAction;
        }

        public DialogueAction[] GetOnExitAction()
        {
            return onExitAction;
        }

#if UNITY_EDITOR
        public void SetRectPosition(Vector2 newRectPosition)
        {
            Undo.RecordObject(this, "Move Dialogue Node");
            rect.position = newRectPosition;
            EditorUtility.SetDirty(this); //needed because this is a subasset of the dialogue scriptable object
        }

        public void SetDialogueText(string newText)
        {
            if(newText != dialogueText)
            {
                Undo.RecordObject(this, "Update Dialogue Text");
                dialogueText = newText;
                EditorUtility.SetDirty(this);
            }
        }

        public void RemoveChildNode(string nodeToRemoveName)
        {
            Undo.RecordObject(this, "Deleted Dialogue Link");
            childrenNodes.Remove(nodeToRemoveName);
            EditorUtility.SetDirty(this);
        }

        public void AddChildNode(string nodeToAddName)
        {
            Undo.RecordObject(this, "Added Dialogue Link");
            childrenNodes.Add(nodeToAddName);
            EditorUtility.SetDirty(this);
        }

        public void SetSpeaker(bool newSpeaker)
        {
            Undo.RecordObject(this, "Set Dialogue Speaker");
            isPlayerSpeaking = newSpeaker;
            EditorUtility.SetDirty(this);
        }
#endif
    }
}