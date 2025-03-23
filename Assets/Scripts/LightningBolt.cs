using UnityEngine;

namespace ASimpleRoguelike {
    [RequireComponent(typeof(LineRenderer))]
    public class LightningBolt : MonoBehaviour {
        public Vector2 startPoint;
        public Vector2 endPoint;
        public int segmentCount = 20;
        public float jaggedness = 0.2f;
        public float fadeDuration = 0.5f;
        
        public LineRenderer lineRenderer;
        private float fadeTime;

        void Update() {
            // Fade the lightning bolt over time
            if (fadeDuration > 0) {
                fadeTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, fadeTime / fadeDuration);
                SetLineRendererAlpha(alpha);

                if (fadeTime >= fadeDuration) {
                    Destroy(gameObject); // Destroy the lightning after it fades out
                }
            }
        }

        public void GenerateLightning() {
            lineRenderer.positionCount = segmentCount + 1;
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;

            Vector2 direction = endPoint - startPoint;
            Vector2 perpendicular = Vector2.Perpendicular(direction).normalized;

            for (int i = 0; i <= segmentCount; i++) {
                float t = (float)i / segmentCount;
                Vector2 point = Vector2.Lerp(startPoint, endPoint, t);

                // Add randomness to make it jagged
                float offset = (Random.value - 0.5f) * jaggedness * direction.magnitude;
                point += perpendicular * offset;

                lineRenderer.SetPosition(i, point);
            }

            lineRenderer.SetPosition(segmentCount, endPoint);
            lineRenderer.SetPosition(0, startPoint);
        }

        private void SetLineRendererAlpha(float alpha) {
            Gradient gradient = new();
            gradient.SetKeys(
                new GradientColorKey[] { new(Color.white, 0.0f), new(Color.white, 1.0f) },
                new GradientAlphaKey[] { new(alpha, 0.0f), new(alpha, 1.0f) }
            );
            lineRenderer.colorGradient = gradient;
        }
    }
}