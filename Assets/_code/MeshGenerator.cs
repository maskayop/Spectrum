using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
	public int width = 10;
	public int height = 10;

	public Texture2D sourceTexture;
	public int mipLevel = 0;

	float[] pixelValues;

	void Start()
	{
		ConvertTextureToArray();
		CreatePlane(width, height);
	}

	void CreatePlane(int width, int height)
	{
		Mesh mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;

		Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];
		Vector2[] uv = new Vector2[vertices.Length];
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
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}

	void ConvertTextureToArray()
	{
		if (sourceTexture == null)
			return;

		// Создаем временную текстуру для чтения конкретного MIP-уровня
		Texture2D mipTexture = new Texture2D(
			sourceTexture.width >> mipLevel,
			sourceTexture.height >> mipLevel,
			sourceTexture.format,
			false);

		// Копируем данные MIP-уровня
		Graphics.CopyTexture(sourceTexture, 0, mipLevel, mipTexture, 0, 0);

		// Получаем цвета пикселей
		Color[] pixels = mipTexture.GetPixels();

		width = mipTexture.width;
		height = mipTexture.height;

		/*
		// Преобразуем в массив Vector3 (например, RGB как XYZ)
		Vector3[] pixelVectors = new Vector3[pixels.Length];
		for (int i = 0; i < pixels.Length; i++)
		{
			pixelVectors[i] = new Vector3(pixels[i].r, pixels[i].g, pixels[i].b);
		}
		*/

		pixelValues = new float[pixels.Length];

		for (int i = 0; i < pixels.Length; i++)
		{
			pixelValues[i] = pixels[i].r;
		}

		// Очищаем временную текстуру
		Destroy(mipTexture);
	}
}