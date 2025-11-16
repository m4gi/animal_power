using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class SettingSliderItem : MonoBehaviour
    {
        [SerializeField] private Slider sliderItem;
        [SerializeField] private Image iconItem;
        [SerializeField] private Image handleImage;
        [Header("Status Config")]
        [SerializeField] private Sprite iconOnSprite;
        [SerializeField] private Sprite iconOffSprite;
        [SerializeField] private Sprite handleOnSprite;
        [SerializeField] private Sprite handleOffSprite;
        [SerializeField] private Color iconOnColor;
        [SerializeField] private Color iconOffColor;
        
        public Action<float> OnValueChanged;

        private float _lastValue;
        private const float VALUE_THRESHOLD = 0.01f;

        private void Awake()
        {
            sliderItem.onValueChanged.AddListener(OnValueChangeLocal);
        }

        private void OnDestroy()
        {
            sliderItem.onValueChanged.RemoveListener(OnValueChangeLocal);
        }

        public void InitValue(float value)
        {
            _lastValue = value;
            sliderItem.value = value;
            UpdateVisual(value);
        }

        private void OnValueChangeLocal(float value)
        {
            UpdateVisual(value);

            if (Mathf.Abs(value - _lastValue) < VALUE_THRESHOLD)
                return;

            _lastValue = value;
            OnValueChanged?.Invoke(value);
        }

        private void UpdateVisual(float value)
        {
            bool isOn = value > 0f;
            if(iconItem && iconOnSprite && iconOffSprite)
            {
                iconItem.sprite = isOn ? iconOnSprite : iconOffSprite;
                iconItem.color = isOn ? iconOnColor : iconOffColor;
            }
            handleImage.sprite = isOn ? handleOnSprite : handleOffSprite;
        }
    }
}
