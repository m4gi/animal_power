using System;
using Magi.Scripts.GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public enum MatchResult
    {
        Win,
        Lose,
        Draw
    }
    
    public class ResultPanelUI : MonoBehaviour
    {
        [SerializeField] private GameObject victoryUI;
        [SerializeField] private GameObject defeatUI;
        [SerializeField] private GameObject drawUI;
        
        [SerializeField] private Image frameReward;
        [SerializeField] private Color winColor;
        [SerializeField] private Color defeatColor;
        
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private Button continueButton;

        [SerializeField] private int winTotalCoin = 2000;
        [SerializeField] private int defeatTotalCoin = 500;
        [SerializeField] private int drawTotalCoin = 1000;
        

        private LocalDataPlayer LocalData => LocalDataPlayer.Instance;
        private void Start()
        {
            continueButton.onClick.AddListener(ContinueOnClick);
        }

        private void ContinueOnClick()
        {
            SceneLoaderSystem.Instance.LoadScene(SceneConst.MainScene);
        }

        public void Show(MatchResult matchResult)
        {
            victoryUI.SetActive(false);
            defeatUI.SetActive(false);
            drawUI.SetActive(false);
            int coin = 0;
            switch (matchResult)
            {
                case MatchResult.Win:
                    victoryUI.SetActive(true);
                    coin = winTotalCoin;
                    LocalData.UnlockNextLevel();
                    break;
                case MatchResult.Lose:
                    defeatUI.SetActive(true);
                    coin = defeatTotalCoin;
                    break;
                case MatchResult.Draw:
                    drawUI.SetActive(true);
                    coin = drawTotalCoin;
                    break;
            }
            frameReward.color = matchResult != MatchResult.Lose ? winColor : defeatColor;
            coinText.text = coin.ToString();
            
            LocalData.AddCoin(coin);
        }
    }
}