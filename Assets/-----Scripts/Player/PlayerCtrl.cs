using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using System.Linq;
using LittleTurtle.Player.Weapon;
using LittleTurtle.UI;
using LittleTurtle.Inventory;
using LittleTurtle.Enemy;
using LittleTurtle.System_;

namespace LittleTurtle {

    public class PlayerCtrl : MonoBehaviour, IDatapersistence {

        [Header("Ability")]
        public float MoveSpeed;
        public float rotateSpeed;
        public int MaxHealth;

        [Header("Components")]
        public Animator animator_player;
        public AudioSource audioSource;
        public GameObject weaponPosition;
        public Collider2D _collider;

        #region = Parameters =

        [HideInInspector]
        private Rigidbody2D rb;

        // player weapon
        public GameObject fist;
        public Dictionary<Equipment_Weapon, GameObject> WeaponList = new Dictionary<Equipment_Weapon, GameObject>();

        public GameObject CurrentWeapon {
            get { return currentWeapon != null ? currentWeapon.gameObject : null; }
            set {
                if (value != null) {
                    currentWeapon = value.GetComponentInChildren<PlayerWeapon>();
                }
            }
        }
        public PlayerWeapon currentWeapon;

        //
        private Canvas_Game canvas_mgr;
        private Vector2 move;
        public bool aiming = false;
        public static PlayerCtrl Instance;
        public static Collider2D PlayerCollider;

        // Instances
        private Joystick_Movement joystick_Movement;
        private Joystick_Attack Joystick_Attack;
        private MonsterDataPanel monsterDataPanel;


        private float _health;
        public float Health { get => _health; set { _health = value; ChangeHealth(); }  }
        void ChangeHealth() {
            canvas_mgr.HealthBarUpdate();
        }

        // pickable item
        private PickableItemsListMgr pickableItemsListMgr;
        private LayerMask layerMask_PickableItem;
        #endregion

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else {
                Destroy(gameObject);
            }

        }
        void Start() {

            #region = Initialize =

            // get components
            layerMask_PickableItem = LayerMask.GetMask("PickableItem");
            pickableItemsListMgr = PickableItemsListMgr.Instance;
            joystick_Movement = Joystick_Movement.Instance;
            Joystick_Attack = Joystick_Attack.Instance;
            canvas_mgr = Canvas_Game.Instance;
            monsterDataPanel = canvas_mgr.monsterDataPanel;
            PlayerCollider = _collider;
            rb = GetComponentInParent<Rigidbody2D>();

            // initialize parameter
            if (!MyGameManager.isLoadingSaved && CurrentWeapon == null) {
                SwitchWeapon(fist);
            }
            #endregion
        }

        public void InitizlizePlayerHealth() {
            Health = MaxHealth;
        }

        private Quaternion targetRotateAngle;

