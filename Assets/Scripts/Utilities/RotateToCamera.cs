using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minesweeper3D
{
    public class RotateToCamera : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            Transform cam = Camera.main.transform;

            Vector3 direction = transform.position - cam.position;

            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}