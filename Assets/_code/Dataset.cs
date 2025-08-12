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
        public RawStringData rawStringData;

        [Header("Processed data")]
        [HideInInspector]
        public double spectralAmplitudeDivider = 1;
        public List<double> spectralData = new List<double>();

        public void Init()
        {
            ParseNumbers();
        }

        void ParseNumbers()
        {
            // –азбиваем строку на массив строк по переносу строки
            string[] lines = rawStringData.rawData.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // ќчищаем список перед добавлением
            spectralData.Clear();

            // ѕарсим каждую строку в float и добавл€ем в список
            foreach (string line in lines)
            {
                // ”бираем пробелы и провер€ем, можно ли преобразовать в float
                if (float.TryParse(line.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out float number))
                    spectralData.Add(number / spectralAmplitudeDivider);
                else
                    Debug.LogWarning($"Ќе удалось преобразовать строку '{line}' в число.");
            }
        }
    }

    public class Dataset : MonoBehaviour
    {
        public double spectralAmplitudeDivider = 1;
        public List<Data> dataset = new List<Data>();
    }
}
