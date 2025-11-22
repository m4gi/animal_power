using System;
using Game.Scripts.GameData;
using Magi.Scripts.GameData;
using MyPooler;
using UnityEngine;

namespace Game.Scripts
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public partial class AnimalUnit : MonoBehaviour, IPooledObject
    {
        public Lane lane;
        public Team team;
        public float moveSpeed;
        public float strength;

        private AnimalState state = AnimalState.Running;

        public AnimalState CurrentState
        {
            get { return state; }
            set
            {
                state = value;
                UpdateAnimation(value);
            }
        }

        public Animator animator;
        public AnimalSkill skill;

        private BoxCollider col;
        private float snapDist;
        private string unitName;

        public string UnitName => unitName;


        public void Init(Lane laneP, Team teamP, AnimalConfig cfg)
        {
            unitName = cfg.animalName;
            lane = laneP;
            team = teamP;
            strength = cfg.strength;
            moveSpeed = cfg.moveSpeed;
            CurrentState = AnimalState.Running;

            laneP.JoinLane(this, teamP);

            if (skill != null)
            {
                skill.Init(this, laneP, team);
                if (skill.triggerType == TriggerType.OnSpawn)
                {
                    skill.StartCast();
                }
            }
        }

        void Awake()
        {
            col = GetComponent<BoxCollider>();
            snapDist = col.size.z * transform.localScale.z * 1.05f;
        }

        void Update()
        {
            if (state != AnimalState.Running) return;

            MoveForward();
            CheckEnemyCollision();
            CheckStackConditions();
        }

        void LateUpdate()
        {
            if (state != AnimalState.Stacking) return;

            Transform front = lane.GetFrontFor(this);
            if (front == null) return;

            float offset = (team == Team.A) ? -snapDist : snapDist;

            Vector3 target = new Vector3(
                transform.position.x,
                transform.position.y,
                front.position.z + offset
            );

            transform.position = target;
        }

        void MoveForward()
        {
            Vector3 dir = (team == Team.A) ? Vector3.forward : Vector3.back;
            transform.position += dir * moveSpeed * Time.deltaTime;
        }

        void CheckEnemyCollision()
        {
            var enemy = lane.GetFrontEnemy(team);
            if (enemy == null) return;
            if (enemy.state == AnimalState.Dead) return;

            float dz = Mathf.Abs(transform.position.z - enemy.transform.position.z);
            if (dz < snapDist)
            {
                lane.KillAnimal(this);
                lane.KillAnimal(enemy);
            }
        }

        void CheckStackConditions()
        {
            float dzCentral = Mathf.Abs(transform.position.z - lane.central.transform.position.z);
            if (dzCentral < snapDist)
            {
                lane.JoinStackAtCurrentPosition(this);
                if(skill != null && skill.triggerType == TriggerType.OnTouch)
                    skill?.StartCast();
                return;
            }

            var front = lane.GetFrontTeammate(this);
            if (front != null)
            {
                float dz = Mathf.Abs(transform.position.z - front.transform.position.z);
                if (dz < snapDist)
                {
                    lane.JoinStackBehindTeammate(this, front);
                    if(skill != null && skill.triggerType == TriggerType.OnTouch)
                        skill?.StartCast();
                }
            }
        }

        public void SnapBehind(Transform front)
        {
            float offset = (team == Team.A) ? -snapDist : snapDist;
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                front.position.z + offset
            );
        }

        public void OnRequestedFromPool()
        {
            CurrentState = AnimalState.Running;
        }

        public void DiscardToPool()
        {
            ObjectPooler.Instance.ReturnToPool(unitName, gameObject);
            ObjectPooler.Instance.GetFromPool("destroy_vfx", gameObject.transform.position + Vector3.up,
                Quaternion.identity);
            if (skill != null)
            {
                skill.EndCast();
            }
        }

        private void UpdateAnimation(AnimalState value)
        {
            if (animator != null)
            {
                switch (value)
                {
                    case AnimalState.Running:
                        animator.SetTrigger(AnimatorConst.WalkTrigger);
                        break;
                    case AnimalState.Stacking:
                        animator.SetTrigger(AnimatorConst.AttackTrigger);
                        break;
                    case AnimalState.Dead:
                        animator.SetTrigger(AnimatorConst.DieTrigger);
                        break;
                    case AnimalState.UseSkill:
                        animator.SetTrigger(AnimatorConst.SkillTrigger);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}