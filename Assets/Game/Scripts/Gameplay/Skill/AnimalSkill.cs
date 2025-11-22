using UnityEngine;

namespace Game.Scripts
{
    public enum TriggerType
    {
        OnSpawn,
        OnDespawn,
        OnTouch
    }
    public abstract class AnimalSkill : MonoBehaviour
    {
        protected AnimalUnit unit;
        protected Lane lane;
        protected Team team;
        
        public TriggerType triggerType;

        public void Init(AnimalUnit u, Lane l, Team t)
        {
            unit = u;
            lane = l;
            team = t;
        }

        public abstract void StartCast();
        public abstract void EndCast();
    }
}