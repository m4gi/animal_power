using System.Collections.Generic;
using System.Linq;
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
        
        private List<string> skillPower = new List<string>();

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

        public void OnDragCardOnClicked(Vector3 mousePos, string id)
        {
            Debug.Log($"OnDragCardOnClicked {mousePos} - {id}");
            var ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out Lane lane))
                {
                    AnimalConfig cfg = CardManager.Instance.playerSlots.FirstOrDefault(x => x.animalName == id);
                    
                    if (summonManager.Summon(playerTeam, lane, cfg))
                    {
                        // CardManager.Instance.ConsumeAndShiftPlayer();

                        nextSummonAllowedTime = Time.time + globalCooldown;

                        UIManager.Instance.StartGlobalCooldown_Player(globalCooldown);
                    }
                    else
                    {
                        //UIManager.Instance.ShowCannotSpawn();
                        return;
                    }
                }
                else
                {
                    Debug.Log("OnDragCardOnClicked Fail! " + mousePos);
                }
            }
        }
    }
}