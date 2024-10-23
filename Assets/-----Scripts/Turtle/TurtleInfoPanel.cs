using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LittleTurtle.Turtle;

namespace LittleTurtle.UI {
    public class TurtleInfoPanel : MonoBehaviour{

        public TMP_Text name_Text;
        public TMP_Text type_Text;
        public TMP_Text desc_Text;
        public RawImage RawImage;

        [Header("HP Bar / Growth Var")]
        public Image HPBar;
        public Image GrowthBar;
        public TMP_Text HP_percent, HP_Value;
        public TMP_Text Growth_percent, Growth_Value;

        [Header("State Button")]
        public Button Btn_Following;
        public Button Btn_Resting;
        public Button Btn_Hiding;

        //
        private TurtleCtrl turtleCtrl;
        private CurveLerping _lerping_HealthBar, _lerping_GrowthBar;
        //

        private void OnEnable() {
            OnHPChange();
            OnGrowthChange();
        }

        // activate by Canvas_Game
        private void Awake() {
            turtleCtrl = TurtleCtrl.Instance;

            _lerping_HealthBar = ScriptableObject.CreateInstance<CurveLerping>();
            _lerping_HealthBar.SetAdjustVaule(HPBar);
            _lerping_GrowthBar = ScriptableObject.CreateInstance<CurveLerping>();
            _lerping_GrowthBar.SetAdjustVaule(GrowthBar);

            Btn_Following.interactable = false;
        }

        private void Start() {
            turtleCtrl.Action_OnGrowthChange += OnGrowthChange;
        }

        private void Update() {
            _lerping_HealthBar.Update();
            _lerping_GrowthBar.Update();
        }


        public void OnHPChange() {
            float f = turtleCtrl.HP / turtleCtrl.MaxHP;
            HP_percent.text = (f * 100).ToString("0") + "%";
            HP_Value.text = turtleCtrl.HP.ToString() + "/" + turtleCtrl.MaxHP;
            _lerping_HealthBar.SetTarget(f, CurveLerping.CurveType.EaseOut_Slow);
        }
        private void OnGrowthChange() {
            float f = turtleCtrl.Growth / turtleCtrl.MaxGrowth;
            Growth_percent.text = (f * 100).ToString("0") + "%";
            Growth_Value.text = turtleCtrl.Growth.ToString() + "/" + turtleCtrl.MaxGrowth;
            _lerping_GrowthBar.SetTarget(f, CurveLerping.CurveType.EaseOut_Slow);
        }


        #region = Button =
        public void _StateSwitch(int state) {
            turtleCtrl.State = (TurtleCtrl.TurtleState)state;

            Btn_Following.interactable = true;
            Btn_Resting.interactable = true;
            Btn_Hiding.interactable = true;
            switch (state) {
                case (int)TurtleCtrl.TurtleState.following:
                    Btn_Following.interactable = false;
                    break;
                case (int)TurtleCtrl.TurtleState.resting:
                    Btn_Resting.interactable = false;
                    break;
                case (int)TurtleCtrl.TurtleState.hiding:
                    Btn_Hiding.interactable = false;
                    break;
             
            }
        }
        #endregion



    }
}
