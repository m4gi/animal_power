using System;
using Magi.Scripts.GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class LevelItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private Button levelButton;
        [SerializeField] private GameObject lockItem;
        [SerializeField] private Image bgImage;
        [SerializeField] private Sprite lockedSprite;
        [SerializeField] private Sprite unlockedSprite;

        private int _level;
        private bool _isLocked;

        private void Start()
        {
            levelButton.onClick.AddListener(LevelItemOnClick);
        }

        public void InitItem(int level, bool isLocked)
        {
            _level = level;
            _isLocked = isLocked;
            
            levelText.text = $"{level + 1}";
            lockItem.SetActive(isLocked);
            bgImage.sprite = isLocked ? lockedSprite : unlockedSprite;
        }

        private void LevelItemOnClick()
        {
            if (_isLocked) return;

            LocalDataPlayer.Instance.currentLevel = _level;
            SceneLoaderSystem.Instance.LoadScene(SceneConst.GameScene);
        }
    }
}