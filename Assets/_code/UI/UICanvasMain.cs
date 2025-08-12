using UnityEngine;
using UnityEngine.UI;

namespace Spectrum
{
    public class UICanvasMain : MonoBehaviour
    {
        public static UICanvasMain Instance;

        [SerializeField] Dataset dataset;

        [Header("UI")]
        [SerializeField] Slider mainSlider;
        [SerializeField] Image TM30Image;

        int currentIndex = 0;

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Cannot create UICanvasMain");
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        void Start()
        {
            Init();
        }

        public void Init()
        {
            mainSlider.minValue = 0;
            mainSlider.maxValue = dataset.dataset.Count - 1;
            mainSlider.value = 0;
        }

        public void ChangeSliderValue()
        {
            currentIndex = Mathf.FloorToInt(mainSlider.value);

            TM30Image.sprite = dataset.dataset[currentIndex].TM30Image;
        }
    }
}
