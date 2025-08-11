using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;

public class MapGenerator : MonoBehaviour
{
    public CinemachineConfiner2D confiner;
    public PolygonCollider2D cameraBounds;

    [Range(0.01f, 1f)]
    public float scale;

    public string seed;
    public int width;
    public int height;
    public int halfW;
    public int halfH;

    float randomX;
    float randomY;

    public Tilemap placeholderTilemap;
    public Tilemap displayTilemap;
    public Tilemap colliderTilemap;

    public TileBase grassTile;
    public TileBase waterTile;
    public TileBase sandTile;
    public TileBase borderTile;

    public Tile[] tiles;
    public enum TileType
    {
        None,
        Grass,
        Dirt
    }

    protected static Vector3Int[] neighbours = new Vector3Int[]
    {
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0)
    };

    protected static Dictionary<Tuple<TileType, TileType, TileType, TileType>, Tile> neighbourTupleToTile;

    void Start()
    {
        System.Random prng = new System.Random(Mathf.Abs(seed.GetHashCode()));
        randomX = prng.Next(0, 10000);
        randomY = prng.Next(0, 10000);

        halfW = width / 2;
        halfH = height / 2;

        CreateTuple();
    }

    void GenerateMap()
    {
        for (int i = -halfW; i <= halfW; i++)
        {
            for (int j = -halfH; j <= halfH; j++)
            {
                float sampleX = (i + randomX) * scale;
                float sampleY = (j + randomY) * scale;

                float value = Mathf.PerlinNoise(sampleX, sampleY);

                Vector3Int coords = new Vector3Int(i, j, 0);
                if (value < 0.3)
                {
                    placeholderTilemap.SetTile(coords, waterTile);
                    SetDisplayTile(coords);
                }
                // else if (value < 0.45)
                // {
                //     placeholderTilemap.SetTile(coords, sandTile);
                //     SetDisplayTile(coords);
                // }
                else
                {
                    placeholderTilemap.SetTile(coords, grassTile);
                    SetDisplayTile(coords);
                }

                if (i == -halfW || i == halfW || j == -halfH || j == halfH)
                {
                    colliderTilemap.SetTile(coords, borderTile);
                }
            }
        }
        RefreshDisplayMap();
    }

    public void CreateTuple()
    {
        neighbourTupleToTile = new()
        {
            {new (TileType.Grass, TileType.Grass, TileType.Grass, TileType.Grass), tiles[6]}, // Full Grass

            {new (TileType.Dirt, TileType.Dirt, TileType.Dirt, TileType.Grass), tiles[13]}, // Outer Bottom Right
            {new (TileType.Dirt, TileType.Dirt, TileType.Grass, TileType.Dirt), tiles[0]}, // Outer Bottom Left
            {new (TileType.Dirt, TileType.Grass, TileType.Dirt, TileType.Dirt), tiles[8]}, // Outer Top RIght
            {new (TileType.Grass, TileType.Dirt, TileType.Dirt, TileType.Dirt), tiles[15]}, // Outer Top Left

            {new (TileType.Dirt, TileType.Grass, TileType.Dirt, TileType.Grass), tiles[1]}, // Edge Right
            {new (TileType.Grass, TileType.Dirt, TileType.Grass, TileType.Dirt), tiles[11]}, // Edge Left
            {new (TileType.Dirt, TileType.Dirt, TileType.Grass, TileType.Grass), tiles[3]}, // Edge Bottom
            {new (TileType.Grass, TileType.Grass, TileType.Dirt, TileType.Dirt), tiles[9]}, // Edge Top

            {new (TileType.Dirt, TileType.Grass, TileType.Grass, TileType.Grass), tiles[5]}, // Inner Bottom Right
            {new (TileType.Grass, TileType.Dirt, TileType.Grass, TileType.Grass), tiles[2]}, // Inner Bottom left
            {new (TileType.Grass, TileType.Grass, TileType.Dirt, TileType.Grass), tiles[10]}, // Inner Top Right
            {new (TileType.Grass, TileType.Grass, TileType.Grass, TileType.Dirt), tiles[7]}, // Inner Top Left
 
            {new (TileType.Dirt, TileType.Grass, TileType.Grass, TileType.Dirt), tiles[14]}, // Dual Up Right
            {new (TileType.Grass, TileType.Dirt, TileType.Dirt, TileType.Grass), tiles[4]}, // Dual Down Right

            {new (TileType.Dirt, TileType.Dirt, TileType.Dirt, TileType.Dirt), tiles[12]} // Full Water
        };
    }

    private TileType getPlaceHolderTileTypeAt(Vector3Int coords)
    {
        if (placeholderTilemap.GetTile(coords) == grassTile)
        {
            return TileType.Grass;
        }
        else
        {
            return TileType.Dirt;
        }
    }

    private Tile calculatedDisplayTile(Vector3Int coords)
    {
        TileType topRight = getPlaceHolderTileTypeAt(coords - neighbours[0]);
        TileType topLeft = getPlaceHolderTileTypeAt(coords - neighbours[1]);
        TileType bottomRight = getPlaceHolderTileTypeAt(coords - neighbours[2]);
        TileType bottomLeft = getPlaceHolderTileTypeAt(coords - neighbours[3]);

        Tuple<TileType, TileType, TileType, TileType> neighbourTuple = new(topLeft, topRight, bottomLeft, bottomRight);
        return neighbourTupleToTile[neighbourTuple];
    }

    private void SetDisplayTile(Vector3Int pos)
    {
        for (int i = 0; i < neighbours.Length; i++)
        {
            Vector3Int newPos = pos + neighbours[i];
            displayTilemap.SetTile(newPos, calculatedDisplayTile(newPos));
        }
    }

    public void RegenerateMap()
    {
        placeholderTilemap.ClearAllTiles();
        System.Random prng = new System.Random(Mathf.Abs(seed.GetHashCode()));
        randomX = prng.Next(0, 10000);
        randomY = prng.Next(0, 10000);
        UpdateCamera();
        GenerateMap();
    }

    public void RefreshDisplayMap()
    {
        for (int i = -halfW; i <= halfW; i++)
        {
            for (int j = -halfH; j <= halfH; j++)
            {
                SetDisplayTile(new Vector3Int(i, j, 0));
            }
        }
    }

    void UpdateCamera()
    {
        // Camera Bounds
        Vector2 p1 = new Vector2(-halfW, -halfW);
        Vector2 p2 = new Vector2(halfW, -halfW);
        Vector2 p3 = new Vector2(halfW, halfW);
        Vector2 p4 = new Vector2(-halfW, halfW);

        cameraBounds.transform.position = Vector3.zero;
        cameraBounds.points = new[] { p1, p2, p3, p4 };

        confiner.InvalidateCache();
    }
    
}
