using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Scripts
{
    public class LevelSelectionPanel : MonoBehaviour
    {
        [SerializeField] private Transform contentTransform;
        [SerializeField] private LevelItem levelItemPrefab;

        [SerializeField] private TextMeshProUGUI totalText;

        private List<LevelItem> levelItems = new List<LevelItem>();

        private LocalDataPlayer LocalData => LocalDataPlayer.Instance;

        private void Awake()
        {
            if (levelItems.Count <= 0)
            {
                var levelData = LocalData.LevelDataConfigs.levels;
                for (int i = 0; i < levelData.Length; i++)
                {
                    var levelItem = Instantiate(levelItemPrefab, contentTransform);
                    bool isLocked = i > LocalData.PlayerData.CurrentLevel;
                    levelItem.InitItem(i, isLocked);
                    levelItems.Add(levelItem);
                }
            }
        }

        private void OnEnable()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            totalText.SetText($"{LocalData.PlayerData.CurrentLevel + 1}/{LocalData.LevelDataConfigs.levels.Length}");
        }
    }
}