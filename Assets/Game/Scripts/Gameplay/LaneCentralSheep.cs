using UnityEngine;

namespace Game.Scripts
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class LaneCentralSheep : MonoBehaviour
    {
        public Lane lane;
        public float pushSpeed = 2f;

        private Vector3 startPosition;

        private float originPushSpeed;

        void Awake()
        {
            originPushSpeed = pushSpeed;
            startPosition = transform.position;
        }

        void Update()
        {
            float forceA = lane.GetForceA();
            float forceB = lane.GetForceB();

            float net = forceA - forceB;

            if (Mathf.Abs(net) < 0.01f)
                return;

            float direction = (net > 0) ? 1f : -1f;

            float speed = pushSpeed * 0.8f;

            transform.position += Vector3.forward * (direction * speed * Time.deltaTime);
        }

        public void ResetPosition()
        {
            transform.position = startPosition;
        }

        public void ResetPushSpeed()
        {
            pushSpeed = originPushSpeed;
        }

        public void SetPushSpeed(float speed)
        {
            pushSpeed = speed;
        }
    }
}