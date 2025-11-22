using System.Collections;
using MyPooler;
using Sirenix.OdinInspector;

namespace Game.Scripts
{
    using UnityEngine;
    using System.Collections.Generic;


    public class Lane : MonoBehaviour
    {
        public Transform spawnA;
        public Transform spawnB;
        public LaneCentralSheep central;

        public Transform leadPointA;
        public Transform leadPointB;

        public Transform homeA;
        public Transform homeB;
        public float goalThreshold = 0.5f;

        public List<AnimalUnit> stackA = new List<AnimalUnit>();
        public List<AnimalUnit> stackB = new List<AnimalUnit>();

        public List<AnimalUnit> allAnimalA = new List<AnimalUnit>();
        public List<AnimalUnit> allAnimalB = new List<AnimalUnit>();

        public void JoinLane(AnimalUnit a, Team team)
        {
            if (team == Team.A)
            {
                allAnimalA.Add(a);
            }
            else if (team == Team.B)
            {
                allAnimalB.Add(a);
            }
        }

        public void JoinStack(AnimalUnit a)
        {
            a.CurrentState = AnimalState.Stacking;

            if (a.team == Team.A)
            {
                Transform front = (stackA.Count == 0) ? leadPointA : stackA[^1].transform;
                a.SnapBehind(front);
                stackA.Add(a);
            }
            else
            {
                Transform front = (stackB.Count == 0) ? leadPointB : stackB[^1].transform;
                a.SnapBehind(front);
                stackB.Add(a);
            }
        }

        public void KillAnimal(AnimalUnit a)
        {
            a.CurrentState = AnimalState.Dead;
            stackA.Remove(a);
            stackB.Remove(a);
            ObjectPooler.Instance.ReturnToPool(a.UnitName, a.gameObject);
            //Destroy(a.gameObject);
        }

        public AnimalUnit GetFrontTeammate(AnimalUnit a)
        {
            var list = (a.team == Team.A) ? stackA : stackB;
            return list.Count == 0 ? null : list[^1];
        }

        public AnimalUnit GetFrontEnemy(Team team)
        {
            var list = (team == Team.A) ? stackB : stackA;
            return list.Count == 0 ? null : list[^1];
        }

        public float GetForceA()
        {
            float f = 0;
            foreach (var a in stackA) f += a.strength;
            return f;
        }

        public float GetForceB()
        {
            float f = 0;
            foreach (var a in stackB) f += a.strength;
            return f;
        }

        public void ResetLane()
        {
            foreach (var a in stackA)
                if (a != null)
                    a.DiscardToPool();
            foreach (var b in stackB)
                if (b != null)
                    b.DiscardToPool();

            stackA.Clear();
            stackB.Clear();

            allAnimalA.Clear();
            allAnimalB.Clear();

            var all = FindObjectsByType<AnimalUnit>(FindObjectsSortMode.None);
            foreach (var u in all)
            {
                if (u != null && u.lane == this)
                {
                    u.DiscardToPool();
                }
            }

            if (central != null)
            {
                central.ResetPosition();
                central.ResetPushSpeed();
            }
        }

        public void JoinStackAtCurrentPosition(AnimalUnit a)
        {
            a.CurrentState = AnimalState.Stacking;

            if (a.team == Team.A)
                stackA.Add(a);
            else
                stackB.Add(a);
        }

        public void JoinStackBehindTeammate(AnimalUnit a, AnimalUnit front)
        {
            a.CurrentState = AnimalState.Stacking;
            a.SnapBehind(front.transform);

            if (a.team == Team.A)
                stackA.Add(a);
            else
                stackB.Add(a);
        }

        public Transform GetFrontFor(AnimalUnit a)
        {
            if (a.team == Team.A)
            {
                int index = stackA.IndexOf(a);
                if (index == 0)
                    return central.transform;
                else
                    return stackA[index - 1].transform;
            }
            else
            {
                int index = stackB.IndexOf(a);
                if (index == 0)
                    return central.transform;
                else
                    return stackB[index - 1].transform;
            }
        }

        public float GetForce(Team t)
        {
            return (t == Team.A) ? GetForceA() : GetForceB();
        }

        public bool IsSpawnBlocked(Team team)
        {
            Vector3 spawnPos = (team == Team.A) ? spawnA.position : spawnB.position;

            float checkRadius = 0.3f;
            Collider[] hits = Physics.OverlapSphere(spawnPos, checkRadius);

            foreach (var h in hits)
            {
                if (h.TryGetComponent<AnimalUnit>(out var unit))
                {
                    if (unit.lane == this)
                        return true;
                }
            }

            return false;
        }

        [Button]
        public void KillAllAnimal(Team targetTeam)
        {
            if (targetTeam == Team.A)
            {
                foreach (var b in allAnimalB)
                    if (b != null)
                        b.DiscardToPool();

                stackB.Clear();
                allAnimalB.Clear();
            }
            else if (targetTeam == Team.B)
            {
                foreach (var a in allAnimalA)
                    if (a != null)
                        a.DiscardToPool();

                stackA.Clear();
                allAnimalA.Clear();
            }
        }
        
        public IEnumerator Shake(float duration, float magnitude)
        {
            Vector3 originalPos = transform.localPosition;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                float offsetX = Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = originalPos + new Vector3(offsetX, 0, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPos;
        }
    }
}