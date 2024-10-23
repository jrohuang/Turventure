using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using LittleTurtle.UI;

namespace LittleTurtle.Inventory {

    public class PickableItemPrefab : MonoBehaviour, IPointerClickHandler {

        public Image image;
        public TMP_Text text;
        public int amount;
        private Item item;
        private Collider2D itemCollider;

        private TextLang textLang;

        public void SetInfo(Item item, int amount, Collider2D collider) {
            itemCollider = collider;
            this.amount = amount;
            this.item = item;
            image.sprite = item.image;
            textLang = text.GetComponent<TextLang>();
            textLang.SetMainText(item.Name);
            textLang.SetAdditionalText(amount.ToString() + " * ", -1);
        }

        public void OnPointerClick(PointerEventData eventData) {

            int i = Canvas_Game.Instance.bag.AddItem(item, amount);
            if (i == 0) {
                PickableItemsListMgr.Instance.TakeItem(itemCollider);
                Destroy(gameObject);
            }
            else if (i > 0 && i < amount) {
                amount -= i;
                textLang.SetAdditionalText(amount.ToString() + " * ", -1);
            }
            else if (i == amount) {
                print("can't pick up");
            }

            //int i = ItemSlotMgr.PickUpItem(item, amount);
            //if (i == amount) {
            //    PickableItemsListMgr.Instance.TakeItem(itemCollider);
            //}
            //else if (i < amount && i != -1) {
            //    amount -= -i;
            //    text.text = amount.ToString() + "  " + item.Name;
            //}
            //else if (i == -1) {
            //    print("slot of this item is full");
            //}
        }
    }

}