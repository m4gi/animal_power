using System;
using Game.Scripts;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject splashArtGo;
        [SerializeField] private Button startButton;
        
        [SerializeField] private TextMeshProUGUI coinText;
        private LocalDataPlayer LocalData => LocalDataPlayer.Instance;
        
        void Start()
        {
            startButton.onClick.AddListener(SplashArtOnClick);
            splashArtGo.SetActive(LocalData.isFistTime);

            LocalData.OnCoinChanged += CoinUpdate;
            CoinUpdate(LocalData.PlayerData.Coin);
            //Debug.Log(JsonConvert.SerializeObject(LocalData.levelMissionData));
            
            SoundSystem.Instance.PlayMusic("main_menu_music");
        }

        private void OnDestroy()
        {
            LocalData.OnCoinChanged -= CoinUpdate;
        }

        private void SplashArtOnClick()
        {
            LocalData.isFistTime = false;
            splashArtGo.SetActive(false);
        }

        private void CoinUpdate(int coin)
        {
            if(coinText)
                coinText.text = StringExtensions.FormatNumber(coin);
        }
    }
}
