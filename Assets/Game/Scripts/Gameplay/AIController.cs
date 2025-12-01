using Game.Scripts.GameData;

namespace Game.Scripts
{
    using UnityEngine;

    public class AIController : MonoBehaviour
    {
        [Header("AI Settings")] public Team aiTeam = Team.B;
        public float summonCooldown = 2f;
        public float nextSummonTime = 0f;

        [Header("AI Behavior Weights")] public float dangerWeight = 2.5f;
        public float counterWeight = 2f;
        public float pushWeight = 1.5f;

        private SummonManager summon => SummonManager.Instance;
        private CardManager deck => CardManager.Instance;
        private LaneManager laneManager => LaneManager.Instance;

        void Update()
        {
            if (GameManager.Instance.gameEnded) return;

            if (Time.time < nextSummonTime)
                return;

            Lane selectedLane = ChooseSmartLane();
            if (selectedLane == null)
                return;

            AnimalConfig cfg = deck.aiSlots[0];

            // summon
            if (summon.Summon(aiTeam, selectedLane, cfg))
            {
                deck.ConsumeAndShiftAI();
                nextSummonTime = Time.time + summonCooldown;
                UIManager.Instance.StartGlobalCooldown_Enemy(summonCooldown);
            }
        }

        // -------------------------------------------------------
        // SMART LANE DECISION
        // -------------------------------------------------------
        Lane ChooseSmartLane()
        {
            Lane best = null;
            float bestScore = float.MinValue;

            foreach (var lane in laneManager.lanes)
            {
                if (lane.IsLaneLocked) 
                    continue;

                float sc = EvaluateLane(lane);
                if (sc > bestScore)
                {
                    bestScore = sc;
                    best = lane;
                }
            }

            return best;
        }


        float EvaluateLane(Lane lane)
        {
            if (lane.IsLaneLocked)
                return -99999f;

            float cz = lane.central.transform.position.z;

            float homeZ = (aiTeam == Team.A)
                ? lane.homeA.position.z
                : lane.homeB.position.z;

            float distToAIHome = Mathf.Abs(cz - homeZ);

            float myForce = lane.GetForce(aiTeam);
            Team enemy = (aiTeam == Team.A) ? Team.B : Team.A;
            float enemyForce = lane.GetForce(enemy);

            float danger = Mathf.Max(0f, 10f - distToAIHome);
            float enemyPush = Mathf.Max(0f, enemyForce - myForce);
            float advantage = Mathf.Max(0f, myForce - enemyForce);

            float netForce = myForce - enemyForce;

            // -----------------------------------------------------
            // 1) OVERKILL CHECK
            // -----------------------------------------------------
            const float OVERKILL_THRESHOLD = 7f;
            if (netForce >= OVERKILL_THRESHOLD)
                return -5000f; 

            // -----------------------------------------------------
            // 2) BALANCE BIAS
            // -----------------------------------------------------
            float balancePenalty = advantage * 0.5f; 

            // -----------------------------------------------------
            // 3) CLAMP ADVANTAGE
            // -----------------------------------------------------
            float clampedAdvantage = Mathf.Min(advantage, 5f);

            // -----------------------------------------------------
            // 4) FINAL SCORE
            // -----------------------------------------------------
            float score =
                danger * dangerWeight +
                enemyPush * counterWeight +
                clampedAdvantage * pushWeight;

            score -= balancePenalty;

            return score;
        }

    }
}