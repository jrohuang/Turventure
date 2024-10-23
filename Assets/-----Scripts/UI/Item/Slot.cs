using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LittleTurtle.Inventory;
using UnityEngine.UI;
using TMPro;
using LittleTurtle.UI;
using UnityEngine.EventSystems;
using LittleTurtle.Turtle;

namespace LittleTurtle {
    public class Slot : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler {

        public SlotType slotType;
        public int Quantity { get => quantity; set { if (quantity != value) OnAmountChange(value, quantity); } }
        private int quantity;

        public Image image_icon;
        public TMP_Text quantity_Text;
        public Image image_ForDrag;

        [HideInInspector]
        public Item item;
        protected Bag bag;
        protected ItemInfoPanel infoPanel;

        protected virtual void Awake() {
            bag = Canvas_Game.Instance.bag;
            infoPanel = Canvas_Game.Instance.itemInfoPanel;
        }

        public virtual void Start() {}

        public virtual void OnPointerClick(PointerEventData eventData) {
            if (item != null) {
                infoPanel.ShowItemInfo(this);
            }
        }
        public void OnDrag(PointerEventData eventData) {
            image_ForDrag.rectTransform.position = eventData.position;
        }

        public virtual void OnBeginDrag(PointerEventData eventData) {
            image_ForDrag.color = Color.white;
        }
        public virtual void OnEndDrag(PointerEventData eventData) {
            // The method which overrides this method needs to perform actions before calling base.OnEndDrag

            // feed turtle 
            if (TurtleCtrl.Instance.isHighlighting) {
                bag.draggingItem_NotInBag = null;
                if (Quantity > 1) {
                    TurtleCtrl.Instance.Feed(this);
                }
                else {
                    TurtleCtrl.Instance.FeedConfirm(this, 1);
                }
            }
            TurtleCtrl.Instance.isHighlighting = false;

            image_ForDrag.color = Color.clear;
        }

        public virtual void OnAmountChange(int newQuantity, int previous) {
            quantity = newQuantity;
            quantity_Text.text = Quantity > 1 ? Quantity.ToString() : "";
        }

        
    }
    public enum SlotType {
        bagSlot,
        equipmentSlot,
        other
    }
}
