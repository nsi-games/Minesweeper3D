using UnityEngine;
using System.Collections;

namespace Minesweeper3D
{
    public class CameraOrbit : MonoBehaviour
    {
        public float zoomSpeed = 5f; // How fast can the camera zoom?
        // X and Y rotation speed
        public float xSpeed = 120.0f;
        public float ySpeed = 120.0f;
        // X and Y rotation limits
        public float yMin = -80f;
        public float yMax = 80f;
        // Distance limits
        public float distanceMin = 10f;
        public float distanceMax = 15f;
        // Current x & y rotation
        private float x = 0.0f;
        private float y = 0.0f;
        private float distance; // Current distance

        // Use this for initialization
        void Start()
        {
            // Set distance to maximum distance
            distance = distanceMax;
        }

        void LateUpdate()
        {
            // Is the Left mouse button pressed?
            if (Input.GetMouseButton(1))
            {
                // Get input X and Y offsets
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");
                // Offset rotation with mouse X and Y offsets
                x += mouseX * xSpeed * Time.deltaTime;
                y -= mouseY * ySpeed * Time.deltaTime;
                // Clamp the Y between min and max limits
                y = Mathf.Clamp(y, yMin, yMax);
                // Get scroll wheel offset
            }

            // Scroll the mouse wheel here
            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            // Update distance using scroll wheel
            distance -= scrollWheel * zoomSpeed;
            // Clamp distance between min and max limits
            distance = Mathf.Clamp(distance, distanceMin, distanceMax);

            // Update transform
            transform.rotation = Quaternion.Euler(y, x, 0);
            transform.position = -transform.forward * distance;
        }
    }
}