using System;
using Game.Scripts;
using Magi.Scripts.GameData;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject firstPanel;
        [SerializeField] private GameObject secondPanel;
        
        [SerializeField] private Button startSplashArtButton;
        [SerializeField] private Button playGameButton;
        
        [SerializeField] private TextMeshProUGUI coinText;
        private LocalDataPlayer LocalData => LocalDataPlayer.Instance;
        
        void Start()
        {
            startSplashArtButton.onClick.AddListener(SplashArtOnClick);
            //playGameButton.onClick.AddListener(PlayGameOnClick);
            firstPanel.SetActive(LocalData.isFistTime);
            secondPanel.SetActive(!LocalData.isFistTime);

            LocalData.OnCoinChanged += CoinUpdate;
            CoinUpdate(LocalData.PlayerData.Coin);
            
            SoundSystem.Instance?.PlayMusic(MusicConst.MainMenuMusic);
        }

        private void OnDestroy()
        {
            LocalData.OnCoinChanged -= CoinUpdate;
        }

        private void SplashArtOnClick()
        {
            LocalData.isFistTime = false;
            firstPanel.SetActive(false);
            secondPanel.SetActive(true);
        }
        
        private void PlayGameOnClick()
        {
            SceneLoaderSystem.Instance.LoadScene(SceneConst.GameScene);
        }

        private void CoinUpdate(int coin)
        {
            if(coinText)
                coinText.text = StringExtensions.FormatNumber(coin);
        }
    }
}
