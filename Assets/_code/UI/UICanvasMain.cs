using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Spectrum
{
    public class UICanvasMain : MonoBehaviour
    {
        public static UICanvasMain Instance;

        [SerializeField] Dataset dataset;

        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] MeshRenderer meshRendererAdditional;
        [SerializeField] int materialIndex;

        [Header("UI")]
        [SerializeField] Image background;
        [SerializeField] float backgroundAlpha;
        [SerializeField] Slider mainSlider;
        [SerializeField] Slider intensitySlider;
        [SerializeField] TextMeshProUGUI intensityValueText;
        [SerializeField] Slider circadianSlider;
        [SerializeField] TextMeshProUGUI circadianValueText;

        [Header("TM-30")]
        [SerializeField] Image TM30Image;
        [SerializeField] TextMeshProUGUI rfValueText;
        [SerializeField] TextMeshProUGUI rgValueText;
        [SerializeField] TextMeshProUGUI raValueText;
        [SerializeField] TextMeshProUGUI cctValueText;

        public int currentIndex = 0;

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

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentIndex--;

                if (currentIndex <= 0)
                    currentIndex = 0;

                ChangeMainSliderValue(false);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentIndex++;

                if (currentIndex >= dataset.dataset.Count)
                    currentIndex = dataset.dataset.Count - 1;

                ChangeMainSliderValue(false);
            }
        }

        public void Init()
        {
            mainSlider.minValue = circadianSlider.minValue = intensitySlider.minValue = 0;
            mainSlider.maxValue = circadianSlider.maxValue = dataset.dataset.Count - 1;
            intensitySlider.maxValue = 100;

            ChangeMainSliderValue(false);
        }

        public void ChangeMainSliderValue(bool isCanvasInput)
        {
            if (isCanvasInput)
                currentIndex = Mathf.FloorToInt(mainSlider.value);            

            intensitySlider.value = dataset.dataset[currentIndex].intensity;
            intensityValueText.text = intensitySlider.value.ToString();

            circadianSlider.value = currentIndex;
            circadianValueText.text = circadianSlider.value.ToString();

            TM30Image.sprite = dataset.dataset[currentIndex].TM30Image;

            background.color = new Vector4(dataset.dataset[currentIndex].emissionColor.r,
                dataset.dataset[currentIndex].emissionColor.g,
                dataset.dataset[currentIndex].emissionColor.b,
                backgroundAlpha);

            Material[] materials = meshRenderer.materials;
            materials[materialIndex].SetColor("_EmissionColor", dataset.dataset[currentIndex].emissionColor *
                dataset.dataset[currentIndex].intensity / 100);
            meshRenderer.materials = materials;
            meshRendererAdditional.materials = materials;
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