        private void FixedUpdate() {
            #region = Move =
            if (joystick_Movement.IsMoveing) {
                if (!aiming) {
                    rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotateAngle, rotateSpeed * Time.fixedDeltaTime));
                }
                rb.velocity = move;
            }
            #endregion
        }


        private List<Monster> monstersAttacked = new List<Monster>(); 

        void Update() {
            #region = Monster Data Panel =
            if (closestMonster) {
                if (Vector2.Distance(transform.position, closestMonster.transform.position) > 3) {
                    monsterDataPanel.Hide();
                    ClosestMonsterUpdate();
                }
                else if (!monsterDataPanel.isActiveAndEnabled) {
                    monsterDataPanel.Show();
                }
            }
            #endregion



            #region = movement(for dev) =

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) {
                joystick_Movement.IsMoveing = true;
            }

            if (joystick_Movement.IsMoveing) {
                if (Input.anyKey) {
                    float h = Input.GetAxis("Horizontal");
                    float v = Input.GetAxis("Vertical");

                    if (h != 0 || v != 0) {
                        move.x = Mathf.Clamp(1, -1, h);
                        move.y = Mathf.Clamp(1, -1, v);

                        SetMovement(move);
                        if (!joystick_Movement.IsMoveing) {
                            joystick_Movement.IsMoveing = true;
                        }
                    }
                }
            }

            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D)) {
                if (!Input.GetKey(KeyCode.A) &!Input.GetKey(KeyCode.W) & !Input.GetKey(KeyCode.S) & !Input.GetKey(KeyCode.D)) {
                    joystick_Movement.PointerUp();
                    move = Vector2.zero;
                }
            }

            #endregion

            #region = Detect Item Gameobjec =
            Collider2D[] collidersInRadius = Physics2D.OverlapCircleAll(transform.position, 1, layerMask_PickableItem);

            // check item enter detection range
            foreach (Collider2D collider in collidersInRadius) {
                if (!pickableItemsListMgr.list.ContainsKey(collider)) {
                    pickableItemsListMgr.AddItem(collider);
                }
            }

            // check item leaves detection range
            List<Collider2D> toRemoveItemList = new List<Collider2D>();
            foreach (Collider2D collider in pickableItemsListMgr.list.Keys) {
                if (!collidersInRadius.Contains(collider)) {
                    toRemoveItemList.Add(collider);
                }
            }
            foreach (Collider2D collider in toRemoveItemList) {
                pickableItemsListMgr.LeaveItem(collider);
            }
            toRemoveItemList.Clear();
            #endregion
        }

        #region = Health Bar =
        private void HealthChange(float Changes) {
            if (Health + Changes > 1) {
                Health = Health + Changes > MaxHealth ? MaxHealth : Health + Changes;
            }
            else {
                // Player Die
            }
        }
        public void GetHurt(float Damage) {
            HealthChange(-Damage);
        }
        #endregion


        #region = movement functions =
        public void SetMovement(Vector2 move) {
            if (move != Vector2.zero) {
                this.move = move.normalized * MoveSpeed;
                targetRotateAngle = Quaternion.Euler(0, 0, Mathf.Atan2(move.y, move.x) * 90f / 1.57f + 90);
            }
        }
        public void SwitchIsMoveing() {
            animator_player.SetBool("walking", joystick_Movement.IsMoveing);
        }
        #endregion

        #region = Aiming functions =
        public void SetRotation(Vector2 dir) {
            rb.MoveRotation(Quaternion.Slerp(transform.rotation,
                Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * 90f / 1.57f + 90), rotateSpeed * Time.deltaTime));
        }
        public void SetIsAiming(bool b) {
            aiming = b;
        }
        #endregion

        #region = Closest Monster =
        private Monster closestMonster;
        public void AttackList_Add(Monster monster) {
            monstersAttacked.Add(monster);
            ClosestMonsterUpdate();
        }
        public void AttackList_Remove(Monster monster) {
            if (monstersAttacked.Contains(monster)) {
                monstersAttacked.Remove(monster);
                ClosestMonsterUpdate();
            }
        }
        public void UpdateMonsterDataPanel() {
            monsterDataPanel.UpdateInfo();
        }

        private void ClosestMonsterUpdate() {
            if (monstersAttacked.Count > 0) {
                float closestDis = Mathf.Infinity;

                foreach (var item in monstersAttacked) {
                    if (Vector2.Distance(item.transform.position, transform.position) < closestDis) {
                        closestMonster = item;
                        closestDis = Vector2.Distance(item.transform.position, transform.position);
                    }
                }

                if (closestDis < 5) {
                    monsterDataPanel.SetMonster(closestMonster);
                }
                else {
                    monsterDataPanel.Hide();
                }
            }
            else {
                closestMonster = null;
                if (monsterDataPanel != null) {
                    monsterDataPanel.Hide();
                }
            }
        }
        #endregion




        #region = Weapon And Attack =

        public void Attack(Vector3 v3) {
            currentWeapon.Attack();
            currentWeapon.ShowAimingArrow(false);
        }
        public void _PunchStart() {
            currentWeapon._AttackStart();
        }
        public void _PunchEnd() {
            currentWeapon._AttackEnd();
        }
        public void SwitchWeapon(GameObject weaponPrefab) {
            if (CurrentWeapon != null) {
                if (currentWeapon.data.weaponPrefab == fist) {
                    CurrentWeapon.SetActive(false);
                }
                else {
                    CurrentWeapon.transform.parent.gameObject.SetActive(false);
                }
            }

            CurrentWeapon = weaponPrefab;

            Joystick_Attack.CooldownTime = currentWeapon.data.cooldownTime;
            Joystick_Attack.CooldownWeapon();

            if (!WeaponList.ContainsKey(currentWeapon.data)) {
                GameObject obj = Instantiate(currentWeapon.data.weaponPrefab, weaponPosition.transform);
                CurrentWeapon = obj;
                WeaponList.Add(currentWeapon.data, obj);

                if (currentWeapon == fist) {
                    currentWeapon.animator = animator_player;
                }
            }
            else {
                CurrentWeapon = WeaponList[currentWeapon.data];
            }

            WeaponList[currentWeapon.data].SetActive(true);
        }

        #endregion


        #region = Save System =

        public void LoadData(GameData data) {

            PlayerData d = data.playerData;

            transform.position = d.position;
            Camera.main.transform.position = new Vector3(d.position.x, d.position.y, -20);
            Health = d.Health;

            // load weapon list
            WeaponList.Clear();
            for (int i = 0; i < d.weaponList.Length; i++) {
                Equipment_Weapon weapon = (FindItemByID.FindItem(d.weaponList[i]) as Equipment_Weapon);
                GameObject obj = Instantiate(weapon.weaponPrefab, weaponPosition.transform);

                WeaponList.Add(weapon, obj);
                obj.SetActive(false);

                if(weapon.ID == d.CurrenetWeaponID){
                    WeaponList[weapon].SetActive(true);
                    CurrentWeapon = WeaponList[weapon];
                    Joystick_Attack.CooldownTime = currentWeapon.data.cooldownTime;
                }
            }
        }

        public void SaveData(ref GameData data) {
            ref PlayerData d = ref data.playerData;

            d.CurrenetWeaponID = currentWeapon.data.ID;
            d.position = transform.position;
            d.Health = Health;
            d.weaponList = WeaponList.Keys.Select(w => w.ID).ToArray();
        }

        #endregion

    }
}