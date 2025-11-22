using System.Collections;
using UnityEngine;

namespace Game.Scripts
{
    public class KillEnemiesSkill : AnimalSkill
    {
        Coroutine coroutine;
        public float timeCast = 0.9f;

        public float rate = 0.5f;
        public override void StartCast()
        {
            coroutine = StartCoroutine(TriggerSkill());
        }

        public override void EndCast()
        {
            unit.CurrentState = AnimalState.Running;
            
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        private IEnumerator TriggerSkill()
        {
            unit.CurrentState = AnimalState.UseSkill;
            yield return lane.Shake(timeCast, rate);
            
            lane.KillAllAnimal(team);
            EndCast();
        }
    }
}