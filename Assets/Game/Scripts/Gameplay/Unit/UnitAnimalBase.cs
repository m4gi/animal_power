using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Game.Scripts
{
    public class UnitAnimalBase : MonoBehaviour
    {
        [Header("Stats")]
        public int power = 1;
        public bool isPlayer = true;
        public float speed = 3f;

        public int LaneIndex
        {
            get => laneIndex;
            set
            {
                laneIndex = value;
                SetupLanePoints();
                CalculateSoloTarget();
            }
        }

        [Header("Formation")]
        public float formationSpacing = 1.5f;

        // === STATE ===
        private bool isSolo = true;
        public Transform leader;
        private int formationIndex = 0;
        private List<UnitAnimalBase> stackGroup = new List<UnitAnimalBase>();
        private bool isMoving = true;
        private Animator animator;
        private Vector3 soloTarget;

        private Vector3 startPointPos;
        private Vector3 endPointPos;
        private int laneIndex = 0;

        public void OnObjectSpawn()
        {
            isSolo = true;
            leader = null;
            formationIndex = 0;
            stackGroup.Clear();
            stackGroup.Add(this);

            UpdateColliders();
            SetupLanePoints();
            CalculateSoloTarget();

            animator = GetComponent<Animator>();
            if (animator)
            {
                animator.SetBool("isRunning", true);
                animator.Rebind();
            }

            // AudioManager.Instance?.PlaySFX(AudioManager.Instance.spawnClip);
        }

        void SetupLanePoints()
        {
            LaneData lane = LaneManager.Instance.GetLane(laneIndex);
            if (lane == null) return;
            startPointPos = lane.GetStartPosition();
            endPointPos = lane.GetEndPosition();
        }

        void CalculateSoloTarget()
        {
            Vector3 target = isPlayer ? endPointPos : startPointPos;
            soloTarget = new Vector3(target.x, 0.5f, target.z);
        }

        void Update()
        {
            if (!isMoving) return;
            if (CheckIndividualDeathZone()) { DieIndividual(); return; }

            float laneX = LaneManager.Instance.GetLane(laneIndex)?.transform.position.x ?? transform.position.x;

            Vector3 targetPos;
            if (isSolo)
            {
                targetPos = new Vector3(laneX, 0.5f, soloTarget.z);
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                if (Vector3.Distance(transform.position, targetPos) < 0.2f) ReachEndpoint();
            }
            else
            {
                if (leader == null || stackGroup.Count == 0) return;

                Vector3 leaderPos = stackGroup[0].transform.position;
                Vector3 behindDir = isPlayer ? Vector3.back : Vector3.forward;
                Vector3 offset = behindDir * formationSpacing * formationIndex;
                targetPos = leaderPos + offset;
                targetPos.x = laneX;

                float dist = Vector3.Distance(transform.position, targetPos);
                float moveSpeed = speed;
                if (dist > formationSpacing * 0.7f) moveSpeed *= 2f;

                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            }

            transform.rotation = isPlayer 
                ? Quaternion.Euler(0, 0, 0) 
                : Quaternion.Euler(0, 180, 0);
        }

        bool CheckIndividualDeathZone()
        {
            Vector3 deathZone = !isPlayer ? startPointPos : endPointPos;
            return Vector3.Distance(transform.position, deathZone) < 1f;
        }

        public void JoinFormation(UnitAnimalBase newLeader)
        {
            if (!isSolo) return;

            isSolo = false;
            leader = newLeader.transform;
            stackGroup = newLeader.stackGroup;

            formationIndex = stackGroup.Count;
            stackGroup.Add(this);

            SetupLanePoints();
            UpdateColliders();

            if (animator) animator.SetBool("isRunning", true);

            Vector3 leaderPos = newLeader.transform.position;
            Vector3 behindDir = newLeader.isPlayer ? Vector3.back : Vector3.forward;
            Vector3 target = leaderPos + behindDir * formationSpacing * formationIndex;

            transform.position = new Vector3(
                LaneManager.Instance.GetLane(laneIndex)?.transform.position.x ?? target.x,
                0.5f,
                target.z
            );

            transform.rotation = newLeader.transform.rotation;
            transform.DOScale(1.2f, 0.15f).SetLoops(2, LoopType.Yoyo);
        }

        void UpdateColliders()
        {
            if (stackGroup.Count == 0) return;

            foreach (var unit in stackGroup)
            {
                unit.SetupCollider(false);
            }

            stackGroup[0].SetupCollider(true);

            if (stackGroup.Count > 1)
            {
                stackGroup[stackGroup.Count - 1].SetupCollider(true);
            }
        }

        void SetupCollider(bool active)
        {
            Collider col = GetComponent<Collider>();
            if (col) col.enabled = active;
        }

        void DieIndividual()
        {
            isMoving = false;
            if (animator) animator.SetTrigger("Die");
            ObjectPool.Instance.SpawnFromPool("DeathExplosion", transform.position, Quaternion.identity);

            if (stackGroup.Contains(this))
            {
                stackGroup.Remove(this);

                for (int i = 1; i < stackGroup.Count; i++)
                {
                    stackGroup[i].formationIndex = i;
                }

                if (stackGroup.Count > 0)
                {
                    UnitAnimalBase newLeader = stackGroup[0];
                    newLeader.leader = newLeader.transform;
                    newLeader.formationIndex = 0;
                    newLeader.UpdateColliders();
                }
            }

            // AudioManager.Instance?.PlaySFX(AudioManager.Instance.dieClip);
            Invoke(nameof(ReturnToPool), 0.8f);
        }

        void OnTriggerEnter(Collider other)
        {
            UnitAnimalBase otherAnimal = other.GetComponent<UnitAnimalBase>();
            if (otherAnimal == null) return;

            if (otherAnimal.isPlayer == isPlayer)
            {
                HandleStackJoin(otherAnimal);
                return;
            }

            if (!IsLeaderOrLast()) return;

            HandleCollision(otherAnimal);
        }

        bool IsLeaderOrLast()
        {
            if (stackGroup.Count == 0) return false;
            int index = stackGroup.IndexOf(this);
            return index == 0 || index == stackGroup.Count - 1;
        }

        void HandleStackJoin(UnitAnimalBase otherAnimal)
        {
            UnitAnimalBase leaderAnimal = GetLeaderAnimal(otherAnimal);
            if (leaderAnimal != this)
                JoinFormation(leaderAnimal);
            else if (otherAnimal != leaderAnimal)
                otherAnimal.JoinFormation(leaderAnimal);
        }

        UnitAnimalBase GetLeaderAnimal(UnitAnimalBase otherAnimal)
        {
            float myDist = Vector3.Distance(transform.position, soloTarget);
            float otherDist = Vector3.Distance(otherAnimal.transform.position, otherAnimal.soloTarget);
            return myDist < otherDist ? this : otherAnimal;
        }

        void HandleCollision(UnitAnimalBase enemy)
        {
            int myPower = GetTotalPower();
            int enemyPower = enemy.GetTotalPower();

            if (myPower > enemyPower)
            {
                MyStackAdvanceToEnd();
                enemy.EnemyStackRetreatToStart();
            }
            else if (myPower < enemyPower)
            {
                MyStackRetreatToStart();
                enemy.MyStackAdvanceToEnd();
            }
            else
            {
                StopFormation();
                enemy.StopFormation();
                transform.DOShakePosition(0.4f, 0.3f);
                enemy.transform.DOShakePosition(0.4f, 0.3f);
                Invoke(nameof(ResumeFormation), 0.5f);
            }

            // AudioManager.Instance?.PlaySFX(AudioManager.Instance.pushClip);
        }

        public void MyStackAdvanceToEnd()
        {
            StopFormation();
            Vector3 endPoint = isPlayer ? endPointPos : startPointPos;
            Vector3 dir = (endPoint - leader.position).normalized;
            Vector3 target = leader.position + dir * 2f;

            leader.DOMove(target, 0.6f)
                  .SetEase(Ease.InOutSine)
                  .OnComplete(ResumeFormation);
        }

        void MyStackRetreatToStart()
        {
            StopFormation();
            Vector3 startPoint = isPlayer ? startPointPos : endPointPos;
            Vector3 dir = (startPoint - leader.position).normalized;
            Vector3 target = leader.position + dir * 3f;

            leader.DOMove(target, 0.8f)
                  .SetEase(Ease.OutBack)
                  .OnComplete(() =>
                  {
                      CheckLastUnitDeathOnRetreat();
                      ResumeFormation();
                  });
        }
        
        public void EnemyStackRetreatToStart() => MyStackRetreatToStart();

        void CheckLastUnitDeathOnRetreat()
        {
            if (stackGroup.Count <= 1) return;

            UnitAnimalBase lastUnit = stackGroup[stackGroup.Count - 1];
            Vector3 startPoint = isPlayer ? startPointPos : endPointPos;

            if (Vector3.Distance(lastUnit.transform.position, startPoint) < 1.5f)
            {
                lastUnit.DieIndividual();
            }
        }

        public void StopFormation()
        {
            isMoving = false;
            foreach (var m in stackGroup)
            {
                m.isMoving = false;
                if (m.animator) m.animator.SetBool("isRunning", false);
            }
        }

        public void ResumeFormation()
        {
            isMoving = true;
            foreach (var m in stackGroup)
            {
                m.isMoving = true;
                if (m.animator) m.animator.SetBool("isRunning", true);
            }
        }

        void ReachEndpoint()
        {
            isMoving = false;
            int damage = GetMaxPower();
            if (isPlayer)
                GameManager.Instance.DealDamageToBot(damage);
            else
                GameManager.Instance.DealDamageToPlayer(damage);

            foreach (var m in stackGroup)
            {
                if (m.animator) m.animator.SetTrigger("Victory");
                ObjectPool.Instance.SpawnFromPool("VictoryExplosion", m.transform.position, Quaternion.identity);
            }

            // AudioManager.Instance?.PlaySFX(AudioManager.Instance.victoryClip);
            Invoke(nameof(DestroyFormation), 1.2f);
        }

        void DestroyFormation()
        {
            foreach (var m in stackGroup.ToList())
                if (m) m.ReturnToPool();
        }

        void ReturnToPool()
        {
            PooledObject pooledObj = GetComponent<PooledObject>();
            if (pooledObj != null) pooledObj.ReturnToPool();
        }

        public int GetTotalPower() => stackGroup.Sum(a => a.power);
        public int GetMaxPower() => stackGroup.Max(a => a.power);

        void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;
            bool hasCollider = GetComponent<Collider>().enabled;
            Gizmos.color = hasCollider ? Color.red : Color.gray;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.8f, 0.5f);

            #if UNITY_EDITOR
            string label = isSolo ? "SOLO" : 
                          (formationIndex == 0 ? "LEADER" : 
                           formationIndex == stackGroup.Count - 1 ? "LAST" : "MID");
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f, label);
            #endif
        }
    }
}