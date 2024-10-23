using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LittleTurtle.Inventory;
using LittleTurtle.Turtle;
using UnityEngine.UI;

namespace LittleTurtle.UI {

    public class ItemInfoPanel : MonoBehaviour{

        public TextLang Name_Text;
        public TextLang ItemType_Text;
        public TextLang Quantity_Text;
        public TextLang Description_Text;
        public TextLang Nutrition_Text;
        public Image icon_Image;
        public GameObject Use_Btn, Drop_Btn, Unequip_Btn;

        public Item itemSelected;

        private Slot slot;
        private Bag bag;
        private TurtleCtrl turtleCtrl;

        private void Awake() {
            if (Canvas_Game.Instance) bag = Canvas_Game.Instance.bag;
        }

        private void Start() {
            bag = Canvas_Game.Instance.bag;
            turtleCtrl = TurtleCtrl.Instance;
        }

        public void ShowItemInfo(Slot slot) {
            
            gameObject.SetActive(true);

            this.slot = slot;
            itemSelected = slot.item;

            icon_Image.sprite = itemSelected.image;
            Use_Btn.SetActive(itemSelected.Usable);

            Name_Text.SetMainText(itemSelected.Name);
            Description_Text.SetMainText(itemSelected.Description);
            ItemType_Text.SetMainText(itemSelected.type.ToString());
            ItemType_Text.SetAdditionalText("---", -1);

            Quantity_Text.SetMainText(slot.Quantity.ToString());
            Quantity_Text.SetAdditionalText(" : ", -1);
            Quantity_Text.SetAdditionalText("Quantity", -2);

            Nutrition_Text.SetMainText(itemSelected.Nutrition.ToString());
            Nutrition_Text.SetAdditionalText(" : ", -1);
            Nutrition_Text.SetAdditionalText("Nutrition", -2);

            if (slot.item.type == ItemType.Weapon) {
                if (bag.WeaponSlot.item) {
                    if (bag.WeaponSlot.item == slot.item) {
                        Use_Btn.SetActive(false);
                        Unequip_Btn.SetActive(true);
                        UnequipItemType = ItemType.Weapon;
                    }
                }
                else {
                    Use_Btn.SetActive(true);
                    Unequip_Btn.SetActive(false);
                }
            }
            else {
                Unequip_Btn.SetActive(false);
            }

        }

        ItemType UnequipItemType;

        public void UpdateAmount() {
            if (slot) {
                Quantity_Text.SetMainText(slot.Quantity.ToString());
                Quantity_Text.SetAdditionalText(" : ", -1);
                Quantity_Text.SetAdditionalText("Quantity", -2);
            }
        }

        public void _UnEquip() {
            switch (UnequipItemType) {
                case ItemType.Weapon:
                    bag.WeaponSlot.UnEquip(slot.Quantity, true);
                    bag.WeaponSlot.item = null;
                    break;
            }
            gameObject.SetActive(false);/////
        }
        public void _Use() {
            Equipment_Weapon _Weapon = (Equipment_Weapon)slot.item;
            _Weapon.Use();
        }
        public void _Feed() {
            if (slot.Quantity > 1) {
                turtleCtrl.Feed(slot);
            }
            else {
                turtleCtrl.FeedConfirm(slot, 1);
            }
        }
    }
}
