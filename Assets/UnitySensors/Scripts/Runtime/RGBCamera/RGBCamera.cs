using UnityEngine;

namespace FRJ.Sensor
{
  [RequireComponent(typeof(Camera))]
  public class RGBCamera : MonoBehaviour
  {
    [SerializeField] private int _width  = 640;
    [SerializeField] private int _height = 480;
    [SerializeField] [Range(0,100)] int _quality = 50;
    [SerializeField] private float _scanRate = 30f;

    public uint width  { get => (uint)this._width; }
    public uint height { get => (uint)this._height; }
    public float scanRate { get => this._scanRate; }
    
    private Camera _camera;
    private Texture2D _texture;
    private Rect _rect;

    [HideInInspector] public byte[] data;

    public void Init()
    {
      this._camera  = GetComponent<Camera>();
      this._texture = new Texture2D(this._width, this._height, TextureFormat.RGB24, false);
      this._rect = new Rect(0, 0, this._width, this._height);
      this._texture.Apply();
      this._camera.targetTexture = new RenderTexture(this._width, this._height, 24);

      Camera.onPostRender += UpdateImage;
    }

    private void UpdateImage(Camera _camera)
    {
      if (this._texture != null && _camera == this._camera) {
        this._texture.ReadPixels(this._rect, 0, 0);
        this.data = this._texture.EncodeToJPG(this._quality);
      }
    }        
  }
}
