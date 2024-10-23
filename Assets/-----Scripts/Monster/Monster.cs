using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using LittleTurtle.UI;
using LittleTurtle.System_;

namespace LittleTurtle.Enemy { 

    [RequireComponent(typeof(AudioSource), typeof(AIPath), typeof(Seeker))]
    [RequireComponent(typeof(AIDestinationSetter), typeof(Rigidbody2D))]
    public abstract class Monster : MonoBehaviour{

        #region = ability =

        [Header("Ability")]
        public MonsterData data;

        [Header("Other")]
        public Vector2 healthBarOffset;
        public CurveLerping lerping_HealthBar;
        private CurveLerping _lerping_HealthBar;
        public GameObject DropPrefab;

        [Header("Component")]
        public Collider2D monsterCollider;
        public GameObject healthBarObject;
        public Transform weaponPos;
        public SpriteRenderer healthBar;
        public SpriteRenderer attackGraph;
        private Collider2D attackGraphCollider;
        protected AudioSource audioSource;
        #endregion

        #region = parameters =

        ///////////////////

        Transform playerTransform;
        AIDestinationSetter setter;
        protected MonsterAnimation monsterAnimation;
        AIPath aiPath;
        //Seeker seeker;
        Rigidbody2D rb;
        private PlayerCtrl playerCtrl;
        //private float rotateSpeed;
        protected bool reachDestination = false;
        protected bool dying = false;
        //health bar
        //private float spriteHeight, spriteWidth;
        //private float minOffsetWhenVertical, maxOffsetWhenHorizontal, minOffsetWhenVertical_Sin;
        public float health;

