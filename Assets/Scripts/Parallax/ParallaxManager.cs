using UnityEngine;

namespace ASimpleRoguelike.Parallax {
    public class ParallaxManager : MonoBehaviour
    {
        [System.Serializable]
        public class ParallaxLayer
        {
            public Sprite sprite;             // The sprite for this layer
            public Vector2 size = Vector2.one; // The size of the sprite
            public Vector2 parallaxMultiplier; // The parallax effect multiplier for movement
        }

        public ParallaxLayer[] layers;  // List of parallax layers
        public Transform target;       // The target the camera or player follows
        public Vector2 viewSize;       // The size of the camera's visible area in world units

        private class LayerInstance
        {
            public Transform[] tiles;  // The tile transforms
            public Vector2 size;       // The size of each tile
        }

        private LayerInstance[] layerInstances;

        private void Start()
        {
            InitializeLayers();
        }

        private void LateUpdate()
        {
            UpdateParallax();
        }

        private void InitializeLayers()
        {
            layerInstances = new LayerInstance[layers.Length];

            for (int i = 0; i < layers.Length; i++)
            {
                ParallaxLayer layer = layers[i];
                GameObject layerContainer = new($"Layer_{i}");
                layerContainer.transform.SetParent(transform);

                Vector2 size = layer.size;

                // Calculate how many tiles are needed to cover the visible area and add extra for wrapping
                Vector2Int tileCount = new(
                    Mathf.CeilToInt(viewSize.x / size.x) + 2,
                    Mathf.CeilToInt(viewSize.y / size.y) + 2);

                Transform[] tiles = new Transform[tileCount.x * tileCount.y];

                for (int x = 0; x < tileCount.x; x++)
                {
                    for (int y = 0; y < tileCount.y; y++)
                    {
                        GameObject tile = new($"Tile_{x}_{y}");
                        tile.transform.SetParent(layerContainer.transform);

                        SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
                        sr.sprite = layer.sprite;

                        tile.transform.localPosition = new Vector3(
                            (x - tileCount.x / 2) * size.x,
                            (y - tileCount.y / 2) * size.y,
                            0);

                        tiles[x + y * tileCount.x] = tile.transform;
                    }
                }

                layerInstances[i] = new LayerInstance { tiles = tiles, size = size };
            }
        }

        private void UpdateParallax()
        {
            for (int i = 0; i < layers.Length; i++)
            {
                ParallaxLayer layer = layers[i];
                LayerInstance instance = layerInstances[i];
                Vector2 targetPosition = target.position * layer.parallaxMultiplier;

                foreach (var tile in instance.tiles)
                {
                    Vector3 tilePosition = tile.localPosition;

                    // Calculate the distance between the tile and the target
                    Vector2 distance = (Vector2)tilePosition - targetPosition;

                    // Wrap horizontally
                    if (distance.x > instance.size.x * (viewSize.x / 2 + 1))
                        tilePosition.x -= instance.size.x * Mathf.CeilToInt(viewSize.x / instance.size.x);
                    else if (distance.x < -instance.size.x * (viewSize.x / 2 + 1))
                        tilePosition.x += instance.size.x * Mathf.CeilToInt(viewSize.x / instance.size.x);

                    // Wrap vertically
                    if (distance.y > instance.size.y * (viewSize.y / 2 + 1))
                        tilePosition.y -= instance.size.y * Mathf.CeilToInt(viewSize.y / instance.size.y);
                    else if (distance.y < -instance.size.y * (viewSize.y / 2 + 1))
                        tilePosition.y += instance.size.y * Mathf.CeilToInt(viewSize.y / instance.size.y);

                    tile.localPosition = tilePosition;

                    // Apply parallax offset
                    tile.localPosition = new Vector3(
                        tile.localPosition.x + targetPosition.x % instance.size.x,
                        tile.localPosition.y + targetPosition.y % instance.size.y,
                        tile.localPosition.z
                    );
                }
            }
        }
    }
}