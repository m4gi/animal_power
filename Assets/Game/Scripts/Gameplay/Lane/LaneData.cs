using UnityEngine;

namespace Game.Scripts
{
    public class LaneData : MonoBehaviour
    {
        public Transform startPoint;
        public Transform endPoint;

        public Vector3 GetStartPosition() => startPoint.position;
        public Vector3 GetEndPosition() => endPoint.position;
    }
}