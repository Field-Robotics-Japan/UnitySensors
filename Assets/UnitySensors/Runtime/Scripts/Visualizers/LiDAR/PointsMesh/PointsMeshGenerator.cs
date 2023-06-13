using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySensors.Visualization
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class PointsMeshGenerator : MonoBehaviour
    {
        [SerializeField]
        private float _frequency = 10;

        [SerializeField]
        private Material _material;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;

        private Mesh _mesh;
        private int[] _indices;

        private float _time_last;
        private float _frequency_inv;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            if (_material) _meshRenderer.material = _material;

            _frequency_inv = 1.0f / _frequency;
        }

        private void Update()
        {
            float time_now = Time.time;
            if (time_now - _time_last < _frequency_inv) return;

            Generate();

            _time_last = time_now;
        }

        protected virtual void Generate()
        {

        }

        protected void GenerateMesh(Vector3[] points)
        {
            if (_indices == null || _indices.Length != points.Length) GenerateIndices(points.Length);
            _mesh = new Mesh();
            _mesh.vertices = points;
            _mesh.SetIndices(_indices, MeshTopology.Points, 0);
            _meshFilter.mesh = _mesh;
        }

        private void GenerateIndices(int pointsNum)
        {
            _indices = new int[pointsNum];
            for (int i = 0; i < pointsNum; i++) _indices[i] = i;
        }
    }
}
