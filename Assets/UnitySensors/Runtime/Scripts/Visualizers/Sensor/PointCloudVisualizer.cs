using UnityEngine;
using UnitySensors.Interface.Sensor;
using UnitySensors.Interface.Sensor.PointCloud;
using UnitySensors.Utils.PointCloud;

namespace UnitySensors.Visualization.Sensor
{
    public class PointCloudVisualizer<T> : Visualizer where T : struct, IPointInterface
    {
        private Transform _transform;
        private IPointCloudInterface<T> _source;

        private Material _mat;
        private Mesh _mesh;
        private ComputeBuffer _pointsBuffer;
        private ComputeBuffer _argsBuffer;

        private uint[] _args = new uint[5] { 0, 0, 0, 0, 0 };
        private int _cachedPointsCount = -1;
        private int _bufferSize;

        protected override void Init(MonoBehaviour source)
        {
            Debug.Assert(source is IPointCloudInterface<T>, "No compatibility between source and visualizer.", this);
            _transform = source.transform;
            _source = (IPointCloudInterface<T>)source;
            _bufferSize = PointUtilities.pointDataSizes[typeof(T)];
            _mat = new Material(Shader.Find(PointUtilities.shaderNames[typeof(T)]));
            _mat.renderQueue = 3000;
            _mesh = new Mesh();
            _mesh.vertices = new Vector3[1] { Vector3.zero };
            _mesh.SetIndices(new int[1] { 0 }, MeshTopology.Points, 0);
            _argsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            UpdateBuffers();
        }

        protected override void Visualize()
        {
            if (_source.pointsNum != _cachedPointsCount) UpdateBuffers();
            _mat.SetMatrix("LocalToWorldMatrix", _transform.localToWorldMatrix);
            _pointsBuffer.SetData(_source.pointCloud.points);
        }

        private void Update()
        {
            Graphics.DrawMeshInstancedIndirect(_mesh, 0, _mat, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), _argsBuffer);
        }

        private void UpdateBuffers()
        {
            if (_pointsBuffer != null) _pointsBuffer.Release();
            _pointsBuffer = new ComputeBuffer(_source.pointsNum, _bufferSize);
            _pointsBuffer.SetData(_source.pointCloud.points);
            _mat.SetBuffer("PointsBuffer", _pointsBuffer);

            uint numIndices = (_mesh != null) ? (uint)_mesh.GetIndexCount(0) : 0;
            _args[0] = numIndices;
            _args[1] = (uint)_source.pointsNum;
            _argsBuffer.SetData(_args);

            _cachedPointsCount = _source.pointsNum;
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