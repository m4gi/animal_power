using Game.Scripts.GameData;

namespace Game.Scripts
{
    using UnityEngine;

    public enum AIDifficulty
    {
        Easy,
        Normal,
        Hard
    }

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

            // card slot 0
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
            Lane bestLane = null;
            float bestScore = float.MinValue;

            foreach (Lane lane in laneManager.lanes)
            {
                float score = EvaluateLane(lane);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestLane = lane;
                }
            }

            return bestLane;
        }

        float EvaluateLane(Lane lane)
        {
            float cz = lane.central.transform.position.z;

            float homeZ = (aiTeam == Team.A)
                ? lane.homeA.position.z
                : lane.homeB.position.z;

            float distToAIHome = Mathf.Abs(cz - homeZ);

            float myForce = lane.GetForce(aiTeam);
            float enemyForce = lane.GetForce(aiTeam == Team.A ? Team.B : Team.A);


            float danger = Mathf.Max(0, 10f - distToAIHome);


            float enemyPush = Mathf.Max(0, enemyForce - myForce);

            float advantage = Mathf.Max(0, myForce - enemyForce);

            // -----------------------------
            // Final score
            // -----------------------------
            float score =
                danger * dangerWeight +
                enemyPush * counterWeight +
                advantage * pushWeight;

            return score;
        }
    }
}