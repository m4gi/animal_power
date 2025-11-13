using System;
using Game.Scripts.GameData;
using UnityEngine;

namespace Game.Scripts.Gameplay
{
    public class LaneClickSpawner : MonoBehaviour
    {
        [SerializeField] private AnimalData animalData;
        [SerializeField] private Camera mainCam;

        private void Awake()
        {
            if (mainCam == null)
                mainCam = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    LaneData lane = hit.collider.GetComponent<LaneData>();
                    if (lane != null)
                    {
                        SpawnRandomCau(lane);
                    }
                }
            }
        }

        void SpawnRandomCau(LaneData lane)
        {
            var info = animalData.GetRandomAnimal();
            GameObject cau = ObjectPool.Instance.SpawnFromPool(
                info.prefabName,
                lane.GetStartPosition(),
                Quaternion.identity
            );
            var unit = cau.GetComponent<UnitAnimalBase>();
            unit.LaneIndex = System.Array.IndexOf(LaneManager.Instance.LaneData, lane);
            unit.speed *= info.speedMultiplier;
            unit.power = info.power;
        }
    }
}