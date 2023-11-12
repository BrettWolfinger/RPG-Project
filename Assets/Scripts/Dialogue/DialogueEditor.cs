using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow 
    {
        //Serialized fields
        Dialogue selectedDialogue = null;
        Vector2 scrollPosition;

        [NonSerialized] GUIStyle nodeStyle;
        [NonSerialized] GUIStyle playerNodeStyle;
        [NonSerialized] DialogueNode draggingNode = null;
        [NonSerialized] Vector2 draggingOffset;
        [NonSerialized] DialogueNode creatingNode = null;
        [NonSerialized] DialogueNode deletingNode = null;
        [NonSerialized] DialogueNode linkingParentNode = null;
        [NonSerialized] private bool showContextMenu;
        [NonSerialized] private Vector2 contextMenuPosition;

        const float CANVASSIZE = 4000;
        const float BACKGROUNDSIZE = 50;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowWindow() 
        {
            GetWindow(typeof(DialogueEditor),false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            //Check specific dialogue asset
            if(EditorUtility.InstanceIDToObject(instanceID) is Dialogue)
            {
                ShowWindow();
                return true;
            }
            return false;
        }

        private void Awake() {
            OnSelectionChange();

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.padding = new RectOffset(20,20,20,20);
            nodeStyle.border = new RectOffset(12,12,12,12);

            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.padding = new RectOffset(20,20,20,20);
            playerNodeStyle.border = new RectOffset(12,12,12,12);
        }

        private void OnSelectionChange()
        {
            if(Selection.activeObject is Dialogue)
            {
                selectedDialogue = Selection.activeObject as Dialogue;
            }
        }

        private void OnGUI() 
        {
            if(selectedDialogue == null)
            {
                
                EditorGUILayout.LabelField("No Dialogue Selected");
            }
            else
            {
                ProcessEvents();
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                //Setup background canvas of size for scroll view
                Rect canvas = GUILayoutUtility.GetRect(CANVASSIZE,CANVASSIZE);
                Texture2D background = Resources.Load("background") as Texture2D;
                Rect texCoords = new Rect(0,0,CANVASSIZE/BACKGROUNDSIZE, CANVASSIZE/BACKGROUNDSIZE);
                GUI.DrawTextureWithTexCoords(canvas,background, texCoords);


                foreach(DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach(DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if(creatingNode != null)
                {
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }
                if(deletingNode != null)
                {
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
                if (showContextMenu)
                {
                    ShowContextMenu();
                }
            }
        }

        private void ProcessEvents()
        {
            if(Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (draggingNode != null)
                {
                    draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    Selection.activeObject = selectedDialogue;
                }
            }
            else if (draggingNode == null && Event.current.type == EventType.MouseDrag)
            {
                scrollPosition -= Event.current.delta;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                draggingNode.SetRectPosition(Event.current.mousePosition + draggingOffset);
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && draggingNode == null)
            {
                showContextMenu = true;
                contextMenuPosition = Event.current.mousePosition;
            }
            else
            {
                showContextMenu = false;
            }
        }

        private void DrawNode(DialogueNode node)
        {
            GUIStyle style = nodeStyle;
            if (node.IsPlayerSpeaking())
            {
                style = playerNodeStyle;
            }
            GUILayout.BeginArea(node.GetRect(), style);

            node.SetDialogueText(EditorGUILayout.TextField(node.GetDialogueText()));

            GUILayout.BeginHorizontal();

            if(selectedDialogue.GetNumberOfNodes() > 1)
            {
                if (GUILayout.Button("x"))
                {
                    deletingNode = node;
                }
            }
            LinkButton(node);
            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }

            GUILayout.EndHorizontal();

            if (node.IsPlayerSpeaking())
            {
                if (GUILayout.Button("Speaker: Player"))
                {
                    node.SetSpeaker(false);
                    style = nodeStyle;
                }
            }
            else
            {
                if (GUILayout.Button("Speaker: NPC"))
                {
                    node.SetSpeaker(true);
                    style = playerNodeStyle;
                }
            }

            GUILayout.EndArea();
        }

        private void LinkButton(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    linkingParentNode = node;
                }
            }
            else if (node == linkingParentNode)
            {
                if (GUILayout.Button("cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.GetChildrenNodes().Contains(node.name))
            {
                if (GUILayout.Button("unlink"))
                {
                    linkingParentNode.RemoveChildNode(node.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
                    linkingParentNode.AddChildNode(node.name);
                    linkingParentNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax,node.GetRect().center.y);
            foreach(DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector2(childNode.GetRect().xMin,childNode.GetRect().center.y);
                //scale curves based on distance
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= .8f;
                Handles.DrawBezier(startPosition, endPosition, 
                    startPosition + controlPointOffset, endPosition - controlPointOffset, 
                    Color.white,null,4f);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 mousePosition)
        {
            DialogueNode foundNode = null;
            foreach(DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if(node.GetRect().Contains(mousePosition))
                {
                    foundNode = node;
                }
            }
            return foundNode;
        }
        private void ShowContextMenu()
        {
            GenericMenu contextMenu = new GenericMenu();
            contextMenu.AddItem(new GUIContent("Add New Node"), false, () => selectedDialogue.CreateNode(null, contextMenuPosition));
            contextMenu.ShowAsContext();
        }
    }
}
