using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class LanePowerBar : MonoBehaviour
    {
        public Lane lane;

        public Slider powerSlider;

        public TextMeshProUGUI powerText;
        
        [Header("Animation Settings")]
        public float smoothSpeed = 5f;

        private float targetValue = 0.5f;
        
        void Update()
        {
            float forceA = lane.GetForceA();
            float forceB = lane.GetForceB();

            float total = forceA + forceB;
            powerSlider.gameObject.SetActive(total >= 0.01f);
            if (total < 0.01f)
            {
                powerSlider.value = 0.5f;
                return;
            }
            
            float powerRatio = forceA / total;

            targetValue = powerRatio;
            LerpValue();

            powerText.text = $"{forceA}/{forceB}";
        }
        
        void LerpValue()
        {
            powerSlider.value = Mathf.Lerp(powerSlider.value, targetValue, Time.deltaTime * smoothSpeed);
        }
    }
}