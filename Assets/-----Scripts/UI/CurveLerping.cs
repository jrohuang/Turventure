using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LittleTurtle.System_;

namespace LittleTurtle.UI{

    [System.Serializable]
    public class Curves {
        public AnimationCurve Curve_EaseOut_Fast;
        public AnimationCurve Curve_EaseOut_Slow;
    }

    [CreateAssetMenu(menuName = "--Scriptable Object / CurveLerping")]
    public class CurveLerping : ScriptableObject{

        [SerializeField]
        public Curves curves;

        [HideInInspector]
        public bool isLerping;
        private float _time, _target;
        private float _current;

        private ValueType valueType;
        private AnimationCurve _curve;

        ///
        private Image _image;
        private SpriteRenderer _sprite;

        private void Awake() {
            isLerping = false;
        }

        public void Update() {
            if (isLerping) {
                _time = Mathf.MoveTowards(_time, 1, Time.deltaTime * 0.1f);

                switch (valueType) {
                    case ValueType.ImageFillAmount:
                        _image.fillAmount = Mathf.Lerp(_image.fillAmount, _target, _curve.Evaluate(_time));
                        break;

                    case ValueType.SpriteRenderer:
                        _sprite.transform.localScale = 
                            new Vector3(Mathf.Lerp(_sprite.transform.localScale.x, _target, _curve.Evaluate(_time)), 1, 1);
                        break;

                }

                if (Mathf.Abs(_current - _target) < 0.0001f) {
                    isLerping = false;
                }
            }
        }


        public void SetTarget(float target) {
            _time = 0;
            _target = target;
            isLerping = true;
        }
        public void SetTarget(float target, CurveType curveType) {

            switch (curveType) {
                case CurveType.EaseOut_Fast:
                    _curve = curves.Curve_EaseOut_Fast;
                    break;
                case CurveType.EaseOut_Slow:
                    _curve = curves.Curve_EaseOut_Slow;
                    break;
            }

            _time = 0;
            _target = target;
            isLerping = true;
        }

        public void SetAdjustVaule(Image image) {
            _image = image;
            valueType = ValueType.ImageFillAmount;

            _SetAdjustVaule();
        }
        public void SetAdjustVaule(SpriteRenderer sprite) {  // adjust localscale.x
            _sprite = sprite;
            valueType = ValueType.SpriteRenderer;

            _SetAdjustVaule();
        }
        
        private void _SetAdjustVaule() {
            if (curves == null) {
                curves = MyGameManager.Instance._curves;
                _curve = curves.Curve_EaseOut_Fast;
            }
            isLerping = false;
        }

        public enum ValueType {
            ImageFillAmount,
            SpriteRenderer
        }
        public enum CurveType {
            EaseOut_Fast,
            EaseOut_Slow
        }
    }
}
