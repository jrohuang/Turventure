using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LittleTurtle {
    public class TemporaryStorage : MonoBehaviour {

        public static TemporaryStorage Instance { get; private set; }
        public float zAxis;

        private void Awake() {
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;

            zAxis = 105;
        }

        public void PlaceObject(GameObject obj) {
            obj.transform.SetParent(transform);

            zAxis -= 0.001f;
            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x,
                                                        obj.transform.localPosition.y, zAxis);
        }
    }
}