using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class EnergyManager : MonoBehaviour
    {
        public float maxEnergy = 10;
        public float currentEnergy = 0;
        public float rechargeRate = 0.5f;
        public Slider energyCostSlider;
        public TextMeshProUGUI  energyCostText;
        void Update()
        {
            currentEnergy += rechargeRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);

            if (energyCostSlider != null)
            {
                energyCostSlider.value = currentEnergy / maxEnergy;
            }

            if (energyCostText != null)
            {
                energyCostText.text = Mathf.FloorToInt(currentEnergy).ToString();
            }
            // UIManager.Instance.UpdateEnergy(currentEnergy, maxEnergy);
        }

        public bool HasEnough(int cost)
        {
            return currentEnergy >= cost;
        }

        public void Consume(float cost)
        {
            currentEnergy -= cost;
            if (currentEnergy < 0) currentEnergy = 0;
        }
    }
}