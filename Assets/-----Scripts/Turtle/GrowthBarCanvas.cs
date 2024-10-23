using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LittleTurtle.UI;

namespace LittleTurtle.Turtle{
    public class GrowthBarCanvas : MonoBehaviour{

        public Image image;
        public TMP_Text percent_Text, value_Text;
        public Animator animator;

        public Vector2 offset;

        private CurveLerping _lerping;
        private HealthBarPosCaculate HPbarPosCaculate;
        private TurtleCtrl turtleCtrl;

        void Start(){

            _lerping = ScriptableObject.CreateInstance<CurveLerping>();
            _lerping.SetAdjustVaule(image);

            Vector2 v2 = transform.parent.GetComponent<Collider2D>().bounds.size;
            HPbarPosCaculate = new HealthBarPosCaculate(v2.x, v2.y, transform.parent, gameObject, offset);
            turtleCtrl = TurtleCtrl.Instance;

        }

        void Update() {
            HPbarPosCaculate.UpdateHealthBarPosition();
            if (_lerping) _lerping.Update();
        }

        float displayAnimLength;
        IEnumerator OnGrowthChange() {

            yield return displayAnimLength;

            float f = turtleCtrl.Growth / turtleCtrl.MaxGrowth;
            percent_Text.text = (f * 100).ToString("0") + "%";
            value_Text.text = turtleCtrl.Growth.ToString() + "/" + turtleCtrl.MaxGrowth;
            _lerping.SetTarget(f, CurveLerping.CurveType.EaseOut_Slow);
        }

        public void Display() {
            animator.SetTrigger("Display");
            
            if (displayAnimLength == 0) {
                displayAnimLength = animator.GetCurrentAnimatorStateInfo(0).length;
            }

            StartCoroutine(OnGrowthChange());

            StopCoroutine(Hide());
            StartCoroutine(Hide());
        }
        IEnumerator Hide() {
            yield return new WaitForSeconds(3);
            animator.SetTrigger("Hide");
        }

    }
}
