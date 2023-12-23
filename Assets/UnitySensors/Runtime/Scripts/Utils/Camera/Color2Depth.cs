using UnityEngine;

namespace UnitySensors.Utils.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class Color2Depth : MonoBehaviour
    {
        private Material _mat;

        [System.NonSerialized]
        public float y_min = 0.0f;
        [System.NonSerialized]
        public float y_max = 1.0f;

        private void Start()
        {
            _mat = new Material(Shader.Find("UnitySensors/Color2Depth"));
            _mat.SetFloat("_F", GetComponent<UnityEngine.Camera>().farClipPlane);
            _mat.SetFloat("_Y_MIN", y_min);
            _mat.SetFloat("_Y_MAX", y_max);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture dest)
        {
            Graphics.Blit(source, dest, _mat);
        }
    }
}
