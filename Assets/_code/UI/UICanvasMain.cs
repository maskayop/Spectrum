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

        [Header("Background")]
        [SerializeField] Image background;
        [SerializeField] float backgroundAlpha;

        [Header("Top")]
        [SerializeField] Slider intensitySlider;
        [SerializeField] TextMeshProUGUI intensityValueText;
        [SerializeField] Slider lumenSlider;
        [SerializeField] TextMeshProUGUI lumenValueText;
        [SerializeField] Slider photopicSlider;
        [SerializeField] TextMeshProUGUI photopicValueText;

        [Header("Center")]
        [SerializeField] Slider csSlider;
        [SerializeField] TextMeshProUGUI csValueText;
        [SerializeField] Slider mderSlider;
        [SerializeField] TextMeshProUGUI mderValueText;
        [SerializeField] Slider mediSlider;
        [SerializeField] TextMeshProUGUI mediValueText;

        [Header("Bottom")]
        [SerializeField] Slider mainSlider;
        [SerializeField] float animationDelay = 1.0f;

        [Header("TM-30")]
        [SerializeField] Image TM30Image;
        [SerializeField] TextMeshProUGUI rfValueText;
        [SerializeField] TextMeshProUGUI rgValueText;
        [SerializeField] TextMeshProUGUI raValueText;
        [SerializeField] TextMeshProUGUI cctValueText;

        public int currentIndex = 0;

        float currentTime = 0;
        bool playAnimation = false;

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

            if (!playAnimation)
                return;

            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = animationDelay;
                currentIndex++;

                if (currentIndex >= dataset.dataAssets.Count)
                    currentIndex = 0;

                ChangeMainSliderValue(false);
            }
        }

        public void Init()
        {
            mainSlider.minValue = 0;
            mainSlider.maxValue = dataset.dataAssets.Count - 1;
            intensitySlider.maxValue = 100;

            ChangeMainSliderValue(false);
        }

        public void ChangeMainSliderValue(bool isCanvasInput)
        {
            if (isCanvasInput)
                currentIndex = Mathf.FloorToInt(mainSlider.value);
            else
                mainSlider.value = currentIndex;

                intensitySlider.value = dataset.dataAssets[currentIndex].data.intensity;
            intensityValueText.text = intensitySlider.value.ToString();

            lumenSlider.value = dataset.dataAssets[currentIndex].data.lumen;
            lumenValueText.text = lumenSlider.value.ToString();

            photopicSlider.value = dataset.dataAssets[currentIndex].data.photopic;
            photopicValueText.text = photopicSlider.value.ToString();

            csSlider.value = dataset.dataAssets[currentIndex].data.cs;
            csValueText.text = csSlider.value.ToString();

            mderSlider.value = dataset.dataAssets[currentIndex].data.mder;
            mderValueText.text = mderSlider.value.ToString();

            mediSlider.value = dataset.dataAssets[currentIndex].data.medi;
            mediValueText.text = mediSlider.value.ToString();

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

        public void PlayAnimation(bool state)
        {
            playAnimation = state;
        }
    }
}
