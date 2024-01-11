using System;
using System.Collections.Generic;
using UnityEngine;
using UnitySensors.Sensor;
using UnitySensors.Data.PointCloud;

namespace UnitySensors.Visualization.PointCloud
{
    public class PointCloudVisualizer<T, TT> : Visualizer<T>
        where T : UnitySensor, IPointCloudInterface<TT>
        where TT : struct, IPointInterface
    {
        private Material _mat;
        private Mesh _mesh;

        private ComputeBuffer _pointsBuffer;
        private ComputeBuffer _argsBuffer;

        private uint[] _args = new uint[5] { 0, 0, 0, 0, 0 };

        private int _cachedPointsCount = -1;

        private int _bufferSize;

        private static readonly Dictionary<Type, int> BUFFER_SIZE_DICTIONARY = new Dictionary<Type, int>()
        {
            { typeof(PointXYZ), 12 },
            { typeof(PointXYZI), 16 }
        };

        private static readonly Dictionary<Type, string> SHADER_NAME_DICTIONARY = new Dictionary<Type, string>()
        {
            { typeof(PointXYZ), "UnitySensors/PointCloudXYZ" },
            { typeof(PointXYZI), "UnitySensors/PointCloudXYZI" }
        };

        protected override void Init()
        {
            _bufferSize = BUFFER_SIZE_DICTIONARY[typeof(TT)];
            _mat = new Material(Shader.Find(SHADER_NAME_DICTIONARY[typeof(TT)]));
            _mat.renderQueue = 3000;
            _mesh = new Mesh();
            _mesh.vertices = new Vector3[1] { Vector3.zero };
            _mesh.SetIndices(new int[1] { 0 }, MeshTopology.Points, 0);
            _argsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            UpdateBuffers();
        }

        protected override void Visualize()
        {
            if (sensor.pointsNum != _cachedPointsCount) UpdateBuffers();
            _mat.SetMatrix("LocalToWorldMatrix", sensor.transform.localToWorldMatrix);
            _pointsBuffer.SetData(sensor.pointCloud.points);
        }

        private void Update()
        {
            Graphics.DrawMeshInstancedIndirect(_mesh, 0, _mat, new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f)), _argsBuffer);
        }

        private void UpdateBuffers()
        {
            if (_pointsBuffer != null) _pointsBuffer.Release();
            _pointsBuffer = new ComputeBuffer(sensor.pointsNum, _bufferSize);
            _pointsBuffer.SetData(sensor.pointCloud.points);
            _mat.SetBuffer("PointsBuffer", _pointsBuffer);

            uint numIndices = (_mesh != null) ? (uint)_mesh.GetIndexCount(0) : 0;
            _args[0] = numIndices;
            _args[1] = (uint)sensor.pointsNum;
            _argsBuffer.SetData(_args);

            _cachedPointsCount = sensor.pointsNum;
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