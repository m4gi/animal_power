using System;
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

        protected override void AwakeSingleton()
        {
            base.AwakeSingleton();
            ResetHP();
        }

        private void Start()
        {
            timeRemaining = matchDuration;

            UIManager.Instance.UpdateHP(teamAHP, teamBHP);
            UIManager.Instance.UpdateTimer(timeRemaining);
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

            lane.ResetLane();

            if (teamAHP <= 0 || teamBHP <= 0)
            {
                EndMatch();
            }

            UIManager.Instance.UpdateHP(teamAHP, teamBHP);
        }

        // ------------------------------------------------------
        // END MATCH
        // ------------------------------------------------------
        void EndMatch()
        {
            gameEnded = true;
            string winner = (teamAHP <= 0) ? "TEAM B" : "TEAM A";

            Debug.Log($"GAME OVER! WINNER: {winner}");
        }

        void EndMatchByTimer()
        {
            gameEnded = true;

            if (teamAHP > teamBHP)
            {
                // UIManager.Instance.ShowMatchResult("YOU WIN!");
            }
            else if (teamBHP > teamAHP)
            {
                //UIManager.Instance.ShowMatchResult("YOU LOSE!");
            }
            else
            {
                //UIManager.Instance.ShowMatchResult("DRAW!");
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