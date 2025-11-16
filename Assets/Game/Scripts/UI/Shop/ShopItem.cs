using System;
using Game.Scripts.GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class ShopItem : MonoBehaviour
    {
        [Header("Background")]
        public Image backgroundImage;
        public Sprite activeSprite;
        public Sprite inactiveSprite;
        
        [Header("Character")]
        public Image characterImage;
        public Color characterActiveColor;
        public Color characterInactiveColor;
        
        [Header("Text")]
        public TextMeshProUGUI itemName;
        public Color itemNameActiveColor;
        public Color itemNameInactiveColor;

        [Header("Status")] 
        public GameObject lockIcon;
        public GameObject checkIcon;
        
        public Button itemButton;

        private string _itemId;
        private AnimalDatabaseLocal _animalDatabaseLocal;
        
        public string ItemId => _itemId;

        private Action<AnimalDatabaseLocal> onClickAction;

        private void Start()
        {
            itemButton.onClick.AddListener(OnItemClick);
        }

        public void InitShopItem(
            AnimalDatabaseLocal item, 
            bool purchased, 
            bool isSelected, 
            Action<AnimalDatabaseLocal> action)
        {
            _animalDatabaseLocal = item;
            _itemId = item.animalID;
            onClickAction = action;

            characterImage.sprite = item.animalSprite;
            itemName.text = item.animalName;

            backgroundImage.sprite = purchased ? activeSprite : inactiveSprite;
            characterImage.color   = purchased ? characterActiveColor : characterInactiveColor;
            itemName.color         = purchased ? itemNameActiveColor : itemNameInactiveColor;

            lockIcon.SetActive(!purchased);
            checkIcon.SetActive(isSelected);
        }

        public void ForceUpdateItem()
        {
            var localData = LocalDataPlayer.Instance;
            var item = _animalDatabaseLocal;
            characterImage.sprite = item.animalSprite;
            itemName.text = item.animalName;
            bool purchased = localData.HasSkin(item.animalID);
            bool isSelected = localData.GetCurrentSkin() == item.animalID;
            backgroundImage.sprite = purchased ? activeSprite : inactiveSprite;
            characterImage.color   = purchased ? characterActiveColor : characterInactiveColor;
            itemName.color         = purchased ? itemNameActiveColor : itemNameInactiveColor;

            lockIcon.SetActive(!purchased);
            checkIcon.SetActive(isSelected);
        }
        
        private void OnItemClick()
        {
            onClickAction?.Invoke(_animalDatabaseLocal);
        }
    }
}