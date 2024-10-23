using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using LittleTurtle.UI;

namespace LittleTurtle.Inventory {

    public class QuickItemSlot : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

        public TMP_Text Text_Amount;
        public Image Image_ItemIcon;
        public Item item;

        public int Amount { get { return amount; } set { amount = value; if(value != -1) UpdateAmountText(); } }
        private int amount;  //  -1 menas slot is null
        private ItemInfoPanel ItemInfoPanel;

        private void Start() {
            ItemInfoPanel = Canvas_Game.Instance.itemInfoPanel;
        }

        public void Initialize() {
            Amount = -1;
            Text_Amount.text = "";
            Image_ItemIcon.enabled = false;
        }

        public void AddItem(Item item, int amount) {
            this.item = item;
            Image_ItemIcon.enabled = true;
            Image_ItemIcon.sprite = item.image;
            Amount = amount;
            // create dictionary to save items
        }
        public void TakeItem(Item item, int amount) {
            // amount = 0 Image_ItemIcon.enabled = false;
        }

        public void UpdateAmountText() {
            
            Text_Amount.text = amount.ToString();


            // update item info panel if this item was selected
            if (ItemInfoPanel) {
                if (ItemInfoPanel.itemSelected == item) {
                    ItemInfoPanel.UpdateAmount();
                }
            }
        }

        public void OnDrag(PointerEventData eventData) {
        }

        public void OnPointerUp(PointerEventData eventData) {
            //if (item != null) {
            //    ItemInfoPanel.ShowItemInfo(this);
            //}
        }

        public void OnPointerDown(PointerEventData eventData) {
        }
    }
}