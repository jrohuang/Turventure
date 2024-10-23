using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LittleTurtle.Turtle;

namespace LittleTurtle.Enemy {

    public class MonsterWeapon : MonoBehaviour{

        public float destroyTime;
        private bool isAnimationFinished = false;
        private List<Collider2D> damagedCollisions = new List<Collider2D>();
        private Monster monster;

        private PlayerCtrl playerCtrl;
        private TurtleCtrl turtleCtrl;

        private void Start() {
            playerCtrl = PlayerCtrl.Instance;
            turtleCtrl = TurtleCtrl.Instance;
        }

        public void SetInfo(Monster monster) {
            this.monster = monster;
        }

        public void _AttackFinished() {
            isAnimationFinished = true;

            if (monster != null) {
                monster.AttackFinished();
            }

            StartCoroutine(DestroyObject());
        }


        public void _Destroy() {
            Destroy(transform.parent.gameObject);
        }
        IEnumerator DestroyObject() {
            yield return new WaitForSeconds(destroyTime);
            GetComponent<Animator>().SetTrigger("Destroy");
        }


        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.tag == "Player") {
                if (!damagedCollisions.Contains(collision) & !isAnimationFinished) {
                    damagedCollisions.Add(collision);
                    playerCtrl.GetHurt(monster.data.Damage);
                }
            }

            if (collision.tag == "Turtle") {
                if (!damagedCollisions.Contains(collision) & !isAnimationFinished) {
                    damagedCollisions.Add(collision);
                    turtleCtrl.GetHurt(monster.data.Damage);
                }
            }
        }
    }
}
