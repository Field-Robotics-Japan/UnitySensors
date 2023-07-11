using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySensors.Visualization
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class VelodynePointsMeshGenerator : PointsMeshGenerator
    {
        [SerializeField]
        private VelodyneSensor _sensor;

        protected override void Generate()
        {
            if (!_sensor) return;
            _sensor.CompleteJob();
            GenerateMesh(_sensor.points.ToArray());
        }
    }
}
