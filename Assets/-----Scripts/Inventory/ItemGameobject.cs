using UnityEngine;
using LittleTurtle.System_;
namespace LittleTurtle.Inventory {

    [System.Serializable]
    public class ItemGameobject : MonoBehaviour{
        public Item item;
        public int amount;
        private void Awake() {
            SaveDataMgr.AddPickableItem(this);
        }
        private void OnDestroy() {
            SaveDataMgr.RemovePickableItem(this);
        }
    }

}
