using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LittleTurtle.Enemy{

    [CreateAssetMenu(menuName = "--Scriptable Object / Monster Data")]
    public class MonsterData : ScriptableObject {

        [Header("---")]
        public int ID;
        public GameObject prefab;

        [Header("Ability")]
        public string monsterName;
        public float MaxHealth;
        public float moveSpeed;
        public AttackTarget attackTarget;
        public float attackTime;
        public float Damage;

        [Header("Other")]
        public AudioClip AudioClip_Attack;
        public Sprite icon;
    }

    public enum AttackTarget {
        player,
        none
    }
}
