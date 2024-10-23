using UnityEngine;
using System.Collections;

namespace LittleTurtle.GameScene {

	public class CameraFollow : MonoBehaviour {

		public static CameraFollow Instance;

		public float lerpSpeed = 0.35f;
		public float X_Offset, Y_Offset;

		private Transform playerPos;

		private void Awake() {
			Instance = this;
		}
		void Start() {
			if (playerPos == null) {
				playerPos = PlayerCtrl.Instance.transform.Find("AnchorPos").transform;
			}
		}

		private Vector3 v3;
		private void LateUpdate() {
			if (playerPos) {
				transform.position = Vector3.SmoothDamp(transform.position, new Vector3(playerPos.transform.position.x + X_Offset, playerPos.transform.position.y + Y_Offset, -20), ref v3, lerpSpeed);
			}
		}

	}
}