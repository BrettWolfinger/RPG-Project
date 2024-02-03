using System.Collections;
using System.Collections.Generic;
using RPG.Quests;
using UnityEngine;

public class QuestListUI : MonoBehaviour
{
    [SerializeField] QuestItemUI questPrefab;
    QuestList questList;

    void Awake() 
    {
        questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();    
    }
    void OnEnable() {
        questList.onNewQuestAdded+=RedrawQuests;
        questList.onObjectiveCompleted+=RedrawQuests;
        RedrawQuests();
    }
    void OnDisable() {
        questList.onNewQuestAdded-=RedrawQuests;
        questList.onObjectiveCompleted-=RedrawQuests;
    }

    private void RedrawQuests()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        foreach (QuestStatus status in questList.GetStatuses()){
            QuestItemUI quInstance = Instantiate<QuestItemUI>(questPrefab, transform);
            quInstance.Setup(status);
        }
    }
}
