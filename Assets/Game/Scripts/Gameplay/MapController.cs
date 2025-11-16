using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private GameObject[] listMap;

        private void Awake()
        {
            var map = listMap[Random.Range(0, listMap.Length)];
            map.SetActive(true);
        }
    }
}