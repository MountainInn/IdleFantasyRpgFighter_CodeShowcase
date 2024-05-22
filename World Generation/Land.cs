using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Zenject;
using UniRx;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Land : MonoBehaviour
{
    [SerializeField] int resolution;
    [Space]
    [SerializeField] int voronoiResolution;
    [SerializeField] float voronoiSize;
    [SerializeField][Range(0, 10000)] float heightmapStrength;
    [Space]
    [SerializeField][Range(0, 20)] float big_yNoiseMult;
    [SerializeField][Range(0, 4)] float big_perlinScale;
    [SerializeField][Range(0, 20)] float yNoiseMult;
    [SerializeField][Range(0, 4)] float perlinScale;
    [SerializeField] Mesh mesh;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshCollider meshCollider;

    [Inject] [SerializeField] GameSettings gameSettings;

    float fullSize => gameSettings.landSize.Value;
    public BoolReactiveProperty generated = new(false);

    void OnValidate()
    {
        meshFilter ??= GetComponent<MeshFilter>();
        meshRenderer ??= GetComponent<MeshRenderer>();
        meshCollider ??= GetComponent<MeshCollider>();

#if UNITY_EDITOR
        if (gameSettings == null)
            gameSettings = UnityEditor.AssetDatabase.LoadAssetAtPath<GameSettings>("Assets/Resources/GameSettings.asset");
       
        RebuildOnStart(transform.position);
#endif
    }

    void Start()
    {

    }

    public void RebuildOnStart(Vector3 spawnPosition)
    {
        mesh = null;
        Rebuild(spawnPosition);
    }

    void Rebuild(Vector3 spawnPosition)
    {
        generated.Value = false;

        MeshData meshData = GenerateTerrainMesh(resolution, spawnPosition);

        Mesh generatedMesh = meshData.CreateMesh();
        generatedMesh.name = "Generated Mesh";

        meshFilter.mesh =
            mesh = generatedMesh;

        meshCollider.sharedMesh = mesh;
       
        generated.Value = true;
    }

    public MeshData GenerateTerrainMesh(int resolution, Vector3 spawnPosition)
    {
        float step = fullSize / resolution;

        int borderedSize = resolution+2;
        int meshSize = resolution;

        int verticesPerLine = meshSize;

        MeshData meshData = new MeshData(verticesPerLine);

        int[,] vertexIndicesMap = new int[borderedSize,borderedSize];
        int meshVertexIndex = 0;
        int borderVertexIndex = -1;

        var points =
            GenerateRandomVoronoiPoints(voronoiResolution, voronoiSize)
            .Select(p => p + spawnPosition)
            .ToArray();

        var heightMap = GenerateVoronoiHeightmap(borderedSize, points, heightmapStrength);


        for (int z = 0; z < borderedSize; z++)
        {
            for (int x = 0; x < borderedSize; x++)
            {
                bool isBorderVertex = z == 0 || z == borderedSize - 1 || x == 0 || x == borderedSize - 1;

                if (isBorderVertex)
                {
                    vertexIndicesMap[x, z] = borderVertexIndex;
                    borderVertexIndex--;
                } else
                {
                    vertexIndicesMap[x, z] = meshVertexIndex;
                    meshVertexIndex++;
                }
                }
            }

            for (int z = 0; z < borderedSize; z++)
            {
                for (int x = 0; x < borderedSize; x++)
                {
                    int vertexIndex = vertexIndicesMap[x, z];


                    Vector3 vertexPosition = new Vector3(x, 0, z) * (step+1);

                    Vector3 normalizedVertexPosition = vertexPosition / fullSize;

                    Vector3 normalizedSpawnPosition = spawnPosition / fullSize;

                    float big_perlinY = PerlinY(normalizedVertexPosition + normalizedSpawnPosition,
                                                big_perlinScale,
                                                big_yNoiseMult);

                    float perlinY = PerlinY(normalizedVertexPosition + normalizedSpawnPosition,
                                            perlinScale,
                                            yNoiseMult);


                    vertexPosition = vertexPosition.WithY(perlinY + big_perlinY - heightMap[x, z]);

                    Vector2 uv = normalizedVertexPosition.xz();

                    meshData.AddVertex(vertexPosition, uv, vertexIndex);

                    if (x < borderedSize - 1 && z < borderedSize - 1)
                    {
                        int a = vertexIndicesMap[x, z];
                        int b = vertexIndicesMap[x + 1, z];
                        int c = vertexIndicesMap[x, z + 1];
                        int d = vertexIndicesMap[x + 1, z + 1];

                        meshData.AddTriangle(c,d,a);
                        meshData.AddTriangle(b,a,d);
                    }

                    vertexIndex++;
                }
            }

            return meshData;
        }

        public float[,] GenerateVoronoiHeightmap(int resolution, Vector3[] voronoiPoints, float heightScale)
        {
            float[,] heightMap = new float[resolution + 1, resolution + 1];
            float maxDistance = Mathf.Sqrt(2 * (resolution * resolution));

            for (int z = 0; z < resolution + 1; z++)
            {
                for (int x = 0; x < resolution + 1; x++)
                {
                    Vector3 currentPoint = new Vector3(x, 0, z) + transform.position;

                    float minDistance =
                        voronoiPoints
                        .Min(p => Vector3.Distance(currentPoint, p));

                    heightMap[x, z] = minDistance / maxDistance * heightScale;
                }
            }

        return heightMap;
    }

    public Vector3[] GenerateRandomVoronoiPoints(int numberOfPoints, float scale)
    {
        Random.InitState(2);

        Vector3[] randomPoints = new Vector3[numberOfPoints];

        for (int i = 0; i < numberOfPoints; i++)
        {
            float x = Random.Range(0f, scale);
            float z = Random.Range(0f, scale);
            randomPoints[i] = new Vector3(x, 0, z);
        }

        return randomPoints;
    }


    public float PerlinY(Vector3 v, float perlinScale, float yNoiseMult)
    {
        return Perlin(v.x, v.z, perlinScale) * yNoiseMult;
    }

    float Perlin(float x, float y, float perlinScale)
    {
        return Mathf.PerlinNoise(x * perlinScale, y * perlinScale);
    }

    public class Pool : PoolOnTreadmill<Land>
    {
    }
}
