using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class LevelSelectionPanel : MonoBehaviour
    {
        [SerializeField] private Transform contentTransform;
        [SerializeField] private LevelItem levelItemPrefab;

        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI totalText;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;

        [SerializeField] private GameObject[] chapterGameObjects;

        public List<LevelItem> levelItems = new List<LevelItem>();

        private LocalDataPlayer LocalData => LocalDataPlayer.Instance;
        private int currentChapter = 0;

        private void Awake()
        {
            var levelData = LocalData.LevelDataConfigs.levels;
            if (levelItems.Count <= 0)
            {
                for (int i = 0; i < levelData.Length; i++)
                {
                    var levelItem = Instantiate(levelItemPrefab, contentTransform);
                    bool isLocked = i > LocalData.PlayerData.CurrentLevel;
                    levelItem.InitItem(i, isLocked);
                    levelItems.Add(levelItem);
                }
            }
            else
            {
                for (int i = 0; i < levelItems.Count; i++)
                {
                    var levelItem = levelItems[i];
                    if (i < levelData.Length)
                    {
                        bool isLocked = i > LocalData.PlayerData.CurrentLevel;
                        levelItem.InitItem(i, isLocked);
                        levelItem.gameObject.SetActive(true);
                    }
                    else
                    {
                        levelItem.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void OnEnable()
        {
            UpdateText();

            ShowCurrentChapter();

            previousButton.onClick.AddListener(PreviousButtonOnClick);
            nextButton.onClick.AddListener(NextButtonOnClick);
        }

        private void UpdateText()
        {
            totalText.SetText($"{LocalData.PlayerData.CurrentLevel + 1}/{levelItems.Count}");
        }

        private void PreviousButtonOnClick()
        {
            if (currentChapter <= 0)
                return;

            currentChapter--;
            ShowCurrentChapter();
        }

        private void NextButtonOnClick()
        {
            if (currentChapter >= chapterGameObjects.Length - 1)
                return;

            currentChapter++;
            ShowCurrentChapter();
        }

        private void ShowCurrentChapter()
        {
            for (int i = 0; i < chapterGameObjects.Length; i++)
            {
                chapterGameObjects[i].SetActive(i == currentChapter);
            }

            previousButton.gameObject.SetActive(currentChapter > 0);
            nextButton.gameObject.SetActive(currentChapter < chapterGameObjects.Length - 1);

            titleText.SetText($"Chapter {currentChapter + 1}");
        }
    }
}