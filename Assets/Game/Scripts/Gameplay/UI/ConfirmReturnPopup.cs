using Magi.Scripts.GameData;
using UnityEngine;

namespace Game.Scripts
{
    public class ConfirmReturnPopup : MonoBehaviour
    {
        public void ReturnHome()
        {
            SceneLoaderSystem.Instance.LoadScene(SceneConst.MainScene);
        }
    }
}