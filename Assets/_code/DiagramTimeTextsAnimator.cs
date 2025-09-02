using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Spectrum
{
    public class DiagramTimeTextsAnimator : MonoBehaviour
    {
        [Range(1, 10)]
        [SerializeField] int alphaFadeCount = 5;
        [SerializeField] float offset;
        [SerializeField] int indexesForText = 12;
        [SerializeField] float alphaPower = 1.0f;
        [SerializeField] List<TextMeshProUGUI> timeTexts = new List<TextMeshProUGUI>();

        RectTransform rt;
        Vector3 defaultPosition;

        void Awake()
        {
            rt = GetComponent<RectTransform>();
            defaultPosition = rt.position;
        }

        void Update()
        {
            UpdateTexts();
        }

        void UpdateTexts()
        {
            for (int i = 0; i < timeTexts.Count; i++)
            {
                float alphaValue = (float)(alphaFadeCount - i + (float)UICanvasMain.Instance.currentIndex / indexesForText) / alphaFadeCount;

                if (alphaValue > 1)
                    alphaValue = Mathf.Clamp01(Mathf.Pow(2f - alphaValue, alphaPower));
                else if (alphaValue < 0)
                    alphaValue = 0;
                
                timeTexts[i].alpha = alphaValue;
            }

            rt.position = new Vector3(defaultPosition.x, defaultPosition.y, offset * UICanvasMain.Instance.currentIndex);
        }
    }
}
