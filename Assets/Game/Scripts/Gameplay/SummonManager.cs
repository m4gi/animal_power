using System.Collections.Generic;
using Game.Scripts.GameData;
using Magi.Scripts.GameData;
using MyPooler;
using Tuns.Base;
using UnityEngine;

namespace Game.Scripts
{
    public class SummonManager : Singleton<SummonManager>
    {
        public bool Summon(Team team, Lane lane, AnimalConfig cfg)
        {
            if (lane.IsSpawnBlocked(team))
                return false;

            Transform spawn = (team == Team.A) ? lane.spawnA : lane.spawnB;

            GameObject go = Instantiate(cfg.prefab, spawn.position, spawn.rotation);
            go.transform.forward = (team == Team.A) ? Vector3.forward : Vector3.back;

            AnimalUnit unit = go.GetComponent<AnimalUnit>();
            unit.Init(lane, team, cfg);
            
            SoundSystem.Instance.PlaySFX(SFXConst.SpawnAnimal);
            return true;
        }
    }

}