        private bool _isAttacking;
        private bool IsAttacking { get => _isAttacking; set { _isAttacking = value; ChangeIsAttack(); } }
        private void ChangeIsAttack() {
            if (_isAttacking) {
                monsterAnimation.SwitchAnimationType(MonsterAnimatorType.idel);
                aiPath.maxSpeed = 0;
                reachDestination = true;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            else {
                monsterAnimation.SwitchAnimationType(MonsterAnimatorType.move);
                aiPath.maxSpeed = data.moveSpeed;
                reachDestination = false;
                rb.constraints = RigidbodyConstraints2D.None;
            }
        }


        #endregion
        HealthBarPosCaculate HPbarPosCaculate;
        public virtual void Start(){
            #region = Get Componets =
            if (data.attackTarget == AttackTarget.player) {
                playerTransform = PlayerCtrl.Instance.gameObject.transform;
            }

            aiPath = GetComponent<AIPath>();
            rb = GetComponent<Rigidbody2D>();
            setter = GetComponent<AIDestinationSetter>();
            monsterAnimation = GetComponentInChildren<MonsterAnimation>();
            audioSource = GetComponent<AudioSource>();
            attackGraphCollider = attackGraph.GetComponent<Collider2D>();
            playerCtrl = PlayerCtrl.Instance;
            #endregion

            #region = Initialize =

            attackGraph.color = new Color(1, 1, 1, 0);

            aiPath.maxSpeed = data.moveSpeed;
            healthBar.transform.localScale = new Vector3(1, 1, 1);

            Vector2 v2 = GetComponent<Collider2D>().bounds.size;
            HPbarPosCaculate = new HealthBarPosCaculate(v2.x, v2.y, transform, healthBarObject, healthBarOffset);

            switch (data.attackTarget) {
                case AttackTarget.player:
                    _isAttacking = false;
                    setter.target = playerTransform;
                    monsterAnimation.SwitchAnimationType(MonsterAnimatorType.move);
                    break;
                case AttackTarget.none:

                    break;
            }

            _lerping_HealthBar = ScriptableObject.CreateInstance<CurveLerping>();
            _lerping_HealthBar.SetAdjustVaule(healthBar);

            SaveDataMgr.monsterList.Add(this);
            
            #endregion


            if (!MyGameManager.isLoadingSaved) {
                health = data.MaxHealth;
                healthBarObject.SetActive(false);
            }
            else {
                healthBarObject.SetActive(true);
                _lerping_HealthBar.SetTarget(health / data.MaxHealth, CurveLerping.CurveType.EaseOut_Slow);
            }
        }
        private void OnDestroy() {
            SaveDataMgr.monsterList.Remove(this);
            playerCtrl.AttackList_Remove(this);
        }

        public virtual void Die() {
            playerCtrl.AttackList_Remove(this);
            monsterCollider.isTrigger = true;

            dying = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

            GameObject g = Instantiate(DropPrefab, transform.position, transform.rotation);
            TemporaryStorage.Instance.PlaceObject(g);

            healthBarObject.SetActive(false);
            attackGraph.gameObject.SetActive(false);
            monsterAnimation.Action_DieAnimationEnd = () => { Destroy(gameObject); };
            monsterAnimation.SwitchAnimationType(MonsterAnimatorType.Die);
        }
        public virtual void Attack() {
            if (data.AudioClip_Attack) {
                audioSource.clip = data.AudioClip_Attack;
                audioSource.Play();
            }
        }
        public virtual void Update() {

            if (dying) return;

            if (_lerping_HealthBar.isLerping) _lerping_HealthBar.Update();

            if (!aiPath.hasPath) {
                return;
            }

            switch (data.attackTarget) {
                case AttackTarget.player:
                    if (attackGraphCollider.IsTouching(PlayerCtrl.PlayerCollider) &! reachDestination) {
                        IsAttacking = true;
                        StartCoroutine(ReadyToAttack());
                    }
                    break;

                case AttackTarget.none:

                    break;
            }

            HPbarPosCaculate.UpdateHealthBarPosition();
        }


        #region == Health ==
        private bool AddToMonsterList = false;
        public virtual void HealthChange(float changes) {
            if (!healthBarObject.activeSelf) {
                healthBarObject.SetActive(true);
            }

            if (health + changes <= 0) {
                Die();
            }
            else if (health + changes > data.MaxHealth) {
                health = data.MaxHealth;
            }
            else {
                health = health + changes;
            }

            if (!AddToMonsterList) {
                if (changes < 0) {
                    AddToMonsterList = true;
                    playerCtrl.AttackList_Add(this);
                }
            }
            else {
                if (changes < 0) {
                    playerCtrl.UpdateMonsterDataPanel();
                }
            }
            
            _lerping_HealthBar.SetTarget(health / data.MaxHealth, CurveLerping.CurveType.EaseOut_Slow);
        }
    
        public virtual void AttackFinished() {
            switch (data.attackTarget) {
                case AttackTarget.player:
                    IsAttacking = false;
                    break;
                case AttackTarget.none:
                    break;
            }
        }

        #endregion

        #region == Attack Method ==
        protected void Attack_General(GameObject weapon) {
            GameObject emptyObject = new GameObject("weapon" + data.monsterName);
            emptyObject.transform.position = weaponPos.position;
            emptyObject.transform.rotation = weaponPos.rotation;

            TemporaryStorage.Instance.PlaceObject(emptyObject);
            GameObject weaponObject = Instantiate(weapon, Vector3.zero, Quaternion.identity, emptyObject.transform);
            weaponObject.GetComponent<MonsterWeapon>().SetInfo(this);
        }
        #endregion

        IEnumerator ReadyToAttack() {
            float passTime = 0;

            while (passTime < data.attackTime) {
                attackGraph.color = new Color(1, 1, 1, Mathf.Lerp(0, 180f / 255f, passTime / data.attackTime));

                passTime += Time.deltaTime;
                yield return null;
            }

            attackGraph.color = new Color(1, 1, 1, 0);
            Attack();
        }

        
        public _MonsterData GetData() {

            _MonsterData d = new _MonsterData();

            d.ID = data.ID;
            d.health = health;
            d.position = transform.position;
            d.quaternion = transform.rotation;

            return d;
        }
        public void LoadData(_MonsterData data) {
            health = data.health;
            transform.position = data.position;
            transform.rotation = data.quaternion;
        }


    }
}