using UnityEngine;
using UnityEngine.Tilemaps;
using Cinemachine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using TMPro;

public class MapGenerator : MonoBehaviour
{
    [Header("Camera Bounds")]
    public CinemachineConfiner2D confiner;
    public PolygonCollider2D cameraBounds;

    [Header("Generate Map Bounds")]
    public Slider scaleSlider;
    public Slider sizeSlider;
    public TMP_InputField desiredSeed;

    [Range(0.01f, 1f)]
    public float scale;

    public string seed;
    public int width;
    public int height;
    public int halfW;
    public int halfH;

    float randomX;
    float randomY;

    [Header("Tilemaps")]
    public Tilemap placeholderTilemap;
    public Tilemap displayTilemap;
    public Tilemap colliderTilemap;

    [Header("Tiles")]
    public TileBase grassTile;
    public TileBase waterTile;
    public TileBase borderTile;

    [Header("Assign")]
    public Tile[] tiles;
    public GameObject parentEnv;

    System.Random prng;

    public enum TileType
    {
        None,
        Grass,
        Water
    }

    [System.Serializable]
    public class EnvType
    {
        public string name;
        public int total;
        public GameObject[] prefabs;
    }

    public EnvType[] environment;

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
        scaleSlider.value = 0.066f;
        scaleSlider.minValue = 0.001f;
        scaleSlider.maxValue = 1f;

        sizeSlider.value = 100f;
        sizeSlider.minValue = 100f;
        sizeSlider.maxValue = 1000f;

        desiredSeed.text = "lstmap";

        prng = new System.Random(Mathf.Abs(seed.GetHashCode()));
        randomX = prng.Next(0, 10000);
        randomY = prng.Next(0, 10000);

        halfW = width / 2;
        halfH = height / 2;

        CreateTuple();
        RegenerateMap();
    }

    void OnEnable()
    {
        sizeSlider.onValueChanged.AddListener(ChangeSize);
        ChangeSize(sizeSlider.value);

        scaleSlider.onValueChanged.AddListener(ChangeScale);
        ChangeScale(scaleSlider.value);

        desiredSeed.onEndEdit.AddListener(ChangeSeed);
        ChangeSeed(desiredSeed.text);
    }

    void OnDisable()
    {
        sizeSlider.onValueChanged.RemoveAllListeners();
        scaleSlider.onValueChanged.RemoveAllListeners();
    }

    void ChangeScale(float value)
    {
        scale = value;
    }

    void ChangeSize(float value)
    {
        width = (int)value;
        height = (int)value;
    }

    void ChangeSeed(string desiredSeed)
    {
        seed = desiredSeed;
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
                if (value < 0.4)
                {
                    placeholderTilemap.SetTile(coords, waterTile);
                    SetDisplayTile(coords);
                }
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
        AddNavModToChildScript.AddBuildNavMesh();
    }

    public void GenerateEnvironment()
    {
        int currentEnv = 0;

        for (int i = 0; i < environment.Length; i++)
        {
            EnvType env = environment[currentEnv];
            for (int j = 0; j < env.total; j++)
            {
                GameObject prefabToSpawn = null;
                if (env.prefabs.Length > 1)
                {
                    prefabToSpawn = env.prefabs[prng.Next(0, env.prefabs.Length)];
                }
                else
                {
                    prefabToSpawn = env.prefabs[0];
                }

                if (prefabToSpawn != null)
                {
                    Instantiate(prefabToSpawn, RandomSpawnPos(), Quaternion.identity, parentEnv.transform);
                }
            }
            if (currentEnv < environment.Length)
            {
                currentEnv++;
            }
            else if (currentEnv == environment.Length)
            {
                currentEnv = 0;
            }
        }
        AddNavModToChildScript.AddModifiersToEnvironment();
    }

    Vector3 RandomSpawnPos()
    {
        int max = 100;
        for (int i = 0; i < max; i++)
        {
            float randomPosX = prng.Next(-(halfW - 1), halfW - 1);
            float randomPosY = prng.Next(-(halfW - 1), halfW - 1);
            Vector3 randomPos = new Vector3(randomPosX + 0.5f, randomPosY + 0.5f, 0);

            if (validateSurrounding(randomPos))
            {
                return randomPos;
            }
            else
            {
                Debug.Log("Not Valid");
            }
        }
        return Vector3.zero;
    }

    bool validateSurrounding(Vector3 pos)
    {
        Vector3Int cell = placeholderTilemap.WorldToCell(pos);
        Vector3Int[] directions = new Vector3Int[]
        {
            cell,
            cell + new Vector3Int(1, 0, 0),
            cell + new Vector3Int(0, 1, 0),
            cell + new Vector3Int(1, 1, 0)
        };

        foreach (var dir in directions)
        {
            TileBase tile = placeholderTilemap.GetTile(dir);
            if (tile == borderTile || tile == waterTile)
            {
                return false;
            }
        }
        return true;
    }

    public void CreateTuple()
    {
        neighbourTupleToTile = new()
        {
            {new (TileType.Grass, TileType.Grass, TileType.Grass, TileType.Grass), tiles[12]}, // Full Grass

            {new (TileType.Water, TileType.Water, TileType.Water, TileType.Grass), tiles[7]}, // Outer Bottom Right
            {new (TileType.Water, TileType.Water, TileType.Grass, TileType.Water), tiles[10]}, // Outer Bottom Left
            {new (TileType.Water, TileType.Grass, TileType.Water, TileType.Water), tiles[2]}, // Outer Top RIght
            {new (TileType.Grass, TileType.Water, TileType.Water, TileType.Water), tiles[5]}, // Outer Top Left

            {new (TileType.Water, TileType.Grass, TileType.Water, TileType.Grass), tiles[11]}, // Edge Right
            {new (TileType.Grass, TileType.Water, TileType.Grass, TileType.Water), tiles[1]}, // Edge Left
            {new (TileType.Water, TileType.Water, TileType.Grass, TileType.Grass), tiles[9]}, // Edge Bottom
            {new (TileType.Grass, TileType.Grass, TileType.Water, TileType.Water), tiles[3]}, // Edge Top

            {new (TileType.Water, TileType.Grass, TileType.Grass, TileType.Grass), tiles[15]}, // Inner Bottom Right
            {new (TileType.Grass, TileType.Water, TileType.Grass, TileType.Grass), tiles[8]}, // Inner Bottom left
            {new (TileType.Grass, TileType.Grass, TileType.Water, TileType.Grass), tiles[0]}, // Inner Top Right
            {new (TileType.Grass, TileType.Grass, TileType.Grass, TileType.Water), tiles[13]}, // Inner Top Left
 
            {new (TileType.Water, TileType.Grass, TileType.Grass, TileType.Water), tiles[4]}, // Dual Up Right
            {new (TileType.Grass, TileType.Water, TileType.Water, TileType.Grass), tiles[14]}, // Dual Down Right

            {new (TileType.Water, TileType.Water, TileType.Water, TileType.Water), tiles[6]} // Full Water
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
            return TileType.Water;
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
        prng = new System.Random(Mathf.Abs(seed.GetHashCode()));
        randomX = prng.Next(0, 10000);
        randomY = prng.Next(0, 10000);

        halfW = width / 2;
        halfH = height / 2;

        GenerateMap();
        ClearEnvironment();
        GenerateEnvironment();
        UpdateCamera();
    }

    public void ClearEnvironment()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in parentEnv.transform)
        {
            children.Add(child.gameObject);
        }

        foreach (GameObject child in children)
        {
            Destroy(child);
        }
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
