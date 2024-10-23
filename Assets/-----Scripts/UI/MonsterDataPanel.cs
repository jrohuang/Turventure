using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using LittleTurtle.Enemy;

namespace LittleTurtle.UI {
    public class MonsterDataPanel : MonoBehaviour{

        public Image Icon_Image;
        public TextLang Name_Text;
        public Image HealthBar_Image;
        public TMP_Text Health_Text;
        public TMP_Text Attack_Text;

        Monster monster;

        public void UpdateInfo() {
            Icon_Image.sprite = monster.data.icon;

            Name_Text.SetMainText(monster.data.monsterName);
            Attack_Text.text = monster.data.Damage.ToString();

            Health_Text.text = monster.health + " / " + monster.data.MaxHealth;
            HealthBar_Image.fillAmount = monster.health / monster.data.MaxHealth;
        }
        
        public void SetMonster(Monster monster) {
            this.monster = monster;
            UpdateInfo();
            gameObject.SetActive(true);
        }
        public void Show() {
            gameObject.SetActive(true);
        }
        public void Hide() {
            gameObject.SetActive(false);
        }

    }
}
