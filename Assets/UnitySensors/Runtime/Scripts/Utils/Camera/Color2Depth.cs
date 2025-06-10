using UnityEngine;

namespace UnitySensors.Utils.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class Color2Depth : MonoBehaviour
    {
        [System.NonSerialized]
        public Material mat_source;

        [System.NonSerialized]
        public float y_min = 0.0f;
        [System.NonSerialized]
        public float y_max = 1.0f;
        [System.NonSerialized]
        public float y_coef = 1.0f;
        private Material _mat;

        private void Start()
        {
            _mat = new(mat_source);
            _mat.SetFloat("_Y_MIN", y_min);
            _mat.SetFloat("_Y_MAX", y_max);
            _mat.SetFloat("_Y_COEF", y_coef);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture dest)
        {
            Graphics.Blit(null, dest, _mat);
        }
    }
}
