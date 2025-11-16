using System;
using System.Collections.Generic;
using Game.Scripts.GameData;
using TMPro;
using UnityEngine;

namespace Game.Scripts
{
    public class ShopPanel : MonoBehaviour
    {
        [SerializeField] private Transform shopContentTransform;
        [SerializeField] private ShopItem shopItemPrefab;

        [SerializeField] private TextMeshProUGUI totalText;

        [SerializeField] private ShopPopup shopPopup;

        private List<ShopItem> shopItems = new List<ShopItem>();

        private LocalDataPlayer LocalData => LocalDataPlayer.Instance;

        private void Awake()
        {
            if (shopItems.Count <= 0)
            {
                var shopData = LocalData.AnimalDataLocal;
                foreach (AnimalDatabaseLocal item in shopData)
                {
                    var shopItem = Instantiate(shopItemPrefab, shopContentTransform);
                    bool purchased = LocalData.HasSkin(item.animalID);
                    bool isSelected = LocalData.GetCurrentSkin() == item.animalID;
                    shopItem.InitShopItem(item, purchased, isSelected, OnClickAction);
                    shopItems.Add(shopItem);
                }
            }
        }

        private void OnEnable()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            totalText.SetText($"{LocalData.PlayerData.Skins.Count}/{LocalData.AnimalDataLocal.Length}");
        }

        private void OnClickAction(AnimalDatabaseLocal data)
        {
            shopPopup.gameObject.SetActive(true);
            shopPopup.InitData(data, OnShopPopupCallback);
        }

        private void OnShopPopupCallback()
        {
            UpdateText();
            foreach (ShopItem shopItem in shopItems)
            {
                shopItem.ForceUpdateItem();
            }
        }
    }
}