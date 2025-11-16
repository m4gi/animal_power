using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.GameData
{
    [CreateAssetMenu(fileName = "AnimalData", menuName = "Game/AnimalData")]
    public class AnimalData : ScriptableObject
    {
        public AnimalConfig[] animals;
        
        public AnimalConfig Get(int id) => animals[id];
        public AnimalConfig Get(string id) => animals.FirstOrDefault(x => x.animalName == id);
        
    }

    [System.Serializable]
    public class AnimalConfig
    {
        public string animalName;
        public GameObject prefab;
        public float moveSpeed = 4f;
        public float strength = 1f;
        public Sprite animalSprite;
        public int animalLevel = 1;
    }
}