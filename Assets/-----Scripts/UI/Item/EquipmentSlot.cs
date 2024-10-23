using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LittleTurtle.UI;

namespace LittleTurtle.Inventory {
    public class EquipmentSlot : Slot, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler{

        public Sprite defultSprite;
        public ItemType type;
        public Image image_highlightImage;

        private PlayerCtrl playerCtrl;

        protected override void Awake() {
            base.Awake();
            if (playerCtrl == null) playerCtrl = PlayerCtrl.Instance;
            
        }

        public override void Start() {
            base.Start();
            if (playerCtrl == null) playerCtrl = PlayerCtrl.Instance;
            image_highlightImage.color = Color.clear;
        }
        public override void OnBeginDrag(PointerEventData eventData) {
            base.OnBeginDrag(eventData);
            bag.draggingItem_NotInBag = item;
        }
        public override void OnEndDrag(PointerEventData eventData) {

            base.OnEndDrag(eventData);

            if (InventoryPanel.isTouching) {
                Quantity = 0;
            }

            if (InventoryPanel.image) {
                InventoryPanel.image.color = Color.white;
            }

            bag.draggingItem_NotInBag = null;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (bag.draggingItem) {
                if (bag.draggingItem.type == type) {
                    image_highlightImage.color = bag.color_blue;
                }
                else {
                    image_highlightImage.color = bag.color_red;
                }

                bag.highlightEquipmentSlot = this;
            }
        }
        public void OnPointerExit(PointerEventData eventData) {
            if (image_highlightImage.color != Color.clear) {
                image_highlightImage.color = Color.clear;
            }

            bag.highlightEquipmentSlot = null;
        }

        public void Equip(Equipment_Weapon weapon, bool isLoadingData) {

            if (bag.draggingItem && bag.draggingItem.type != type &! isLoadingData) {
                return;
            }

            // if weapon equipped
            if (item != null) {
                bag.AddItem(item, Quantity);
                if(bag.Items_Equipment.ContainsKey(item)) bag.Items_Equipment.Remove(item);
            }

            item = weapon;

            // equip
            if (!isLoadingData) {
                Quantity = bag.Items[weapon].Quantity;
                infoPanel.ShowItemInfo(this);
                infoPanel.gameObject.SetActive(true);
                Canvas_Game.Instance.bag.RemoveItem(weapon, Quantity);
                playerCtrl.SwitchWeapon(weapon.weaponPrefab);
            }

            // item record
            if (!isLoadingData) {
                bag.Items_Equipment.Add(weapon, this);
            }

            // ui setting
            image_highlightImage.color = Color.clear;
            image_icon.sprite = weapon.image;
            image_icon.color = Color.white;
            image_ForDrag.sprite = weapon.image;
            image_ForDrag.color = Color.clear;
            quantity_Text.text = Quantity == 1 ? null : Quantity.ToString();
        }

        public void UnEquip(int _quantity, bool PutBackinBag) {

            if (Quantity == 0) {
                bag.Items_Equipment.Remove(item);
                playerCtrl.SwitchWeapon(playerCtrl.fist);
                image_icon.sprite = defultSprite;
                image_icon.color = new Color(1, 1, 1, 165f / 255);
            }

            if (PutBackinBag) {
                bag.AddItem(item, _quantity);
                Quantity = 0;
            }
            
            image_highlightImage.color = Color.clear;
            image_icon.color = Color.white;
            image_ForDrag.color = Color.clear;
            infoPanel.gameObject.SetActive(false);

            if (Quantity == 0) {
                item = null;
            }
        }

        public override void OnAmountChange(int newQuantity, int previous) {
            base.OnAmountChange(newQuantity, previous);
            if (Quantity == 0) {
                if (bag.draggingItem_NotInBag) {
                    UnEquip(previous - newQuantity, true);
                }
                else {
                    UnEquip(0, false);
                }
            }
        }
    }
}
