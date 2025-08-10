using UnityEngine;

namespace Spectrum
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] DiagramGenerator diagramGenerator;
        [SerializeField] float verticalOffset;

        [Header("Mesh")]
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] float tiling = 1;

        float prevTiling;

        void Start()
        {
            Init();
        }

        void Update()
        {
            ChangeGridTiling();
        }

        public void Init()
        {
            if (!diagramGenerator)
                Destroy(gameObject);

            transform.transform.localPosition = new Vector3(0, verticalOffset, 0);
            transform.localScale = new Vector3(diagramGenerator.width, 1, diagramGenerator.height);
        }

        void ChangeGridTiling()
        {
            if (!meshRenderer)
                return;

            if (prevTiling == tiling)
                return;

            Vector4 tilingAndOffset = new Vector4(tiling, tiling, 0f, 0f);
            meshRenderer.material.SetVector("_BaseMap_ST", tilingAndOffset);
            
            prevTiling = tiling;
        }
    }
}
