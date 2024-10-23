using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LittleTurtle.UI;

namespace LittleTurtle.Inventory {
    public class BagSlotPrefab : Slot, IBeginDragHandler, IEndDragHandler{

        protected override void Awake() {
            base.Awake();
            if (!bag && Canvas_Game.Instance) {
                bag = Canvas_Game.Instance.bag;
            }
        }

        public void SetInfo(Item item) {
            this.item = item;
            image_icon.sprite = item.image;
            image_ForDrag.sprite = item.image;
            quantity_Text.text = Quantity.ToString();

            infoPanel = Canvas_Game.Instance.itemInfoPanel;
            image_ForDrag.color = Color.clear;
        }

        public override void OnPointerClick(PointerEventData eventData) {
            base.OnPointerClick(eventData);
        }
        
        public override void OnBeginDrag(PointerEventData eventData) {
            base.OnBeginDrag(eventData);
            bag.draggingItem = item;
            bag.ToggleMaskOfItemsPanel(false);
        }
        public override void OnEndDrag(PointerEventData eventData) {
            if (bag.highlightEquipmentSlot) {
                if (item.type == ItemType.Weapon) {
                    bag.highlightEquipmentSlot.Equip(item as Equipment_Weapon, false);
                }
                else {
                    bag.highlightEquipmentSlot.image_highlightImage.color = Color.clear;
                }
            }
            base.OnEndDrag(eventData);
            bag.draggingItem = null;
            bag.ToggleMaskOfItemsPanel(true);//
        }



        public override void OnAmountChange(int newQuantity, int previous) {
            base.OnAmountChange(newQuantity, previous);

            if (Quantity == 0) {
                bag.Items.Remove(item);
                Destroy(gameObject);
            }

            infoPanel.UpdateAmount();
        }
    }
}
