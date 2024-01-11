using UnityEngine;
using RosMessageTypes.Sensor;
using UnitySensors.Data.Texture;
using UnitySensors.Sensor;
using UnitySensors.ROS.Serializer.Image;

namespace UnitySensors.ROS.Publisher.PointCloud
{
    public class ImageMsgPublisher<T> : RosMsgPublisher<T, ImageMsgSerializer<T>, CompressedImageMsg> where T : UnitySensor, ITextureInterface
    {
    }
}
