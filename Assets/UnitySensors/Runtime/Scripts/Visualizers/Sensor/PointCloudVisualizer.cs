using UnityEngine;
using UnitySensors.Interface.Sensor;
using UnitySensors.Interface.Sensor.PointCloud;
using UnitySensors.Utils.PointCloud;

namespace UnitySensors.Visualization.Sensor
{
    public class PointCloudVisualizer<T> : Visualizer where T : struct, IPointInterface
    {
        [SerializeField] private PointUtilitiesSO _pointUtilitiesSO;
        private IPointCloudInterface<T> _sourceInterface;
        private Transform _transform;

        private Material _mat;
        private Mesh _mesh;
        private ComputeBuffer _pointsBuffer;
        private ComputeBuffer _argsBuffer;

        private uint[] _args = new uint[5] { 0, 0, 0, 0, 0 };
        private int _cachedPointsCount = -1;
        private int _bufferSize;

        public void SetSource(IPointCloudInterface<T> sourceInterface)
        {
            _sourceInterface = sourceInterface;
        }

        protected virtual void Start()
        {


            _transform = this.transform;
            _bufferSize = PointUtilitiesSO.pointDataSizes[typeof(T)];
            _mat = new Material(_pointUtilitiesSO.GetPointMaterialTemplate(typeof(T)));
            _mat.renderQueue = 3000;
            // Quad
            _mesh = new Mesh();
            _mesh.vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, 0),
                new Vector3( 0.5f, -0.5f, 0),
                new Vector3( 0.5f,  0.5f, 0),
                new Vector3(-0.5f,  0.5f, 0)
            };
            _mesh.uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1)
            };
            _mesh.SetIndices(new int[] { 0, 1, 2, 2, 3, 0 }, MeshTopology.Triangles, 0);
            _argsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            UpdateBuffers();
        }

        protected override void Visualize()
        {
            if (_sourceInterface.pointsNum != _cachedPointsCount) UpdateBuffers();
            _mat.SetMatrix("LocalToWorldMatrix", _transform.localToWorldMatrix);
            _pointsBuffer.SetData(_sourceInterface.pointCloud.points);
        }

        private void Update()
        {
            Graphics.DrawMeshInstancedIndirect(_mesh, 0, _mat, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), _argsBuffer);
        }

        private void UpdateBuffers()
        {
            if (_pointsBuffer != null) _pointsBuffer.Release();
            _pointsBuffer = new ComputeBuffer(_sourceInterface.pointsNum, _bufferSize);
            _pointsBuffer.SetData(_sourceInterface.pointCloud.points);
            _mat.SetBuffer("PointsBuffer", _pointsBuffer);

            uint numIndices = (_mesh != null) ? (uint)_mesh.GetIndexCount(0) : 0;
            _args[0] = numIndices;
            _args[1] = (uint)_sourceInterface.pointsNum;
            _argsBuffer.SetData(_args);

            _cachedPointsCount = _sourceInterface.pointsNum;
        }

        private void OnDisable()
        {
            if (_pointsBuffer != null) _pointsBuffer.Release();
            _pointsBuffer = null;
            if (_argsBuffer != null) _argsBuffer.Release();
            _argsBuffer = null;
        }
    }
}