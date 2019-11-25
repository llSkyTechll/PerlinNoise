using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour {

    public enum GenerationType
    {
        RANDOM, PERLINNOISE
    }

    public GenerationType generationType;

    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public bool autoUpdate;
    public int seed;
    public Vector2 offset;
    public Tilemap tilemap;

    public TerrainType[] regions;
    public Mineral[] mineral;
    public Ground[] ground;

    public void GenerateMap()
    {
        if(generationType == GenerationType.PERLINNOISE)
        {
            GenerateMapWithNoise();
        }
        else if(generationType == GenerationType.RANDOM)
        {
            GenerateMapWithRandom();
        }
    }

    public void GenerateMapWithNoise()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] noiseMapDeterminationGisement = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed + 1, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] noiseMapDeterminationContenuGisement = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed + 2, noiseScale, octaves, persistance, lacunarity, offset);
        TileBase[] customeTileMap = new TileBase[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float randomDeterminationgisement = noiseMapDeterminationGisement[x, y];
                if (randomDeterminationgisement > 0.80)
                {
                    float rnd = noiseMapDeterminationContenuGisement[x, y];
                    customeTileMap[y * mapWidth + x] = FindTileFromMineral(rnd);
                }
                else
                {
                    float rnd = noiseMap[x, y];
                    customeTileMap[y * mapWidth + x] = FindTileFromRegion(rnd);
                }
                
            }
        }
        SetTileMap(customeTileMap);
    }

    private TileBase FindTileFromMineral(float rnd)
    {
        for (int i = 0; i < mineral.Length; i++)
        {
            if (rnd <= mineral[i].height)
            {
                return mineral[i].tile;
            }
        }
        return mineral[0].tile;
    }

    public void GenerateMapWithRandom()
    {
        TileBase[] customeTileMap = new TileBase[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float rnd = Random.Range(0f, 1f);
                customeTileMap[y * mapWidth + x] = FindTileFromRegion(rnd);
            }
        }
        SetTileMap(customeTileMap);
    }

    private void SetTileMap(TileBase[] customTileMap)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), customTileMap[y * mapWidth + x]);
            }
        }
    }

    private TileBase FindTileFromRegion(float rnd)
    {
        for (int i = 0; i < ground.Length; i++)
        {
            if (rnd <= ground[i].height)
            {
                return ground[i].tile;
            }
        }
        return ground[0].tile;
    }

    private TileBase GenerateMineral(float rnd)
    {
        return mineral[(int)rnd].tile;
    }

    private TileBase GenerateGround(float rnd)
    {
        if (rnd <= 0.3)
        {
            return ground[0].tile;
        }
        else if (rnd < 0.5)
        {
            return ground[1].tile;
        }
        return ground[0].tile;
    }

    private TileBase GeneratePureMineral(float rnd)
    {
        if (rnd > 0.7 && rnd <= 0.8)
        {
            return mineral[0].tile;
        }
        else if (rnd < 0.9 && rnd > 0.8)
        {
            return mineral[1].tile;
        }
        return mineral[2].tile;
    }

    private void OnValidate()
    {
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public TileBase tile;
}

[System.Serializable]
public struct Mineral
{
    public string name;
    public float height;
    public TileBase tile;
}

[System.Serializable]
public struct Ground
{
    public string name;
    public float height;
    public TileBase tile;
}