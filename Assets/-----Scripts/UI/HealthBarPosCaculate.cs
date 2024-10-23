using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LittleTurtle.UI {

    public class HealthBarPosCaculate {

        private float spriteWidth, spriteHeight;
        private float minOffsetWhenVertical_Sin, minOffsetWhenVertical;
        private float maxOffsetWhenHorizontal;
        private Vector2 offset;
        private Transform transform;
        private GameObject HPBarObject;

        public HealthBarPosCaculate(float spriteWidth, float spriteHeight, Transform transform, GameObject HPBarObject, Vector2 offset) {
            this.offset = offset;
            this.HPBarObject = HPBarObject;
            this.transform = transform;
            this.spriteWidth = spriteWidth;
            this.spriteHeight = spriteHeight;
            Initialize();
        }

        private void Initialize() {
            minOffsetWhenVertical_Sin = Mathf.Asin(spriteHeight / 2 / Mathf.Sqrt(spriteWidth * spriteWidth + spriteHeight * spriteHeight));
            minOffsetWhenVertical = (spriteWidth / 2) * Mathf.Abs(Mathf.Cos(minOffsetWhenVertical_Sin)) + (spriteHeight / 2) * Mathf.Abs(Mathf.Sin(minOffsetWhenVertical_Sin));
            maxOffsetWhenHorizontal = (spriteWidth / 2) * Mathf.Abs(Mathf.Cos(minOffsetWhenVertical_Sin / 2)) + (spriteHeight / 2) * Mathf.Abs(Mathf.Sin(minOffsetWhenVertical_Sin / 2));
        }

        public void UpdateHealthBarPosition() {
            float z = (transform.rotation.eulerAngles.z + 90f) * Mathf.Deg2Rad;
            float sinZ = Mathf.Sin(z);
            float cosZ = Mathf.Cos(z);
            float sinAbs = Mathf.Abs(sinZ);

            float offsetY = 0;

            if (sinAbs > minOffsetWhenVertical_Sin) {
                offsetY = minOffsetWhenVertical;
            }
            else if (sinAbs < minOffsetWhenVertical_Sin / 2) {
                offsetY = maxOffsetWhenHorizontal;
            }
            else {
                float cosAbs = Mathf.Abs(cosZ);
                offsetY = spriteWidth / 2 * cosAbs + spriteHeight / 2 * sinAbs;
            }

            HPBarObject.transform.position = new Vector3(transform.position.x + offset.x,
                                                             transform.position.y + offsetY + offset.y, 0);
            HPBarObject.transform.rotation = Quaternion.identity;
        }

    }
}
