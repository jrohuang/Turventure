using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LittleTurtle.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LittleTurtle.Inventory{

    public class InventoryPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        public static bool isTouching = false;
        public Image _image;
        public static Image image;

        private Bag bag;
        private void Awake() {
            image = _image;

            if (Canvas_Game.Instance) {
                bag = Canvas_Game.Instance.bag;
            }
        }

        private void Start() {
            if (bag == null && Canvas_Game.Instance) {
                bag = Canvas_Game.Instance.bag;
            }
        }


        public void OnPointerEnter(PointerEventData eventData) {
            isTouching = true;

            if (bag.draggingItem_NotInBag) {
                image.color = Color.black;
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            isTouching = false;
            image.color = Color.white;
        }
    }
}
