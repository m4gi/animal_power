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
        [SerializeField] private ObjectSettings objectSettings;
        [SerializeField] private UIEffect uiEffect;

        private int costEnergy;

        private bool tempState = false;

        public void InitCard(AnimalConfig config)
        {
            costEnergy = config.animalLevel;
            cooldownMask.fillAmount = 0;
            itemImage.sprite = config.animalSprite;
            for (int i = 0; i < groupStar.Length; i++)
            {
                groupStar[i].SetActive(i < config.animalLevel);
            }

            energyCostText.text = $"{config.animalLevel}";
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
    }
}