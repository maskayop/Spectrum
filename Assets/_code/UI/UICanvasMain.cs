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
        [SerializeField] Slider circadianSlider;
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
            mainSlider.minValue = circadianSlider.minValue = 0;
            mainSlider.maxValue = circadianSlider.maxValue = dataset.dataset.Count - 1;
            mainSlider.value = 0;
            circadianSlider.value = 0;
        }

        public void ChangeMainSliderValue()
        {
            currentIndex = Mathf.FloorToInt(mainSlider.value);
            circadianSlider.value = currentIndex;

            TM30Image.sprite = dataset.dataset[currentIndex].TM30Image;
        }
    }
}
