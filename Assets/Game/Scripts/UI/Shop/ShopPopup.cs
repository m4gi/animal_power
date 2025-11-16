using System;
using Game.Scripts.GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class ShopPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image iconImage;

        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private Button payButton;
        [SerializeField] private Button selectButton;

        private readonly string titleFormat = "<color=#ffd408>{0}</color>";

        private LocalDataPlayer LocalData => LocalDataPlayer.Instance;

        private Action ItemAction;
        private AnimalDatabaseLocal _animalDatabaseLocal;

        private void Start()
        {
            payButton.onClick.AddListener(PayButtonOnClick);
            selectButton.onClick.AddListener(SelectButtonOnClick);
        }

        private void PayButtonOnClick()
        {
            if (LocalData.TrySpendCoin(_animalDatabaseLocal.animalPrice))
            {
                LocalData.AddSkin(_animalDatabaseLocal.animalID);
                LocalData.SetCurrentSkin(_animalDatabaseLocal.animalID);
                ItemAction?.Invoke();
                GetComponent<UIAnimatedScale>()?.HideWithAnimation();
                return;
            }
            ToastMessageSystem.Show("Not enough money!");
        }

        private void SelectButtonOnClick()
        {
            LocalData.SetCurrentSkin(_animalDatabaseLocal.animalID);
            ItemAction?.Invoke();
            GetComponent<UIAnimatedScale>()?.HideWithAnimation();
        }

        public void InitData(AnimalDatabaseLocal data, Action callback)
        {
            _animalDatabaseLocal = data;
            ItemAction = callback;
            
            coinText.SetText($"{data.animalPrice}");
            bool isEnoughMoney = LocalData.CanSpendCoin(data.animalPrice);
            coinText.color = isEnoughMoney ? Color.white : Color.red;
            
            titleText.SetText(string.Format(titleFormat, data.animalName));
            iconImage.sprite = data.animalSprite;

            bool purchased = LocalData.HasSkin(data.animalID);

            payButton.gameObject.SetActive(!purchased);
            selectButton.gameObject.SetActive(purchased);
        }
    }
}