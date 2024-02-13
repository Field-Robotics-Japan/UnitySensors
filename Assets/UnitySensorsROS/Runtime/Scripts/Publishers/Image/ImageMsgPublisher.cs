using UnityEngine;
using RosMessageTypes.Sensor;
using UnitySensors.Data.Texture;
using UnitySensors.Sensor;
using UnitySensors.ROS.Serializer.Image;

namespace UnitySensors.ROS.Publisher.Image
{
    public class ImageMsgPublisher<T> : RosMsgPublisher<T, ImageMsgSerializer<T>, CompressedImageMsg> where T : UnitySensor, ITextureInterface
    {
    }
}
