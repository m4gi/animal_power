using System.Collections.Generic;

namespace Magi.Scripts.GameData
{
    [System.Serializable]
    public class PlayerData
    {
        public int Coin { get; set; } = 0;
        public bool SfxStatus { get; set; }
        public bool MusicStatus { get; set; }
        public float SfxVolume { get; set; }
        public float MusicVolume { get; set; }
        public List<string> Skins { get; set; }
        public string SelectedSkin { get; set; }
        public int CurrentLevel  { get; set; }
    }

    [System.Serializable]
    public class MissionStaus
    {
        public string MissionId { get; set; }
        public bool[] MissionStatus { get; set; } = new bool[3];
    }
}