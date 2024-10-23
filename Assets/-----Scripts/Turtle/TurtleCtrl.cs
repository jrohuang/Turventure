using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using LittleTurtle.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using UnityEngine.UI;
using LittleTurtle.Inventory;

namespace LittleTurtle.Turtle {
    public class TurtleCtrl : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDatapersistence {

        [Header("ability")]
        public string Name;
        public float moveSpeed;
        public float reachDistance;
        public float MaxHP;

        [Header("Path Finding")]
        public AIPath aiPath;
        public AIDestinationSetter setter;

        [Header("HP Bar")]
        public GameObject HPBarObj;
        public Vector2 HPBarOffset;
        public SpriteRenderer HPBarImage;

        [Header("Canvas")]
        public TMP_Text distanceText;

        [Header("Other")]
        public Animator animator;
        public GameObject highlightImage;

        public static TurtleCtrl Instance;

        public bool isReached { get => _isReached; set {
                _isReached = value;
                aiPath.maxSpeed = value ? 0 : moveSpeed;
                animator.SetBool("Move", !value);    
        } }
        private bool _isReached;
        private CurveLerping _lerping_HealthBar;
        private Image DirImage;
        private RectTransform DirImageParentRT;
        private float DirArrowPosDis;
        private Bag bag;

        [HideInInspector]
        public float HP { get => _HP; set { if (value <= MaxHP) _HP = value; } }
        public float MaxGrowth = 100;
        private float _HP;
        public float Growth { get => _growth; set => OnGrowthChange(value); }
        private float _growth;


        public TurtleState State { get => _state; set { _state = value; OnTurtleStateChange(); } }
        private TurtleState _state;

        [HideInInspector]
        public bool IsOnScreen {
            get => _isOnScreen; set {
                if (value != _isOnScreen) {
                    _isOnScreen = value;
                    OnIsOnScreenChange();
                }
            }
        }
        private bool _isOnScreen;

        private PlayerCtrl playerCtrl;
        private TurtleInfoPanel infoPanel;
        private HealthBarPosCaculate HPBarCaculate;
        private void Awake() {
            if (!Instance) Instance = this;
        }

        void Start(){
            HP = MaxHP;
            playerCtrl = PlayerCtrl.Instance;
            setter.target = playerCtrl.transform;
            
            infoPanel = Canvas_Game.Instance.turtleInfoPanel;
            DirImage = Canvas_Game.Instance.turtleDirArrowIcon;
            
            bag = Canvas_Game.Instance.bag;
            DirImageParentRT = DirImage.transform.parent.GetComponent<RectTransform>();
            DirArrowPosDis = DirImageParentRT.sizeDelta.x / 2.3f;

            Vector2 v2 = GetComponent<Collider2D>().bounds.size;
            HPBarCaculate = new HealthBarPosCaculate(v2.x, v2.y, transform, HPBarObj, HPBarOffset);
            
            _lerping_HealthBar = ScriptableObject.CreateInstance<CurveLerping>();
            _lerping_HealthBar.SetAdjustVaule(HPBarImage);
            State = TurtleState.following;
        }

        void Update() {
            
            Vector3 v3 = Camera.main.WorldToViewportPoint(transform.position);
            IsOnScreen = (v3.x > 0 && v3.x < 1 && v3.y > 0 && v3.y < 1) ? true : false;

            if (State == TurtleState.following) {
                isReached = aiPath.remainingDistance < reachDistance ? true : false;
            }

            HPBarCaculate.UpdateHealthBarPosition();
            _lerping_HealthBar.Update();
            
            // show distance and dir arrow icon
            if (!IsOnScreen) {
                float f = Vector2.Distance(transform.position, playerCtrl.transform.position) * 0.6f;
                distanceText.text = f > 10 ? f.ToString("0") + "m" : f.ToString("0.0") + "m";

                Vector2 v2 = (transform.position - playerCtrl.transform.position).normalized;
                DirImage.rectTransform.anchoredPosition = v2 * DirArrowPosDis;
                DirImage.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg - 90);
            }

