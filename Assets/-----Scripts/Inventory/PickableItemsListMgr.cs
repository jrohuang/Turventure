using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LittleTurtle.Inventory {

    public class PickableItemsListMgr : MonoBehaviour {

        public static PickableItemsListMgr Instance;
        public GameObject pickablePrefab;

        public Dictionary<Collider2D, PickableItemPrefab> list = new Dictionary<Collider2D, PickableItemPrefab>();
        private PlayerCtrl playerCtrl;

        private void Awake() {
            if (Instance == null) Instance = this;
        }
        private void Start() {
            playerCtrl = PlayerCtrl.Instance;
        }

        public void AddItem(Collider2D collider) {
            ItemGameobject item = collider.GetComponent<ItemGameobject>();
            PickableItemPrefab prefab = Instantiate(pickablePrefab, transform).GetComponent<PickableItemPrefab>();
            prefab.SetInfo(item.item, item.amount, collider);
            list.Add(collider, prefab);
        }
        public void LeaveItem(Collider2D collider) {
            RemoveItem(collider);
        }
        public void TakeItem(Collider2D collider) {
            RemoveItem(collider);
            Destroy(collider.gameObject);
        }

        public void RemoveItem(Collider2D collider) {
            list.TryGetValue(collider, out var prefab);
            list.Remove(collider);
            Destroy(prefab.gameObject);
        }
    }

}