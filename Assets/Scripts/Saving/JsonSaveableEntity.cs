using System;
using System.Collections.Generic;
using RPG.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Newtonsoft.Json.Linq;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class JsonSaveableEntity : MonoBehaviour 
    {
        [SerializeField] string uniqueIdentifier = "";
        static Dictionary<string, JsonSaveableEntity> globalLookup = new Dictionary<string, JsonSaveableEntity>();
        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        public JToken CaptureAsJtoken()
        {
            JObject state = new JObject();
            IDictionary<string,JToken> stateDict = state;
            foreach (IJsonSaveable jsonsaveable in GetComponents<IJsonSaveable>())
            {
                JToken token = jsonsaveable.CaptureAsJToken();
                string component = jsonsaveable.GetType().ToString();
                Debug.Log($"{name} Capture {component} = {token.ToString()}");
                stateDict[component] = token;
            }
            return state;
        }

        public void RestoreFromJToken(JToken s)
        {
            JObject state = s.ToObject<JObject>();
            IDictionary<string,JToken> stateDict = state;
            foreach (IJsonSaveable jsonsaveable in GetComponents<IJsonSaveable>())
            {
                string component = jsonsaveable.GetType().ToString();
                if(stateDict.ContainsKey(component))
                {
                    Debug.Log($"{name} Restore {component} =>{stateDict[component].ToString()}");
                    jsonsaveable.RestoreFromJToken(stateDict[component]);
                }
            }
        }
#if UNITY_EDITOR
        private void Update() 
        {
            if(Application.IsPlaying(gameObject)) return;
            if(string.IsNullOrEmpty(gameObject.scene.path)) return;
            
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");
            
            if(string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            //globalLookup[property.stringValue] = this;
        }
#endif
        private bool IsUnique(string candidate)
        {
            if(!globalLookup.ContainsKey(candidate)) return true;
            if(globalLookup[candidate] == this) return true;

            //deleted game object that's still in the dictionary
            if(globalLookup[candidate] == null)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            //identifier has changed while the previous one still exists
            if(globalLookup[candidate].GetUniqueIdentifier() != candidate)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            return false;
        }
    }
}