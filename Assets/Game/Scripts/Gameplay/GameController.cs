using Game.Scripts.GameData;
using Tuns.Base;

namespace Game.Scripts
{
    using UnityEngine;

    public class GameController : Singleton<GameController>
    {
        public SummonManager summonManager;

        public Team playerTeam = Team.A;  
        public int selectedAnimalType = 0;
        
        public float globalCooldown = 2f;
        private float nextSummonAllowedTime = 0f;

        public void SetAnimalType(int typeID)
        {
            selectedAnimalType = typeID;
        }

        public void OnLaneClicked(Lane lane)
        {
            if (Time.time < nextSummonAllowedTime)
                return;
            
            AnimalConfig cfg = CardManager.Instance.playerSlots[0];

            if (summonManager.Summon(playerTeam, lane, cfg))
            {
                CardManager.Instance.ConsumeAndShiftPlayer();
                
                nextSummonAllowedTime = Time.time + globalCooldown;
                
                UIManager.Instance.StartGlobalCooldown_Player(globalCooldown);
            }
            else
            {
                //UIManager.Instance.ShowCannotSpawn();
                return;
            }
        }
    }

}