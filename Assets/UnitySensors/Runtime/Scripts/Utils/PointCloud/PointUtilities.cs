using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using UnitySensors.DataType.Sensor.PointCloud;

namespace UnitySensors.Utils.PointCloud
{
    public static class PointUtilities
    {
        public readonly static ReadOnlyDictionary<Type, int> pointDataSizes = new ReadOnlyDictionary<Type, int>(new Dictionary<Type, int>
        {
            { typeof(PointXYZ), 12 },
            { typeof(PointXYZI), 16 },
            { typeof(PointXYZRGB), 16 }
        });

        public readonly static ReadOnlyDictionary<Type, string> shaderNames = new ReadOnlyDictionary<Type, string>(new Dictionary<Type, string>
        {
            { typeof(PointXYZ), "UnitySensors/PointCloudXYZ" },
            { typeof(PointXYZI), "UnitySensors/PointCloudXYZI" },
            { typeof(PointXYZRGB), "UnitySensors/PointCloudXYZRGB" }
        });
    }
}