            // temp
            if (Input.GetKeyDown(KeyCode.P)) {
                GetHurt(1);
            }
        }


        public Action Action_OnGrowthChange;
        private void OnGrowthChange(float nutrition) {
            _growth = nutrition;

            if (Growth > MaxGrowth) {
                print("level up !");
                // 
            }
            else {

            }

            Action_OnGrowthChange?.Invoke();
        }

        public void OnIsOnScreenChange() {
            if (IsOnScreen) {
                distanceText.text = "";
                DirImage.gameObject.SetActive(false);
            }
            else {
                DirImage.gameObject.SetActive(true);
            }
        }

        private void OnTurtleStateChange() {
            switch (State) {
                case TurtleState.following:
                    aiPath.maxSpeed = moveSpeed;
                    animator.SetBool("Move", true);
                    animator.SetBool("Hide", false);
                    break;

                case TurtleState.resting:
                    aiPath.maxSpeed = 0;
                    animator.SetBool("Move", false);
                    animator.SetBool("Hide", false);
                    break;

                case TurtleState.hiding:
                    aiPath.maxSpeed = 0;
                    animator.SetBool("Move", false);
                    animator.SetBool("Hide", true);
                    break;
            }
        }

        public void GetHurt(float damage) {
            if (HP - damage > 0) {
                HP -= damage;
                infoPanel.OnHPChange();
                _lerping_HealthBar.SetTarget(HP / MaxHP, CurveLerping.CurveType.EaseOut_Slow);
            }
            else {
                HP = 0;
                print("turtle die");
            }
        }


        public bool isHighlighting { get => _isHighlighting; set { _isHighlighting = value; highlightImage.SetActive(value); } }
        private bool _isHighlighting;

        public GrowthBarCanvas growthBar;
        
        public void Feed(Slot slot) {
            Slider_QuantitySelect.DisplaySlider(slot, slot.Quantity);
            Slider_QuantitySelect.Action_confirm += FeedConfirm;
        }
        public void FeedConfirm(Slot slot, int quantity) {
            if (quantity == 0) {
                return;
            }

            growthBar.Display();
            Growth += slot.item.Nutrition * quantity;

            if (slot.slotType == SlotType.bagSlot) {
                bag.RemoveItem(slot.item, quantity);
            }
            else if (slot.slotType == SlotType.equipmentSlot) {
                if (bag.Items_Equipment[slot.item].Quantity == 1) {
                    bag.Items_Equipment[slot.item].Quantity = 0;
                }
                else {
                    bag.Items_Equipment[slot.item].Quantity -= quantity;
                }
            }
        }
        


        public void OnPointerEnter(PointerEventData eventData) {
            if (bag.draggingItem || bag.draggingItem_NotInBag) {
                highlightImage.SetActive(true);
                isHighlighting = true;
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (bag.draggingItem || bag.draggingItem_NotInBag) {
                highlightImage.SetActive(false);
                isHighlighting = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            infoPanel.gameObject.SetActive(true);
        }

        public void LoadData(GameData data) {
            TurtleData d = data.turleData;

            MaxHP = d.MaxHP;
            MaxGrowth = d.MaxGrowth;
            Name = d.Name;
            moveSpeed = d.moveSpeed;
            HP = d.HP;
            Growth = d.Growth;

            infoPanel.OnHPChange();
            _lerping_HealthBar.SetTarget(HP / MaxHP, CurveLerping.CurveType.EaseOut_Slow);

            transform.position = d.position;
            transform.rotation = d.rotation;
            infoPanel._StateSwitch(d.TurtleState);
        }

        public void SaveData(ref GameData data) {
            ref TurtleData d = ref data.turleData;

            d.Name = Name;
            d.moveSpeed = moveSpeed;
            d.HP = HP;
            d.Growth = Growth;

            d.TurtleState = (int)State;
            d.position = transform.position;
            d.rotation = transform.rotation;

            d.MaxHP = MaxHP;
            d.MaxGrowth = MaxGrowth;
        }

        public enum TurtleState {
            following,
            resting,
            hiding
        }
    }
}
