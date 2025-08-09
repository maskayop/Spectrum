using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Spectrum
{
    [Serializable]
    public class Data
    {
        public string name;

        [TextArea(3, 10)]
        public string rawData;

        [Header("Processed data")]
        public List<double> spectralData = new List<double>();

        public void Init()
        {
            ParseNumbers();
        }

        void ParseNumbers()
        {
            // Разбиваем строку на массив строк по переносу строки
            string[] lines = rawData.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // Очищаем список перед добавлением
            spectralData.Clear();

            // Парсим каждую строку в float и добавляем в список
            foreach (string line in lines)
            {
                // Убираем пробелы и проверяем, можно ли преобразовать в float
                if (double.TryParse(line.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
                {
                    spectralData.Add(number);
                }
                else
                {
                    Debug.LogWarning($"Не удалось преобразовать строку '{line}' в число.");
                }
            }
        }
    }

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class DiagramGenerator : MonoBehaviour
    {
        public int width = 10;
        public int height = 10;

        [Header("Sin")]
        public float amplitude = 1;
        public float frequency = 1;
        public float speed = 1;

        [Header("Colors")]
        public List<Color> colors = new List<Color>();

        [Header("Data")]
        public List<Data> dataset = new List<Data>();

        Vector3[] vertices;
        Vector2[] uv;

        Mesh mesh;

        float timeValue = 0;

        void Start()
        {
            CreatePlane();

            for (int i = 0; i < dataset.Count; i++)
                dataset[i].Init();
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

            vertices = new Vector3[(width + 1) * (height + 1)];
            uv = new Vector2[vertices.Length];
            int[] triangles = new int[width * height * 6];

            // Создаем вершины
            for (int z = 0, i = 0; z <= height; z++)
            {
                for (int x = 0; x <= width; x++, i++)
                {
                    vertices[i] = new Vector3(x, 0, z);
                    uv[i] = new Vector2((float)x / width, (float)z / height);
                }
            }

            // Создаем треугольники
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
            for (int z = 0, i = 0; z <= height; z++)
            {
                for (int x = 0; x <= width; x++, i++)
                {
                    vertices[i] = new Vector3(x, Mathf.Sin(x * frequency + timeValue * speed) * amplitude, z);
                }
            }

            mesh.vertices = vertices;
            mesh.RecalculateNormals();
        }

        void CalculateVertexColors()
        {
            Color[] tempColors = new Color[mesh.vertices.Length];
            int amountInColorCroup = Mathf.CeilToInt((float)width / colors.Count);

            for (int z = 0, i = 0; z <= height; z++)
            {
                for (int x = 0; x <= width; x++, i++)
                {
                    if (amountInColorCroup != 0)
                    {
                        int c = Mathf.FloorToInt(x / amountInColorCroup);

                        if (c + 1 < colors.Count)
                            tempColors[i] = Color.Lerp(colors[c], colors[c + 1], ((float)x / amountInColorCroup) % 1);
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
