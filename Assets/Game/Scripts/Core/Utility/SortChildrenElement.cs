using System;
using UnityEngine;

namespace Game.Scripts.Core.Utility
{
    public class SortChildrenElement : MonoBehaviour
    {
        public GameObject[] chillObjects;

        public void Sort()
        {
            for (int i = 0; i < chillObjects.Length; i++)
            {
                chillObjects[i].transform.SetSiblingIndex(i);
            }
        }
    }
}