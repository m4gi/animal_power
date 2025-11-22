using System;
using Game.Scripts.GameData;
using Magi.Scripts.GameData;
using Tuns.Base;
using UnityEngine;


namespace Game.Scripts
{
    public class GameManager : Singleton<GameManager>
    {
        [Header("Team Settings")] public int maxHP = 100;
        public int hpPerGoal = 10;

        public int teamAHP;
        public int teamBHP;

        [Header("Lanes")] public Lane[] lanes;

        [Header("Game State")] public bool gameEnded = false;

        [Header("Match Settings")] public float matchDuration = 300f;
        private float timeRemaining;
        
        [Header("Database Settings")]
        public CardManager cardManager;
        
        [Header("UI")]
        public ResultPanelUI resultPanel;
        
        private LocalDataPlayer LocalData => LocalDataPlayer.Instance;

        protected override void AwakeSingleton()
        {
            base.AwakeSingleton();
            // if (LocalDataPlayer.Instance != null)
            // {
            //     var data =  LocalDataPlayer.Instance.GetCurrentAnimalData();
            //     cardManager.playerDeck = data.animalData;
            //     cardManager.aiDeck = data.animalData;
            // }
            if (LocalData != null)
            {
                var dataLevel = LocalData.GetCurrentLevelData();
                maxHP = dataLevel.maxHealth * hpPerGoal;
                matchDuration = dataLevel.time;
            }
            ResetHP();
        }

        private void Start()
        {
            timeRemaining = matchDuration;
            UIManager.Instance.InitHP(maxHP);
            UIManager.Instance.UpdateHP(teamAHP, teamBHP);
            UIManager.Instance.UpdateTimer(timeRemaining);
            
            SoundSystem.Instance?.PlayMusic(MusicConst.MainGameMusic);
        }

        void ResetHP()
        {
            teamAHP = maxHP;
            teamBHP = maxHP;
        }

        void Update()
        {
            if (gameEnded) return;

            foreach (var lane in lanes)
            {
                if (lane == null || lane.central == null) continue;

                float cz = lane.central.transform.position.z;

                if (cz <= lane.homeB.position.z + lane.goalThreshold)
                {
                    ApplyGoal(Team.A, lane);
                    return;
                }

                if (cz >= lane.homeA.position.z - lane.goalThreshold)
                {
                    ApplyGoal(Team.B, lane);
                    return;
                }
            }

            timeRemaining -= Time.deltaTime;
            UIManager.Instance.UpdateTimer(timeRemaining);


            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                EndMatchByTimer();
            }
        }

        // ------------------------------------------------------
        // APPLY DAMAGE + RESET LANE
        // ------------------------------------------------------
        void ApplyGoal(Team loser, Lane lane)
        {
            if (loser == Team.A)
            {
                teamAHP -= hpPerGoal;
                Debug.Log($"Team A takes damage! HP = {teamAHP}");
            }
            else
            {
                teamBHP -= hpPerGoal;
                Debug.Log($"Team B takes damage! HP = {teamBHP}");
            }
            
            SoundSystem.Instance?.PlaySFX(SFXConst.HitBase);
            lane.ResetLane();

            if (teamAHP <= 0 || teamBHP <= 0)
            {
                EndMatch();
            }

            UIManager.Instance?.UpdateHP(teamAHP, teamBHP);
        }

        // ------------------------------------------------------
        // END MATCH
        // ------------------------------------------------------
        void EndMatch()
        {
            gameEnded = true;
            string winner = (teamAHP <= 0) ? "TEAM B" : "TEAM A";
            MatchResult matchResult = teamAHP > 0 ? MatchResult.Win : MatchResult.Lose;
            Debug.Log($"GAME OVER! WINNER: {winner}");
            resultPanel.gameObject.SetActive(true);
            resultPanel.Show(matchResult);
        }

        void EndMatchByTimer()
        {
            gameEnded = true;
            resultPanel.gameObject.SetActive(true);
            if (teamAHP > teamBHP)
            {
                // UIManager.Instance.ShowMatchResult("YOU WIN!");
                resultPanel.Show(MatchResult.Win);
            }
            else if (teamBHP > teamAHP)
            {
                //UIManager.Instance.ShowMatchResult("YOU LOSE!");
                resultPanel.Show(MatchResult.Lose);
            }
            else
            {
                //UIManager.Instance.ShowMatchResult("DRAW!");
                resultPanel.Show(MatchResult.Draw);
            }
        }

        // ------------------------------------------------------
        // RESTART MATCH
        // ------------------------------------------------------
        public void RestartMatch()
        {
            ResetHP();
            gameEnded = false;

            foreach (var lane in lanes)
            {
                if (lane != null)
                    lane.ResetLane();
            }
        }
    }
}