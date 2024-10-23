using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LittleTurtle.Player.Weapon;

namespace LittleTurtle.Inventory {

    [CreateAssetMenu(menuName = "--Scriptable Object / Item")]
    public class Item : ScriptableObject {

        [Header("---")]
        public GameObject prefab;
        public int ID;
        public string Name;
        public ItemType type;

        [Header("---")]
        public float Nutrition;
        public bool Usable;

        [Header("---")]
        public Sprite image;
        public string Description;
    }

    public enum ItemType {
        MonsterDrop,
        Resource,
        Weapon,
        Armor
    }

}