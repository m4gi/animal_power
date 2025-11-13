using System.Collections.Generic;
using System.Linq;
using Game.Scripts.GameData;
using UnityEngine;

namespace Game.Scripts
{
    public class BotAI : MonoBehaviour
    {
    
        [Header("AI Settings")]
        public float thinkInterval = 0.5f; // Tính toán mỗi 0.5s
        public float spawnDelay = 0.3f;   // Delay giữa các lần thả
        public AnimalData animalData;

        private float timer = 0f;
        private float spawnTimer = 0f;
        private bool isSpawning = false;

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= thinkInterval && !isSpawning)
            {
                timer = 0f;
                ThinkAndAct();
            }

            if (isSpawning)
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer >= spawnDelay)
                {
                    isSpawning = false;
                    spawnTimer = 0f;
                }
            }
        }

        void ThinkAndAct()
        {
            // Bước 1: Thu thập trạng thái 5 lane
            List<LaneState> laneStates = AnalyzeLanes();

            // Bước 2: Chọn lane tối ưu
            LaneState bestLane = ChooseBestLane(laneStates);
            if (bestLane == null) return;

            // Bước 3: Chọn loại cừu phù hợp
            AnimalConfig bestCau = ChooseBestCau(bestLane);

            // Bước 4: Thả cừu
            SpawnCau(bestLane.laneIndex, bestCau);
        }

        // === PHÂN TÍCH 5 LANE ===
        List<LaneState> AnalyzeLanes()
        {
            List<LaneState> states = new List<LaneState>();

            for (int i = 0; i < 5; i++)
            {
                LaneData lane = LaneManager.Instance.GetLane(i);
                if (lane == null) continue;

                LaneState state = new LaneState
                {
                    laneIndex = i,
                    playerPower = 0,
                    botPower = 0,
                    hasPlayerStack = false,
                    hasBotStack = false,
                    playerDistanceToEnd = float.MaxValue,
                    botDistanceToEnd = float.MaxValue
                };

                // Tìm tất cả Unit trong lane
                Collider[] hits = Physics.OverlapSphere(lane.GetStartPosition(), 50f);
                foreach (Collider col in hits)
                {
                    UnitAnimalBase unit = col.GetComponent<UnitAnimalBase>();
                    if (unit == null || unit.LaneIndex != i) continue;

                    if (unit.isPlayer)
                    {
                        state.playerPower += unit.GetTotalPower();
                        state.hasPlayerStack = true;
                        float dist = Vector3.Distance(unit.transform.position, lane.GetEndPosition());
                        if (dist < state.playerDistanceToEnd)
                            state.playerDistanceToEnd = dist;
                    }
                    else
                    {
                        state.botPower += unit.GetTotalPower();
                        state.hasBotStack = true;
                        float dist = Vector3.Distance(unit.transform.position, lane.GetStartPosition());
                        if (dist < state.botDistanceToEnd)
                            state.botDistanceToEnd = dist;
                    }
                }

                states.Add(state);
            }

            return states;
        }

        // === CHỌN LANE TỐI ƯU ===
        LaneState ChooseBestLane(List<LaneState> states)
        {
            LaneState best = null;
            float bestScore = float.MinValue;

            foreach (LaneState state in states)
            {
                float score = CalculateLaneScore(state);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = state;
                }
            }

            return best;
        }

        float CalculateLaneScore(LaneState state)
        {
            float score = 0f;

            // 1. Ưu tiên lane yếu của player
            if (state.hasPlayerStack && state.playerPower < 50)
                score += 100;

            // 2. Ưu tiên lane mình đang mạnh
            if (state.hasBotStack && state.botPower > state.playerPower)
                score += 80;

            // 3. Ưu tiên lane trống
            if (!state.hasPlayerStack && !state.hasBotStack)
                score += 50;

            // 4. Ưu tiên lane gần endpoint
            if (state.hasBotStack && state.botDistanceToEnd < 10f)
                score += 120;

            // 5. Tránh lane player mạnh
            if (state.hasPlayerStack && state.playerPower > 100)
                score -= 150;

            return score;
        }

        // === CHỌN LOẠI CỪU ===
        AnimalConfig ChooseBestCau(LaneState state)
        {
            // Nếu player mạnh → dùng cừu to
            if (state.hasPlayerStack && state.playerPower > 60)
                return animalData.animals.Where(c => c.power >= 60).OrderByDescending(c => c.power).FirstOrDefault();

            // Nếu lane trống → dùng cừu nhỏ nhanh
            if (!state.hasPlayerStack && !state.hasBotStack)
                return animalData.animals.OrderBy(c => c.power).FirstOrDefault();

            // Nếu mình đang mạnh → dùng cừu vừa
            if (state.hasBotStack && state.botPower > state.playerPower)
                return animalData.animals.Where(c => c.power >= 40).OrderByDescending(c => c.power).FirstOrDefault();

            // Mặc định: cừu trung bình
            return animalData.animals[Random.Range(0, animalData.animals.Length)];
        }

        // === THẢ CỪU ===
        void SpawnCau(int laneIndex, AnimalConfig cauInfo)
        {
            if (isSpawning) return;
            isSpawning = true;

            LaneData lane = LaneManager.Instance.GetLane(laneIndex);
            if (lane == null) return;

            GameObject cau = ObjectPool.Instance.SpawnFromPool(
                cauInfo.prefabName,
                lane.GetEndPosition(), // Bot spawn từ trên
                Quaternion.Euler(0, 180, 0)
            );

            if (cau != null)
            {
                UnitAnimalBase unit = cau.GetComponent<UnitAnimalBase>();
                unit.isPlayer = false;
                unit.LaneIndex = laneIndex;
                unit.speed *= cauInfo.speedMultiplier;
                unit.power = cauInfo.power;
            }
        }

        // === CẤU TRÚC DỮ LIỆU LANE ===
        private class LaneState
        {
            public int laneIndex;
            public int playerPower;
            public int botPower;
            public bool hasPlayerStack;
            public bool hasBotStack;
            public float playerDistanceToEnd;
            public float botDistanceToEnd;
        }
    }
}