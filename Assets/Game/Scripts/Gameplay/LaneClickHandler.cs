using UnityEngine;

namespace Game.Scripts
{
    public class LaneClickHandler : MonoBehaviour
    {
        public Lane lane;

        private void OnMouseDown()
        {
            GameController.Instance.OnLaneClicked(lane);
        }
    }
}