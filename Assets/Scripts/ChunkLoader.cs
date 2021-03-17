using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public const float maxViewDst = 200;
    public Transform player;

    public static Vector2 playerPosition;
    int chunkSize;
    int chunksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

    void Start() {
        chunkSize = 100;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
    }

    void Update() {
        playerPosition = new Vector2(player.position.x, player.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks() {
    
        for ( int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++) {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(playerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(playerPosition.y / chunkSize);
    
        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
                for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
                    Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (terrainChunkDictionary.ContainsKey (viewedChunkCoord)) {
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                        if (terrainChunkDictionary[viewedChunkCoord].isVisible()) {
                            terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
                        }
                    } else {
                        terrainChunkDictionary.Add (viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform));
                    }
                }
        }
    }

    public class TerrainChunk {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        public TerrainChunk(Vector2 coord, int size, Transform parent) {
            position = coord * size;
            bounds = new Bounds(position,Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = positionV3;
            meshObject.transform.localScale = Vector3.one * size / 10f;
            meshObject.transform.parent = parent;
            SetVisible(false);
        }

        public void UpdateTerrainChunk() {
            float playerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(playerPosition));
            bool visible = playerDstFromNearestEdge <= maxViewDst;
            SetVisible(visible);
        }

        public void SetVisible(bool visible) {
            meshObject.SetActive(visible);
        }

        public bool isVisible() {
            return meshObject.activeSelf;
        }
    }
}
