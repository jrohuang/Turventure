using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LittleTurtle.Inventory {
    public class Bag : MonoBehaviour, IDatapersistence {

        public GridLayoutGroup layoutGroup;
        public GameObject bagSlotPrefab;
        public Mask mask_ItemsPanel;

        [Header("HighlightColor")]
        public Color color_blue;
        public Color color_red;

        public EquipmentSlot WeaponSlot;
        public EquipmentSlot ArmorSlot;

        public Dictionary<Item, BagSlotPrefab> Items = new Dictionary<Item, BagSlotPrefab>();
        public Dictionary<Item, EquipmentSlot> Items_Equipment = new Dictionary<Item, EquipmentSlot>();

        void Start() {
            StartCoroutine(FitCellSize());
        }


        #region = Add and Remove Item =
        // return number of item can't pick up
        public int AddItem(Item newItem, int Increment) {
            if (!Items.ContainsKey(newItem)) {
                // item doesn't exists => add slot
                AddSlot(newItem, Increment);

                return 0;
            }
            else {
                // item exists
                BagSlotPrefab slot = Items[newItem];
                if (slot.Quantity < 99) {
                    // 99 - slot.Amount - Increment => the rest quantity after item increase
                    if (99 - slot.Quantity - Increment >= 0) {
                        slot.Quantity += Increment;
                        return 0;
                    }
                    else {
                        slot.Quantity = 99;
                        return Increment - slot.Quantity;
                    }
                }
                else {
                    print("the number of items is full");
                    return Increment;
                }

            }


        }
        public void RemoveItem(Item item, int Decrement) {
            if (Items.ContainsKey(item)) {

                if (Items[item].Quantity > Decrement) {
                    BagSlotPrefab slot = Items[item];
                    slot.Quantity -= slot.Quantity > Decrement ? Decrement : slot.Quantity;
                }
                else {
                    Slot s = Items[item];
                    Items.Remove(item);
                    Destroy(s.gameObject);
                }



            }
            else {
                Debug.Log("Error : No Item to Remove");
            }
        }
        #endregion


        #region = Add and Remove Slot =
        private void AddSlot(Item item, int quantity) {
            BagSlotPrefab slotPrefab = Instantiate(bagSlotPrefab, layoutGroup.transform).GetComponent<BagSlotPrefab>();
            slotPrefab.SetInfo(item);
            Items.Add(item, slotPrefab);
            Items[item].Quantity += quantity;
        }
        public void RemoveSlot(Item ItemToRemove) {
            if (Items.ContainsKey(ItemToRemove)) {

                BagSlotPrefab slot = Items[ItemToRemove];
                Items.Remove(ItemToRemove);
                Destroy(slot.gameObject);
            }
        }

        #endregion

        [HideInInspector] public Item draggingItem;
        [HideInInspector] public Item draggingItem_NotInBag;
        [HideInInspector] public EquipmentSlot highlightEquipmentSlot;
        public void ToggleMaskOfItemsPanel(bool enable) {
            mask_ItemsPanel.enabled = enable;
        }
        private IEnumerator FitCellSize() {
            yield return null;

            RectTransform rt = layoutGroup.GetComponent<RectTransform>();
            float f = (rt.rect.width - layoutGroup.spacing.x * 2 - layoutGroup.padding.left - layoutGroup.padding.right) / 3;
            layoutGroup.cellSize = new Vector2(f, f);
        }


        public void LoadData(GameData data) {

            InventoryData d = data.inventoryData;

            // bag items
            for (int i = 0; i < d.BagItems.Length; i++) {
                AddItem(FindItemByID.FindItem(d.BagItems[i][0]), d.BagItems[i][1]);
            }

            //equipment items
            foreach (var item in d.EquipmentItems) {
                Item equipment = FindItemByID.FindItem(item[0]);
                switch (equipment.type) {
                    case ItemType.Weapon:
                        WeaponSlot.Equip(equipment as Equipment_Weapon, true);
                        WeaponSlot.Quantity = item[1];

                        Items_Equipment.Add(equipment, WeaponSlot);
                        break;

                    case ItemType.Armor:
                        //ArmorSlot.Equip(equipment as equipment_armor);
                        break;
                }
            }

        }

        public void SaveData(ref GameData data) {
            ref InventoryData d = ref data.inventoryData;

            // bag items
            d.BagItems = new int[Items.Count][];
            int index = 0;
            foreach (var item in Items) {
                d.BagItems[index] = new int[2];
                d.BagItems[index][0] = item.Key.ID;
                d.BagItems[index][1] = item.Value.Quantity;

                index++;
            }

            //equipment items
            d.EquipmentItems = new int[Items_Equipment.Count][];
            index = 0;
            foreach (var item in Items_Equipment) {
                d.EquipmentItems[index] = new int[2];

                d.EquipmentItems[index][0] = item.Key.ID;
                d.EquipmentItems[index][1] = item.Value.Quantity;
            }

        }
    }
}
