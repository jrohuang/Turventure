using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LittleTurtle.Player.Weapon;
using LittleTurtle.UI;

namespace LittleTurtle.Inventory{

    [CreateAssetMenu(menuName = "--Scriptable Object / Item / Equipment_Weapon")]
    public class Equipment_Weapon : Item {

        [Header("Vaule")]
        public float Damage;
        public float cooldownTime;

        [Header("Audio")]
        public AudioClip audio_Attack;
        public AudioClip audio_Hit;

        public GameObject weaponPrefab;

        public Item GetItemData() {
            return this;
        }
        public void Use() {
            Canvas_Game.Instance.bag.WeaponSlot.Equip(this, false);
        }
    }

}
