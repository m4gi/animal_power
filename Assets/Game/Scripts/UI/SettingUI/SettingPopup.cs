using Game.Scripts;
using UnityEngine;

public class SettingPopup : MonoBehaviour
{
    
    [SerializeField] private SettingSliderItem musicSlider;
    [SerializeField] private SettingSliderItem sfxSlider;

    private LocalDataPlayer LocalData => LocalDataPlayer.Instance;

    void Start()
    {
        musicSlider.OnValueChanged += OnMusicSliderValueChange;
        sfxSlider.OnValueChanged += OnSfxSliderValueChange;
    }

    private void OnEnable()
    {
        float musicValue = LocalData.PlayerData.MusicVolume;
        float sfxValue = LocalData.PlayerData.SfxVolume;
        
        musicSlider.InitValue(musicValue);
        sfxSlider.InitValue(sfxValue);
    }

    private void OnDisable()
    {
        LocalData.SaveData();
    }
    
    #region Slider Setting
    
    private void OnMusicSliderValueChange(float value)
    {
        LocalData.SetMusicVolume(value);
    }
    
    private void OnSfxSliderValueChange(float value)
    {
        LocalData.SetSfxVolume(value);
    }

    #endregion 
}