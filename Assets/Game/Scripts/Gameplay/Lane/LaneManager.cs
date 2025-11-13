using Tuns.Base;
using UnityEngine;

namespace Game.Scripts
{
    public class LaneManager : Singleton<LaneManager>
    {
        [SerializeField] private LaneData[] lanes = new LaneData[5];

        public LaneData[] LaneData => lanes;
        
        public LaneData GetLane(int index)
        {
            if (index < 0 || index >= lanes.Length) return null;
            return lanes[index];
        }
    }
}