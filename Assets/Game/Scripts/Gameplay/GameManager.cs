using TMPro;
using Tuns.Base;
using UnityEngine;

namespace Game.Scripts
{
    public class GameManager : Singleton<GameManager>

    {
        public int playerHealth = 100;
        public int botHealth = 100;
        public TextMeshProUGUI playerHealthText;
        public TextMeshProUGUI botHealthText;

        private void Start()
        {
            UpdateUI();
        }

        public void DealDamageToBot(int damage)
        {
            botHealth -= damage;
            if (botHealth < 0) botHealth = 0;
            UpdateUI();
            CheckWin();
        }

        public void DealDamageToPlayer(int damage)
        {
            playerHealth -= damage;
            if (playerHealth < 0) playerHealth = 0;
            UpdateUI();
            CheckWin();
        }

        void UpdateUI()
        {
            if (playerHealthText != null)
                playerHealthText.text = playerHealth.ToString();
            if (botHealthText != null)
                botHealthText.text = botHealth.ToString();
        }

        void CheckWin()
        {
            if (playerHealth <= 0 || botHealth <= 0)
            {
                Debug.Log(playerHealth <= 0 ? "BOT WIN!" : "PLAYER WIN!");
            }
        }
    }
}