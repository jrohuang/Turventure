using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using LittleTurtle.Inventory;
using LittleTurtle.UI;

namespace LittleTurtle{
    public class Slider_QuantitySelect : MonoBehaviour {

        public Image Icon_Image;
        public TMP_Text Quantity_text;
        public Slider _slider;

        private static TMP_Text text;
        private static Image icon;
        private static Slider slider;
        private static GameObject GO;
        private static int maxQuantity;

        public static Action<Slot, int> Action_confirm;
        private static Item item;

        private GameObject itemInfoPanel;

        // activate before game start
        private void Awake() {
            text = Quantity_text;
            slider = _slider;
            icon = Icon_Image;

            GO = gameObject;
            slider = GetComponentInChildren<Slider>();
        }

        private void Start() {
            itemInfoPanel = Canvas_Game.Instance.itemInfoPanel.gameObject;
        }

        private static Slot slot;
        public static void DisplaySlider(Slot _slot, int _maxQuantity) {
            maxQuantity = _maxQuantity;
            slot = _slot;
            icon.sprite = _slot.item.image;
            text.text = 0.ToString();
            slider.value = 0;
            GO.SetActive(true);
        }

        public void _Confirm() {
            Action_confirm.Invoke(slot, int.Parse(text.text));
            Action_confirm = null;
            GO.SetActive(false);
        }
        public void _SliderChange() {
            text.text = slider.value == 0 ? 0.ToString() : (Mathf.Floor(slider.value / (1 / (float)(maxQuantity - 1)) + 1)).ToString();
        }
        public void _Add() {
            slider.value = (0.1f + (int.Parse(text.text) / ((float)maxQuantity - 1)));
        }
        public void _Subtract() {
            slider.value = ((int.Parse(text.text) - 1) * 1 / (float)maxQuantity);
        }
        
    }
}
