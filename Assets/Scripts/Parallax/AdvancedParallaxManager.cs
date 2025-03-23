using UnityEngine;
using System;

namespace ASimpleRoguelike.Parallax {
    public class AdvancedParallaxManager : MonoBehaviour {
        [Serializable]
        public class ParallaxLayer {
            public Transform layer;           // The transform of the layer
            public Vector2 parallaxEffect;    // Parallax speed multiplier
            public Vector2 layerSize;         // Size of the layer
        }

        public Camera cam;                    // The main camera
        public ParallaxLayer[] layers;        // Parallax layers

        private Vector2 camStartPos;          // Initial position of the camera
        private Vector2[] layerStartPositions;// Initial positions of the layers

        void Start() {
            if (cam == null)
                cam = Camera.main;

            // Cache the camera's initial position
            camStartPos = cam.transform.position;

            // Cache the initial positions of the layers
            layerStartPositions = new Vector2[layers.Length];
            for (int i = 0; i < layers.Length; i++) {
                layerStartPositions[i] = layers[i].layer.position;
            }
        }

        void LateUpdate() {
            Vector2 camDelta = (Vector2)cam.transform.position - camStartPos;

            for (int i = 0; i < layers.Length; i++) {
                ParallaxLayer layer = layers[i];
                Vector2 offset = camDelta * layer.parallaxEffect;

                // Seamlessly wrap the layer's position
                Vector2 layerPosition = layerStartPositions[i] + offset;

                // Calculate the wrapped position
                layerPosition.x = WrapPosition(layerPosition.x, cam.transform.position.x, layer.layerSize.x);
                layerPosition.y = WrapPosition(layerPosition.y, cam.transform.position.y, layer.layerSize.y);

                // Update the layer's position
                layer.layer.position = new Vector3(layerPosition.x, layerPosition.y, layer.layer.position.z);
            }
        }

        // Wraps a position around the camera, ensuring seamless tiling
        private float WrapPosition(float layerPos, float camPos, float layerSize) {
            float distance = layerPos - camPos;

            // Determine how many full layer sizes the distance exceeds, and adjust accordingly
            int wraps = Mathf.FloorToInt((distance + layerSize / 2) / layerSize);

            layerPos -= wraps * layerSize;
            return layerPos;
        }

        void OnDrawGizmos() {
            if (layers == null || cam == null) return;

            // Draw the wrapping lines and layer bounds
            foreach (var layer in layers) {
                if (layer.layer == null) continue;

                float width = layer.layerSize.x;
                float height = layer.layerSize.y;
                Vector3 layerPos = layer.layer.position;

                // Draw the main box for the layer
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(layerPos, new Vector3(width, height, 0));

                // Draw wrapping boundaries
                Vector3 camPos = cam.transform.position;
                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vector3(layerPos.x - width / 2, camPos.y - 10, 0), new Vector3(layerPos.x - width / 2, camPos.y + 10, 0)); // Left
                Gizmos.DrawLine(new Vector3(layerPos.x + width / 2, camPos.y - 10, 0), new Vector3(layerPos.x + width / 2, camPos.y + 10, 0)); // Right
                Gizmos.DrawLine(new Vector3(camPos.x - 10, layerPos.y - height / 2, 0), new Vector3(camPos.x + 10, layerPos.y - height / 2, 0)); // Bottom
                Gizmos.DrawLine(new Vector3(camPos.x - 10, layerPos.y + height / 2, 0), new Vector3(camPos.x + 10, layerPos.y + height / 2, 0)); // Top

                // Draw crosshair
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(new Vector3(layerPos.x - width / 2, camPos.y, 0), new Vector3(layerPos.x + width / 2, camPos.y, 0)); // Horizontal
                Gizmos.DrawLine(new Vector3(camPos.x, layerPos.y - height / 2, 0), new Vector3(camPos.x, layerPos.y + height / 2, 0)); // Vertical
            }
        }
    }
}