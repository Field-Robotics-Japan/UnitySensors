using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySensors.DataType.Geometry
{
    public class Vector3D
    {
        public double x;
        public double y;
        public double z;

        public Vector3D()
        {
            x = y = z = 0;
        }

        public Vector3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3D(Vector3 vec)
        {
            x = vec.x;
            y = vec.y;
            z = vec.z;
        }

        public void FromVector3(Vector3 source)
        {
            x = source.x;
            y = source.y;
            z = source.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3((float)x, (float)y, (float)z);
        }
    }
}
