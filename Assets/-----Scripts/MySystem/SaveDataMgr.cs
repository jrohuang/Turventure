using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LittleTurtle.Enemy;
using LittleTurtle.Inventory;

namespace LittleTurtle.System_{

    public class SaveDataMgr : MonoBehaviour, IDatapersistence{

        public GameObject MonsterGroupForTest;
        private static GameObject monsterGroup;
        
        public static List<Monster> monsterList = new List<Monster>();
        private static List<ItemGameobject> List_ItemGameobjects = new List<ItemGameobject>();

        private void Awake() {
            monsterGroup = MonsterGroupForTest;
        }

        public static void CreateNewGame() {
            Instantiate(monsterGroup);
        }

        public static void AddPickableItem(ItemGameobject item) {
            List_ItemGameobjects.Add(item);
        }
        public static void RemovePickableItem(ItemGameobject item) {
            if (List_ItemGameobjects.Contains(item)) {
                List_ItemGameobjects.Remove(item);
            }
        }

        public void LoadData(GameData data) {
            print("load monster and item data");
            monsterList.ForEach(m => Destroy(m));
            monsterList.Clear();

            _MonsterData[] d = data.monsters;

            foreach (var monsterData in d) {
                Monster m = Instantiate(FindItemByID.GetMonsterByID(monsterData.ID).prefab).GetComponent<Monster>();
                m.LoadData(monsterData);
            }

            List_ItemGameobjects = new List<ItemGameobject>();
            foreach (var item in data.itemGameobjects) {
                Instantiate(FindItemByID.FindItem(item.ID).prefab, item.position, Quaternion.identity, transform);
            }
        }

        public void SaveData(ref GameData data) {
            ref _MonsterData[] data_monster = ref data.monsters;
            data_monster = new _MonsterData[monsterList.Count];

            int i = 0;
            foreach (var monster in monsterList) {
                data_monster[i] = monster.GetData();
                i++;
            }

            ref List<PickableItemData> data_PickableItem = ref data.itemGameobjects;

            foreach (var item in List_ItemGameobjects) {
                PickableItemData itemGameobjectData = new PickableItemData();
                itemGameobjectData.ID = item.item.ID;
                itemGameobjectData.position = item.transform.position;
                itemGameobjectData.Quantity = item.amount;
                data_PickableItem.Add(itemGameobjectData);
            }
        }
        
    }
}
