using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public float scale;
    public int width;
    public int height;
    public string seed;

    float offsetX;
    float offsetY;

    public Tilemap tilemap;
    public TileBase grassTile;
    public TileBase waterTile;

    void Start()
    {
        System.Random prng = new System.Random(Mathf.Abs(seed.GetHashCode()));
        offsetX = prng.Next(0, 10000);
        offsetY = prng.Next(0, 10000);

        GenerateMap();
    }

    void GenerateMap()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float sampleX = (i + offsetX) * scale;
                float sampleY = (j + offsetY) * scale;

                float value = Mathf.PerlinNoise(sampleX, sampleY);

                if (value < 0.5)
                {
                    tilemap.SetTile(new Vector3Int(i, j, 0), grassTile);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(i, j, 0), waterTile);
                }
            }
        }
    }

    public void RegenerateMap()
    {
        tilemap.ClearAllTiles();
        System.Random prng = new System.Random(Mathf.Abs(seed.GetHashCode()));
        offsetX = prng.Next(0, 10000);
        offsetY = prng.Next(0, 10000);
        GenerateMap();
    }
}
