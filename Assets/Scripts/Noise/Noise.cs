using UnityEngine;

public class Noise : MonoBehaviour
{
    public bool EnableColorShift = false;
    public ShadowSetteing Setting = new ShadowSetteing();
    [System.Serializable]
    public class ShadowSetteing
    {
        public Color EffectColor;
        public Vector2 Distance;
        public bool UseAlpha;
    }

    public void ApplyNoise(ref Texture2D texture)
    {
        Color[] inputColors = texture.GetPixels();
        int width = texture.width;
        int height = texture.height;

        if (EnableColorShift)
        {
            ColorShift(ref inputColors, width, height);
        }

        texture.SetPixels(inputColors);
        texture.Apply();
    }

    private void ColorShift(ref Color[] input, int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var color = input[(width * y) + x];
                input[(width * y) + x] = new Color(color.g, color.b, color.r);
            }
        }
    }
}
