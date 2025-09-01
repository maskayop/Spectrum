using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Spectrum
{
    [Serializable]
    public class RawStringData
    {
        [TextArea(1, 5)]
        public string rawData;
    }
    
    [Serializable]
    public class Data
    {
        public string name;

        [Header("TM-30")]
        public Sprite TM30Image;
        public float rf;
        public float rg;
        public float ra;
        public float cct;
        
        [Space(20)]
        public RawStringData rawStringData;
        public float intensity;
        public float lumen;
        public float photopic;
        public float cs;
        public float mder;
        public float medi;
        public Color emissionColor;

        [Header("Processed data")]
        [HideInInspector]
        public double spectralAmplitudeDivider = 1;
        public List<double> spectralData = new List<double>();

        public void Init()
        {
            ParseNumbers();
            emissionColor = KelvinToColor(cct);
        }

        void ParseNumbers()
        {
            // Разбиваем строку на массив строк по переносу строки
            string[] lines = rawStringData.rawData.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // Очищаем список перед добавлением
            spectralData.Clear();

            // Парсим каждую строку в float и добавляем в список
            foreach (string line in lines)
            {
                // Убираем пробелы и проверяем, можно ли преобразовать в float
                if (float.TryParse(line.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out float number))
                    spectralData.Add(number / spectralAmplitudeDivider);
                else
                    Debug.LogWarning($"Не удалось преобразовать строку '{line}' в число.");
            }
        }

        Color KelvinToColor(float kelvin)
        {
            if (kelvin == 0)
                return Color.black;

            // Ограничим температуру в разумных пределах (1000–40000 K)
            kelvin = Mathf.Clamp(kelvin, 1000f, 40000f);

            // Делим на 100 для упрощения формул
            float temp = kelvin / 100f;

            float r, g, b;

            // Вычисление красного канала
            if (temp <= 66)
            {
                r = 255;
            }
            else
            {
                r = temp - 60;
                r = 329.698727446f * Mathf.Pow(r, -0.1332047592f);
                r = Mathf.Clamp(r, 0f, 255f);
            }

            // Вычисление зелёного канала
            if (temp <= 66)
            {
                g = temp;
                g = 99.4708025861f * Mathf.Log(g) - 161.1195681661f;
                g = Mathf.Clamp(g, 0f, 255f);
            }
            else
            {
                g = temp - 60;
                g = 288.1221695283f * Mathf.Pow(g, -0.0755148492f);
                g = Mathf.Clamp(g, 0f, 255f);
            }

            // Вычисление синего канала
            if (temp >= 66)
            {
                b = 255;
            }
            else if (temp <= 19)
            {
                b = 0;
            }
            else
            {
                b = temp - 10;
                b = 138.5177312231f * Mathf.Log(b) - 305.0447927307f;
                b = Mathf.Clamp(b, 0f, 255f);
            }

            // Возвращаем нормализованный цвет в диапазоне [0, 1] для Unity
            return new Color(r / 255f, g / 255f, b / 255f);
        }
    }

    public class Dataset : MonoBehaviour
    {
        public double spectralAmplitudeDivider = 1;
        public List<Data_Asset> dataAssets = new List<Data_Asset>();
    }
}
