using System;
using UnityEngine;

namespace Game.Scripts
{
    public class UIPanelActive : MonoBehaviour
    {
        [SerializeField] private GameObject[] hidePanel;

        private void OnEnable()
        {
            foreach (var panel in hidePanel)
            {
                panel.SetActive(false);
            }
        }
    }
}