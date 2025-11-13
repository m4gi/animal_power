using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.GameData
{
    [CreateAssetMenu(fileName = "AnimalData", menuName = "AnimalPower/AnimalData")]
    public class AnimalData : ScriptableObject
    {
        public AnimalConfig[] Animals;
    }

    [System.Serializable]
    public class AnimalConfig
    {
        public string unitName;
        public Sprite icon;
        public GameObject prefab;
    
        [Header("Stats")]
        public float baseStrength = 10f;
        public float speed = 2f;
    
        [Header("Visual")]
        public Color teamColor = Color.white;
    }
}