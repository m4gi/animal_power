using System;
using UnityEngine;

namespace Game.Scripts.GameData
{
    [CreateAssetMenu(fileName = "LevelDataConfig", menuName = "Game/LevelDataConfig")]
    public class LevelDataConfig : ScriptableObject
    {
        public LevelData[] levels;

        // private void OnValidate()
        // {
        //     levels = new LevelData[80];
        //     for (int i = 0; i < 80; i++)
        //     {
        //         levels[i] = new LevelData();
        //         levels[i].level = i;
        //         levels[i].maxHealth = i + 1;
        //         levels[i].time = i < 40 ? 300 : 500;
        //     }
        // }
    }

    [System.Serializable]
    public class LevelData
    {
        public int level;
        public int maxHealth;
        public int time;
    }
}