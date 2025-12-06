using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game.Scripts
{
    public class ChapterAnimManager : MonoBehaviour
    {
        [SerializeField] private Transform[] itemsToScale;
        
        private void OnEnable()
        {
            PlayScaleUpAnimation();
        }

        private void PlayScaleUpAnimation()
        {
            float delay = 0f;

            foreach (var item in itemsToScale)
            {
                if (item == null) continue;

                item.transform.localScale = Vector3.one * 0.5f;

                item.transform.DOScale(1f, 0.25f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(delay);

                delay += 0.05f;
            }
        }
    }
}