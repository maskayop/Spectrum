using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spectrum
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class DiagramGenerator : MonoBehaviour
    {
        public enum DiagramType { spectral, sinX, sinY }
        public DiagramType type;

        public int width = 10;
        public int height = 10;
        public float speed = 1;

        [Header("Spectral")]
        [Range(0,2)]
        public int datasetIndex = 0;
        [Range(0.5f,2.0f)]
        public float amplitudeRatio = 1;

        [Header("Sin")]
        public float sinAmplitude = 1;
        public float frequency = 1;

        public enum ColorsType { horizontal, vertical }
        [Header("Colors")]
        public ColorsType colorsType;
        public Color cutColor = Color.white;
        public List<int> cutIndexes = new List<int>();
        public List<Color> colors = new List<Color>();

        [Header("Data")]
        public Dataset dataset;

        Vector3[] vertices;
        Vector2[] uv;

        Mesh mesh;

        float timeValue = 0;

        void Start()
        {
            for (int i = 0; i < dataset.dataAssets.Count; i++)
            {
                dataset.dataAssets[i].data.spectralAmplitudeDivider = dataset.spectralAmplitudeDivider;
                dataset.dataAssets[i].data.Init();
            }
            
            CreatePlane();
        }

        void Update()
        {
            timeValue += Time.deltaTime;
            UpdateGeneratedMesh();
        }

        void CreatePlane()
        {
            if (width == 0 || height == 0)
                return;

            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;

            if (type == DiagramType.spectral)
            {
                if (dataset.dataAssets.Count == 0)
                    return;

                //width = dataset.dataAssets[0].data.spectralData.Count / 2;
                height = width;
            }

            vertices = new Vector3[(width + 1) * (height + 1)];
            uv = new Vector2[vertices.Length];
            int[] triangles = new int[width * height * 6];

            // Создаём вершины
            for (int z = 0, i = 0; z <= height; z++)
            {
                for (int x = 0; x <= width; x++, i++)
                {
                    vertices[i] = new Vector3(x, 0, z);
                    uv[i] = new Vector2((float)x / width, (float)z / height);
                }
            }

            // Создаём треугольники
            for (int ti = 0, vi = 0, y = 0; y < height; y++, vi++)
            {
                for (int x = 0; x < width; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 1] = vi + width + 1;
                    triangles[ti + 2] = vi + 1;
                    triangles[ti + 3] = vi + 1;
                    triangles[ti + 4] = vi + width + 1;
                    triangles[ti + 5] = vi + width + 2;
                }
            }

            mesh.vertices = vertices;
            CalculateVertexColors();
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }

        void UpdateGeneratedMesh()
        {
            if (dataset.dataAssets.Count == 0)
                return;
            else if (datasetIndex <= 0)
                datasetIndex = 0;
            else if (datasetIndex >= dataset.dataAssets.Count)
                datasetIndex = dataset.dataAssets.Count - 1;

            if (type == DiagramType.spectral)
            {
                int amountInGroup = Mathf.CeilToInt((float)height / (dataset.dataAssets.Count - 1));
                float verticalOffset = 0;

                for (int z = 0, i = 0; z <= height; z++)
                {
                    for (int x = 0; x <= width; x++, i++)
                    {
                        if (amountInGroup != 0)
                        {
                            int d = 0;                            
                            d = Mathf.FloorToInt(z / amountInGroup);

                            int d1 = d + UICanvasMain.Instance.currentIndex;
                            int d2 = d + 1 + UICanvasMain.Instance.currentIndex;

                            if (d2 < dataset.dataAssets.Count)
                                verticalOffset = Mathf.Lerp(
                                    (float)dataset.dataAssets[d1].data.spectralData[x * 2],
                                    (float)dataset.dataAssets[d2].data.spectralData[x * 2],
                                    (float)z / amountInGroup % 1);
                            else
                                verticalOffset = Mathf.Lerp(
                                    (float)dataset.dataAssets[d1 - 47].data.spectralData[x * 2],
                                    (float)dataset.dataAssets[d2 - 47].data.spectralData[x * 2],
                                    (float)z / amountInGroup % 1);
                            //verticalOffset = (float)dataset.dataAssets[dataset.dataAssets.Count - 1].data.spectralData[x * 2];
                        }
                        else
                            verticalOffset = (float)dataset.dataAssets[datasetIndex].data.spectralData[x * 2];

                        vertices[i] = new Vector3(x, verticalOffset * amplitudeRatio, z);
                    }
                }
            }
            else if (type == DiagramType.sinX)
            {
                for (int z = 0, i = 0; z <= height; z++)
                {
                    for (int x = 0; x <= width; x++, i++)
                    {
                        vertices[i] = new Vector3(x, Mathf.Sin(x * frequency + timeValue * speed) * sinAmplitude, z);
                    }
                }
            }
            else if (type == DiagramType.sinY)
            {
                for (int z = 0, i = 0; z <= height; z++)
                {
                    for (int x = 0; x <= width; x++, i++)
                    {
                        vertices[i] = new Vector3(x, Mathf.Sin(z * frequency + timeValue * speed) * sinAmplitude, z);
                    }
                }
            }

            mesh.vertices = vertices;
            mesh.RecalculateNormals();
        }

        void CalculateVertexColors()
        {
            Color[] tempColors = new Color[mesh.vertices.Length];
            int amountInColorGroup = 0;

            if (colorsType == ColorsType.horizontal)
                amountInColorGroup = Mathf.CeilToInt((float)width / (colors.Count - 1));
            else if (colorsType == ColorsType.vertical)
                amountInColorGroup = Mathf.CeilToInt((float)height / (colors.Count - 1));
            
            for (int z = 0, i = 0; z <= height; z++)
            {
                for (int x = 0; x <= width; x++, i++)
                {
                    if (amountInColorGroup != 0)
                    {
                        int c = 0;

                        if (colorsType == ColorsType.horizontal)
                            c = Mathf.FloorToInt(x / amountInColorGroup);
                        else if (colorsType == ColorsType.vertical)
                            c = Mathf.FloorToInt(z / amountInColorGroup);

                        if (c + 1 < colors.Count)
                        {
                            if (colorsType == ColorsType.horizontal)
                                tempColors[i] = Color.Lerp(colors[c], colors[c + 1], (float)x / amountInColorGroup % 1);
                            else if (colorsType == ColorsType.vertical)
                                tempColors[i] = Color.Lerp(colors[c], colors[c + 1], (float)z / amountInColorGroup % 1);

                            for (int cut = 0; cut < cutIndexes.Count; cut++)
                            {
                                if (cutIndexes[cut] == x)
                                    tempColors[i] = cutColor;
                            }
                        }
                        else
                            tempColors[i] = colors[colors.Count - 1];
                    }
                    else
                        tempColors[i] = Color.white;
                }
            }

            mesh.colors = tempColors;
        }
    }
}
