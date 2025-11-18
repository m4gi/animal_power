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

        public void InitCard(AnimalConfig config)
        {
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