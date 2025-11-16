using System;
using UnityEngine;

namespace Game.Scripts.GameData
{
    [Serializable]
    public class AnimalDatabaseLocal
    {
        public string animalID;
        public string animalName;
        public AnimalData animalData;
        public Sprite animalSprite;
        public int animalPrice;
    }
}