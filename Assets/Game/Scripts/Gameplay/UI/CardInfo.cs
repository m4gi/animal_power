using System;
using Coffee.UIEffects;
using Game.Scripts.GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class CardInfo : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private GameObject[] groupStar;
        [SerializeField] private Image cooldownMask;
        [SerializeField] private TextMeshProUGUI energyCostText;
        [SerializeField] private TextMeshProUGUI powerText;
        [SerializeField] private ObjectSettings objectSettings;
        [SerializeField] private UIEffect uiEffect;
        [SerializeField] private Button helpButton;
        
        private int costEnergy;

        private bool tempState = false;
        
        private AnimalConfig _animalConfig;

        private void Start()
        {
            helpButton.onClick.AddListener(ShowHintCard);
        }

        private void OnDestroy()
        {
            helpButton.onClick.RemoveListener(ShowHintCard);
        }

        public void InitCard(AnimalConfig config)
        {
            _animalConfig = config;
            costEnergy = config.animalLevel;
            cooldownMask.fillAmount = 0;
            itemImage.sprite = config.animalSprite;
            for (int i = 0; i < groupStar.Length; i++)
            {
                groupStar[i].SetActive(i < config.animalLevel);
            }

            energyCostText.text = $"{config.animalLevel}";
            powerText.text = $"{config.strength}";
            if (objectSettings != null)
                objectSettings.Id = config.animalName;
        }

        public void SetStateEffect(int currentEnergy)
        {
            bool isActive = currentEnergy >= costEnergy;
            if (isActive == tempState) return;
            tempState = isActive;

            if (uiEffect != null)
            {
                uiEffect.enabled = isActive;
            }
        }

        public void UpdateCooldownMask(float amount)
        {
            //cooldownMask.fillAmount = amount;
        }

        public float GetAmount()
        {
            return cooldownMask.fillAmount;
        }

        private void ShowHintCard()
        {
            UIManager.Instance.ShowCardDetail(_animalConfig);
        }

        public void SetActiveHelpButton(bool active)
        {
            helpButton.gameObject.SetActive(active);
        }
    }
}