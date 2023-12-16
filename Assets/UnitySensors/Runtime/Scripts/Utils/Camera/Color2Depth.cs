using UnityEngine;

namespace UnitySensors.Utils.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class Color2Depth : MonoBehaviour
    {
        private Material _mat;

        private void Awake()
        {
            _mat = new Material(Shader.Find("UnitySensors/Color2Depth"));
        }

        private void OnRenderImage(RenderTexture source, RenderTexture dest)
        {
            Graphics.Blit(source, dest, _mat);
        }
    }
}
