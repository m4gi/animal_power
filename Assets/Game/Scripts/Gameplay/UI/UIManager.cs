using Game.Scripts.GameData;
using TMPro;
using Tuns.Base;
using UnityEngine;

namespace Game.Scripts
{
    public class UIManager : Singleton<UIManager>
    {
        [Header("Card Slots")]
        public CardInfo[] playerCardSlots;
        public CardInfo[] enemyCardSlots;
        
        [Header("UI")]
        public TextMeshProUGUI timerText;
        public HealthSliderUI playerHealthSlider;
        public HealthSliderUI enemyHealthSlider;
        
        [Header("Settings")]
        public float cooldownAnimSpeed = 8f;
        
        [Header("Energy Manager")]
        public EnergyManager playerEnergyManager;

        private float globalCooldownDurationPlayer = 0f;
        private float globalCooldownEndTimePlayer = 0f;
        
        private float globalCooldownDurationEnemy = 0f;
        private float globalCooldownEndTimeEnemy = 0f;
        
        private void Update()
        {
            UpdateCooldownUI_Player();
            UpdateUIActive_Player();
            UpdateCooldownUI_Enemy();
        }

        private void UpdateUIActive_Player()
        {
            int currentEnergy = Mathf.FloorToInt(playerEnergyManager.currentEnergy);
            foreach (CardInfo cardInfo in playerCardSlots)
            {
                cardInfo.SetStateEffect(currentEnergy);
            }
        }

        public void UpdatePlayerCardUI_Player(AnimalConfig[] slots)
        {
            for (int i = 0; i < playerCardSlots.Length; i++)
            {
                if (slots[i] == null) continue;

                playerCardSlots[i].InitCard(slots[i]);;
            }
        }
        
        public void UpdatePlayerCardUI_Enemy(AnimalConfig[] slots)
        {
            for (int i = 0; i < 3; i++)
            {
                if (slots[i] == null) continue;

                enemyCardSlots[i].InitCard(slots[i]);;
            }
        }
        
        public void StartGlobalCooldown_Player(float duration)
        {
            globalCooldownDurationPlayer = duration;
            globalCooldownEndTimePlayer = Time.time + duration;

            if (playerCardSlots[0] != null)
                playerCardSlots[0].UpdateCooldownMask(1);
        }
        
        public void StartGlobalCooldown_Enemy(float duration)
        {
            globalCooldownDurationEnemy = duration;
            globalCooldownEndTimeEnemy = Time.time + duration;

            if (enemyCardSlots[0] != null)
                enemyCardSlots[0].UpdateCooldownMask(1);
        }
        
        void UpdateCooldownUI_Enemy()
        {
            if (enemyCardSlots[0] == null) return;

            if (Time.time >= globalCooldownEndTimeEnemy)
            {
                enemyCardSlots[0].UpdateCooldownMask(0);
                return;
            }

            float remain = globalCooldownEndTimeEnemy - Time.time;
            float ratio = remain / globalCooldownDurationEnemy;
            
            enemyCardSlots[0].UpdateCooldownMask(Mathf.Lerp(
                enemyCardSlots[0].GetAmount(),
                ratio,
                Time.deltaTime * cooldownAnimSpeed
            ));
        }
        
        void UpdateCooldownUI_Player()
        {
            if (playerCardSlots[0] == null) return;

            if (Time.time >= globalCooldownEndTimePlayer)
            {
                playerCardSlots[0].UpdateCooldownMask(0);
                return;
            }

            float remain = globalCooldownEndTimePlayer - Time.time;
            float ratio = remain / globalCooldownDurationPlayer;
            
            playerCardSlots[0].UpdateCooldownMask(Mathf.Lerp(
                playerCardSlots[0].GetAmount(),
                ratio,
                Time.deltaTime * cooldownAnimSpeed
            ));
        }
        
        public void UpdateTimer(float seconds)
        {
            int m = Mathf.FloorToInt(seconds / 60);
            int s = Mathf.FloorToInt(seconds % 60);
            timerText.text = $"Time:\n{m:00}:{s:00}";
        }

        public void InitHP(int initialHP)
        {
            playerHealthSlider.InitSlider(initialHP);
            enemyHealthSlider.InitSlider(initialHP);
        }
        
        public void UpdateHP(int pHP, int eHP)
        {
            playerHealthSlider.SetHpSlider(pHP);
            enemyHealthSlider.SetHpSlider(eHP);
        }
    }
}