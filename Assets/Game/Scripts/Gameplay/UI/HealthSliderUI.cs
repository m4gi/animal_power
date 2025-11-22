using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class HealthSliderUI : MonoBehaviour
    {
        [Header("UI Ref")] 
        [SerializeField] private Slider _hpSlider;
        [SerializeField] private TextMeshProUGUI _hpText;

        [Header("Animation Settings")] 
        public float animDuration = 0.3f;
        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Coroutine animRoutine;

        public void InitSlider(int initHP)
        {
            _hpSlider.value = initHP;
            _hpSlider.maxValue = initHP;
        }
        
        public void SetHpSlider(float currentHp)
        {
            if (animRoutine != null)
                StopCoroutine(animRoutine);

            animRoutine = StartCoroutine(AnimateHp(currentHp));
        }

        private IEnumerator AnimateHp(float targetHp)
        {
            float startSliderValue = _hpSlider.value;
            float startHpNumber = startSliderValue;

            float time = 0f;

            while (time < animDuration)
            {
                time += Time.deltaTime;
                float t = time / animDuration;
                float easing = curve.Evaluate(t);

                _hpSlider.value = Mathf.Lerp(startSliderValue, targetHp, easing);

                float displayValue = Mathf.Lerp(startHpNumber, targetHp, easing);
                _hpText.text = Mathf.RoundToInt(displayValue).ToString();

                yield return null;
            }

            _hpSlider.value = targetHp;
            _hpText.text = Mathf.RoundToInt(targetHp).ToString();
        }
    }
}