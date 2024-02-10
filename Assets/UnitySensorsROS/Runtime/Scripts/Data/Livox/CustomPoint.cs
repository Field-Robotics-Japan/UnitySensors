using RosMessageTypes.Livox;

namespace UnitySensors.ROS.Data.Livox
{
    public struct CustomPoint
    {
        public uint offset_time;
        public float x;
        public float y;
        public float z;
        public byte reflectivity;
        public byte tag;
        public byte line;

        public CustomPointMsg ConvertToMsg()
        {
            return new CustomPointMsg()
            {
                offset_time = offset_time,
                x = x,
                y = y,
                z = z,
                reflectivity = reflectivity,
                tag = tag,
                line = line
            };
        }
    }
}