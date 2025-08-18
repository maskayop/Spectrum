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

                if (currentIndex >= dataset.dataAssets.Count)
                    currentIndex = dataset.dataAssets.Count - 1;

                ChangeMainSliderValue(false);
            }
        }

        public void Init()
        {
            mainSlider.minValue = circadianSlider.minValue = intensitySlider.minValue = 0;
            mainSlider.maxValue = circadianSlider.maxValue = dataset.dataAssets.Count - 1;
            intensitySlider.maxValue = 100;

            ChangeMainSliderValue(false);
        }

        public void ChangeMainSliderValue(bool isCanvasInput)
        {
            if (isCanvasInput)
                currentIndex = Mathf.FloorToInt(mainSlider.value);            

            intensitySlider.value = dataset.dataAssets[currentIndex].data.intensity;
            intensityValueText.text = intensitySlider.value.ToString();

            circadianSlider.value = currentIndex;
            circadianValueText.text = circadianSlider.value.ToString();

            TM30Image.sprite = dataset.dataAssets[currentIndex].data.TM30Image;

            background.color = new Vector4(dataset.dataAssets[currentIndex].data.emissionColor.r,
                dataset.dataAssets[currentIndex].data.emissionColor.g,
                dataset.dataAssets[currentIndex].data.emissionColor.b,
                backgroundAlpha);

            Material[] materials = meshRenderer.materials;
            materials[materialIndex].SetColor("_EmissionColor", dataset.dataAssets[currentIndex].data.emissionColor *
                dataset.dataAssets[currentIndex].data.intensity / 100);
            meshRenderer.materials = materials;
            meshRendererAdditional.materials = materials;

            rfValueText.text = dataset.dataAssets[currentIndex].data.rf.ToString();
            rgValueText.text = dataset.dataAssets[currentIndex].data.rg.ToString();
            raValueText.text = dataset.dataAssets[currentIndex].data.ra.ToString();
            cctValueText.text = dataset.dataAssets[currentIndex].data.cct.ToString();
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
