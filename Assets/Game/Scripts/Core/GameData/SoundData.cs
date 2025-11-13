using System;
using UnityEngine;

namespace Magi.Scripts.GameData
{
    [Serializable]
    public class SoundData
    {
        public string soundId;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume;
    }
}