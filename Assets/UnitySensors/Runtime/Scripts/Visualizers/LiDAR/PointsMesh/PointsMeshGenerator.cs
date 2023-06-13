using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySensors.Visualization
{
    [System.Serializable]
    public class PointsMeshGenerator
    {
        [SerializeField]
        private MeshFilter _meshFilter;

        private Mesh _mesh;
        private int[] _indices;

        public void Init(int pointsNum)
        {
            _indices = new int[pointsNum];
            for(int i = 0; i < pointsNum; i++)
            {
                _indices[i] = i;
            }
        }

        public void Generate(Vector3[] points)
        {
            _mesh = new Mesh();
            _meshFilter.mesh = _mesh;
            _mesh.vertices = points;
            _mesh.SetIndices(_indices, MeshTopology.Points, 0);
        }
    }
}
