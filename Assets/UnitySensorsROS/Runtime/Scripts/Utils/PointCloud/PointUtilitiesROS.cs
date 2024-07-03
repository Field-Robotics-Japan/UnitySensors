using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using RosMessageTypes.Sensor;

using UnitySensors.DataType.Sensor.PointCloud;

namespace UnitySensors.ROS.Utils.PointCloud
{
    public static class PointUtilitiesROS
    {
        public readonly static ReadOnlyDictionary<Type, PointFieldMsg[]> pointFields = new ReadOnlyDictionary<Type, PointFieldMsg[]>(new Dictionary<Type, PointFieldMsg[]>
        {
            {
                typeof(PointXYZ), 
                new PointFieldMsg[]
                {
                    new PointFieldMsg("y", 0, 7, 1),
                    new PointFieldMsg("z", 4, 7, 1),
                    new PointFieldMsg("x", 8, 7, 1),
                }
            },
            {
                typeof(PointXYZI),
                new PointFieldMsg[]
                {
                    new PointFieldMsg("y", 0, 7, 1),
                    new PointFieldMsg("z", 4, 7, 1),
                    new PointFieldMsg("x", 8, 7, 1),
                    new PointFieldMsg("intensity", 12, 7, 1),
                }
            },
            {
                typeof(PointXYZRGB),
                new PointFieldMsg[]
                {
                    new PointFieldMsg("y", 0, 7, 1),
                    new PointFieldMsg("z", 4, 7, 1),
                    new PointFieldMsg("x", 8, 7, 1),
                    new PointFieldMsg("rgba", 12, 6, 1)
                }
            }
        });
    }
}
