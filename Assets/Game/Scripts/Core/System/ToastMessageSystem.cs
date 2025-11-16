using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Tuns.Base;

namespace Game.Scripts
{
    public class ToastMessageSystem : Singleton<ToastMessageSystem>
    {
        [Header("Toast Settings")] 
        public CanvasGroup toastGroup;
        public TMP_Text toastText;
        public float fadeDuration = 0.3f;
        public float defaultDuration = 2f;

        Queue<(string message, float duration)> toastQueue = new Queue<(string, float)>();
        bool isShowing = false;

        protected override void AwakeSingleton()
        {
            base.AwakeSingleton();
            toastGroup.alpha = 0;
        }

        public static void Show(string message, float duration = -1f)
        {
            if (Instance == null) return;

            if (duration <= 0) duration = Instance.defaultDuration;

            Instance.toastQueue.Enqueue((message, duration));

            if (!Instance.isShowing)
                Instance.StartCoroutine(Instance.ProcessQueue());
        }

        IEnumerator ProcessQueue()
        {
            isShowing = true;

            while (toastQueue.Count > 0)
            {
                var (msg, duration) = toastQueue.Dequeue();

                toastText.text = msg;
                yield return StartCoroutine(Fade(0, 1)); // fade-in

                yield return new WaitForSeconds(duration);

                yield return StartCoroutine(Fade(1, 0)); // fade-out
            }

            isShowing = false;
        }

        IEnumerator Fade(float from, float to)
        {
            float t = 0;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                toastGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);
                yield return null;
            }

            toastGroup.alpha = to;
        }
    }
}