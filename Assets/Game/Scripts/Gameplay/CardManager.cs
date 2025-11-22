using Game.Scripts.GameData;
using Tuns.Base;
using UnityEngine;

namespace Game.Scripts
{
    public class CardManager : Singleton<CardManager>
    {
        [Header("Deck Source")]
        public AnimalData playerDeck;
        public AnimalData aiDeck;

        [Header("Player Slots")]
        public AnimalConfig[] playerSlots = new AnimalConfig[5];

        [Header("AI Slots")]
        public AnimalConfig[] aiSlots = new AnimalConfig[3];

        private void Start()
        {
            InitPlayerSlots();
            InitAISlots();
        }

        // -------------------------
        // INITIAL SETUP
        // -------------------------
        void InitPlayerSlots()
        {
            for (int i = 0; i < playerSlots.Length; i++)
                playerSlots[i] = GetPlayerDeck(i);

            UIManager.Instance.UpdatePlayerCardUI_Player(playerSlots);
        }

        void InitAISlots()
        {
            for (int i = 0; i < 3; i++)
                aiSlots[i] = GetRandomFromAIDeck();
            
            UIManager.Instance.UpdatePlayerCardUI_Enemy(aiSlots);
        }

        public AnimalConfig GetPlayerDeck(int index)
        {
            return playerDeck.animals[index];
        }
        
        // -------------------------
        // RANDOM
        // -------------------------
        public AnimalConfig GetRandomFromPlayerDeck()
        {
            return playerDeck.animals[Random.Range(0, playerDeck.animals.Length)];
        }

        public AnimalConfig GetRandomFromAIDeck()
        {
            return aiDeck.animals[Random.Range(0, aiDeck.animals.Length)];
        }

        // -------------------------
        // PLAYER SHIFT
        // -------------------------
        public AnimalConfig ConsumeAndShiftPlayer()
        {
            AnimalConfig chosen = playerSlots[0];

            playerSlots[0] = playerSlots[1];
            playerSlots[1] = playerSlots[2];
            playerSlots[2] = GetRandomFromPlayerDeck();

            UIManager.Instance.UpdatePlayerCardUI_Player(playerSlots);

            return chosen;
        }

        // -------------------------
        // AI SHIFT
        // -------------------------
        public AnimalConfig ConsumeAndShiftAI()
        {
            AnimalConfig chosen = aiSlots[0];

            aiSlots[0] = aiSlots[1];
            aiSlots[1] = aiSlots[2];
            aiSlots[2] = GetRandomFromAIDeck();

            UIManager.Instance.UpdatePlayerCardUI_Enemy(aiSlots);
            return chosen;
        }
    }
}
