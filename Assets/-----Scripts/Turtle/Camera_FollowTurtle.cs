using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LittleTurtle.Turtle{

    public class Camera_FollowTurtle : MonoBehaviour{

        Camera camera_T;

        void Start(){
            camera_T = GetComponent<Camera>();
        }

        void Update(){
            camera_T.transform.rotation = Quaternion.identity;
        }
    }
}
