using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LittleTurtle.UI;

namespace LittleTurtle.Inventory {

    public class ItemSlotMgr : MonoBehaviour{

        public static QuickItemSlot[] itemSlots;

        void Start() {
            Transform Slots = transform.Find("ItemSlots");
            itemSlots = new QuickItemSlot[Slots.childCount];
            for (int i = 0; i < Slots.childCount; i++) {
                itemSlots[i] = Slots.GetChild(i).GetComponent<QuickItemSlot>();
            }

            foreach (var item in itemSlots) {
                item.Initialize();
            }
        }

        // return how much can get, -1 means already full
        public static int PickUpItem(Item item, int amount) {

            // Check if item already exists in slots
            foreach (var i in itemSlots) {
                if (i.Amount != -1) {
                    if (i.item == item) {
                        int totalAmount = i.Amount + amount;

                        if (totalAmount < 100) {
                            i.Amount += amount;
                            return amount;
                        }
                        else {
                            i.Amount = 99;
                            return totalAmount - 99;
                        }
                    }
                    print(i.item.Name + " / " + item.Name);
                }
            }

            // Add item to Slots
            foreach (var i in itemSlots) {
                if (i.Amount == -1) {
                    i.AddItem(item, (amount < 100 ? amount : 99));
                    return (amount < 100 ? amount : 99);
                }
            }

            // Slots is full
            return -1;
        }
    }


}