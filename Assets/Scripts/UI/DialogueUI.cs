using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField] TextMeshProUGUI speakerName;
        [SerializeField] TextMeshProUGUI AIText;
        [SerializeField] Button nextButton;
        [SerializeField] Button quitButton;
        [SerializeField] GameObject npcResponseRoot;
        [SerializeField] Transform choiceRoot;
        [SerializeField] GameObject choicePrefab;

        private void Awake() {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
        }

        // Start is called before the first frame update
        void Start()
        {
            playerConversant.onConversationUpdated += UpdateUI;
            nextButton.onClick.AddListener(Next);
            quitButton.onClick.AddListener(Quit);

            UpdateUI();
        }

        void Next()
        {
            playerConversant.Next();
        }

        void Quit()
        {
            playerConversant.Quit();
        }

        void UpdateUI()
        {
            gameObject.SetActive(playerConversant.IsActive());
            if(!playerConversant.IsActive())
            {
                return;
            }
            speakerName.text = playerConversant.GetCurrentSpeakerName();
            npcResponseRoot.SetActive(!playerConversant.IsChoosing());
            choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());
            if(playerConversant.IsChoosing())
            {
                HandleChoiceList();
            }
            else
            {
                AIText.SetText(playerConversant.GetText());
                nextButton.gameObject.SetActive(playerConversant.HasNext());
            }
        }

        private void HandleChoiceList()
        {
            foreach (Transform choice in choiceRoot)
            {
                Destroy(choice.gameObject);
            }
            foreach (DialogueNode choice in playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                TextMeshProUGUI text = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                text.SetText(choice.GetDialogueText());
                Button button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() => 
                {
                    playerConversant.SelectChoice(choice);
                });
            }
        }
    }
}
