using System.Collections.Generic;
using UnityEngine;
using LittleTurtle.Enemy;
using LittleTurtle.Inventory;

namespace LittleTurtle {
    public class FindItemByID : MonoBehaviour{

        private static Dictionary<int, Item> items;
        private static Dictionary<int, MonsterData> monsters;

        private void Awake() {
            items = new Dictionary<int, Item>();
            Object[] objects = Resources.LoadAll("Item", typeof(Item));
            foreach (var item in objects) {
                Item i = item as Item;
                items.Add(i.ID, i);
            }

            monsters = new Dictionary<int, MonsterData>();
            objects = Resources.LoadAll("MonsterData", typeof(MonsterData));
            foreach (var monster in objects) {
                MonsterData m = monster as MonsterData;
                monsters.Add(m.ID, m);
            }
        }

        public static Item FindItem(int id) {
            return items[id];
        }
        public static MonsterData GetMonsterByID(int id) {
            return monsters[id];
        }
    }
}
