using Game.Scripts.GameData;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class CardInfo : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private GameObject[] groupStar;
        [SerializeField] private Image cooldownMask;

        public void InitCard(AnimalConfig config)
        {
            itemImage.sprite = config.animalSprite;
            for (int i = 0; i < groupStar.Length; i++)
            {
                groupStar[i].SetActive(i < config.animalLevel);
            }
        }

        public void UpdateCooldownMask(float amount)
        {
            cooldownMask.fillAmount = amount;   
        }

        public float GetAmount()
        {
            return cooldownMask.fillAmount;
        }
    }
}