using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

namespace UnitySensors.Utils.Texture
{
    public class TextureLoader
    {
        public bool success { get; private set; }
        public RenderTexture source;
        public Texture2D destination;

        public IEnumerator LoadTextureAsync()
        {
            success = false;
            Graphics.CopyTexture(source, destination);
            bool completed = false;
            // Copy destination texture from GPU to CPU
            var textureData = destination.GetRawTextureData<Color>();
            AsyncGPUReadback.RequestIntoNativeArray(ref textureData, destination, 0, request =>
            {
                if (request.hasError)
                {
                    Debug.LogError("GPU readback error detected.");
                    success = false;
                }
                else success = true;
                completed = true;
            });
            yield return new WaitUntil(() => completed);
        }
    }
}