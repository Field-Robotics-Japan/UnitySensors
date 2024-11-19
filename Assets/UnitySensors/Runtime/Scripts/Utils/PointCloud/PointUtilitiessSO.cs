
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnitySensors.DataType.Sensor.PointCloud;
namespace UnitySensors.Utils.PointCloud
{
    [CreateAssetMenu(fileName = "PointUtilitiesSO", menuName = "UnitySensors/PointUtilitiesSO", order = 1)]
    public class PointUtilitiesSO : ScriptableObject
    {

        // TODO: There should be a better way to handle this
        [SerializeField] private Material _pointCloudXYZMaterial;
        [SerializeField] private Material _pointCloudXYZIMaterial;
        [SerializeField] private Material _pointCloudXYZRGBMaterial;

        public readonly static ReadOnlyDictionary<Type, int> pointDataSizes = new ReadOnlyDictionary<Type, int>(new Dictionary<Type, int>
        {
            { typeof(PointXYZ), 12 },
            { typeof(PointXYZI), 16 },
            { typeof(PointXYZRGB), 16 }
        });

        public Material GetPointMaterialTemplate(Type type)
        {
            if (type == typeof(PointXYZ))
            {
                return _pointCloudXYZMaterial;
            }
            else if (type == typeof(PointXYZI))
            {
                return _pointCloudXYZIMaterial;
            }
            else if (type == typeof(PointXYZRGB))
            {
                return _pointCloudXYZRGBMaterial;
            }
            else
            {
                throw new ArgumentException("Unsupported point type");
            }
        }
    }
}