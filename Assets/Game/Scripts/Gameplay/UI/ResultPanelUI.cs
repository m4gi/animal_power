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
        [SerializeField] private Button replayButton;
        [SerializeField] private Button nextButton;

        [SerializeField] private int winTotalCoin = 2000;
        [SerializeField] private int defeatTotalCoin = 500;
        [SerializeField] private int drawTotalCoin = 1000;
        

        private LocalDataPlayer LocalData => LocalDataPlayer.Instance;
        private void Start()
        {
            replayButton.onClick.AddListener(ReplayOnClick);
            continueButton.onClick.AddListener(ContinueOnClick);
            nextButton.onClick.AddListener(NextOnClick);
        }
        
        private void NextOnClick()
        {
            LocalDataPlayer.Instance.currentLevel++;
            SceneLoaderSystem.Instance.LoadScene(SceneConst.GameScene);
        }

        private void ReplayOnClick()
        {
            SceneLoaderSystem.Instance.LoadScene(SceneConst.GameScene);
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
            replayButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
            int coin = 0;
            switch (matchResult)
            {
                case MatchResult.Win:
                    victoryUI.SetActive(true);
                    coin = winTotalCoin;
                    LocalData.UnlockNextLevel();
                    nextButton.gameObject.SetActive(true);
                    break;
                case MatchResult.Lose:
                    defeatUI.SetActive(true);
                    coin = defeatTotalCoin;
                    replayButton.gameObject.SetActive(true);
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