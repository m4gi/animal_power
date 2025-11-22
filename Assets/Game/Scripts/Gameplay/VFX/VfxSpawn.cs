using CartoonFX;
using MyPooler;
using UnityEngine;

namespace Game.Scripts.VFX
{
    public class VfxSpawn : MonoBehaviour, IPooledObject
    {
       public CFXR_Effect effect;
       public float effectDuration = 0.5f;
        public void OnRequestedFromPool()
        {
            effect.Animate(effectDuration);
            Invoke(nameof(DiscardToPool), effectDuration);
        }

        public void DiscardToPool()
        {
            ObjectPooler.Instance.ReturnToPool("spawn_vfx", this.gameObject);
        }
    }
}