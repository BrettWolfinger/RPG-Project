    using System.Collections.Generic;
    using UnityEditor;
     
    namespace RPG.Quests.Editer
    {
        [CustomEditor(typeof(QuestCompletion))]
        public class QuestCompletionEditor:Editor
        {
            private SerializedProperty quest;
            private SerializedProperty objective;
     
            private void OnEnable()
            {
                quest = serializedObject.FindProperty("quest");
                objective = serializedObject.FindProperty("objective");
            }
     
            public override void OnInspectorGUI()
            {
                EditorGUILayout.PropertyField(quest);
                if (quest.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox("Please select a Quest to continue.", MessageType.Info);
                    return;
                }
                EditorGUILayout.PrefixLabel("Objective");
                List<string> references = new List<string>();
                List<string> descriptions= new List<string>();
                Quest selectedQuest = (Quest)quest.objectReferenceValue;
                {
                    foreach (Quest.Objective questObjective in selectedQuest.GetObjectives())
                    {
                        references.Add(questObjective.reference);
                        descriptions.Add(questObjective.description);
                    }
                }
                int selectedIndex = references.IndexOf(objective.stringValue);
                int testIndex = EditorGUILayout.Popup(selectedIndex, descriptions.ToArray());
                if (testIndex != selectedIndex)
                {
                    objective.stringValue = references[testIndex];
                }
                serializedObject.ApplyModifiedProperties();
            }
        }
    }