using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "NewDialogue", menuName = "RPG/Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<DialogueNode> nodes = new List<DialogueNode>();
        Vector2 newNodeOffset = new Vector2(250,0);
        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

#if UNITY_EDITOR
        private void Awake() 
        {
            if(nodes.Count == 0)
            {
                CreateNode(null);
            }

            OnValidate();
        }
#endif

        private void OnValidate() {
            nodeLookup.Clear();
            foreach (DialogueNode node in GetAllNodes())
            {
                nodeLookup[node.name] = node;
            }
        }
        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes[0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach(string childID in parentNode.GetChildrenNodes())
            {
                if(nodeLookup.ContainsKey(childID))
                {
                    yield return nodeLookup[childID];
                }
            }
        }

        public int GetNumberOfNodes()
        {
            return nodes.Count;
        }
#if UNITY_EDITOR
        public void CreateNode(DialogueNode parent, Vector2 position)
        {
            DialogueNode childNode = CreateInstance<DialogueNode>();
            childNode.name = System.Guid.NewGuid().ToString();
            Undo.RegisterCreatedObjectUndo(childNode, "Created Dialogue Node");
            if (parent != null)
            {
                parent.AddChildNode(childNode.name);
                childNode.SetRectPosition(parent.GetRect().position + newNodeOffset);
            }
            if(position != Vector2.zero)
            {
                childNode.SetRectPosition(position);
            }
            Undo.RecordObject(this, "Added Dialogue Node");
            nodes.Add(childNode);
            OnValidate();
        }

        public void CreateNode(DialogueNode parent)
        {
            CreateNode(parent,Vector2.zero);
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanOrphans(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void CleanOrphans(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChildNode(nodeToDelete.name);
            }
        }
#endif
        //about to save file to harddrive
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if(AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if(AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize()
        {
        }
    }
}