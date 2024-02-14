using System.Collections.Generic;

using UnityEngine;

using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
using RosMessageTypes.Tf2;

using UnitySensors.Sensor.TF;

namespace UnitySensors.ROS.Serializer.TF
{
    using TFSensor = Sensor.TF.TF;

    [System.Serializable]
    public class TFMsgSerializer : RosMsgSerializer<TFSensor, TFMessageMsg>
    {
        [SerializeField]
        private HeaderSerializer _header;

        public override void Init(TFSensor sensor)
        {
            base.Init(sensor);
            _header.Init(sensor);
        }

        public override TFMessageMsg Serialize()
        {
            HeaderMsg headerMsg = _header.Serialize();
            List<TransformStampedMsg> transforms = new List<TransformStampedMsg>();

            TFData[] tfData = sensor.GetTFData();
            foreach(TFData data in tfData)
            {
                TransformStampedMsg transform = new TransformStampedMsg();
                transform.header.seq = headerMsg.seq;
                transform.header.stamp = headerMsg.stamp;
                transform.header.frame_id = data.frame_id_parent;
                transform.child_frame_id = data.frame_id_child;
                transform.transform.translation = data.position.To<FLU>();
                transform.transform.rotation = data.rotation.To<FLU>();
                transforms.Add(transform);
            }
            _msg.transforms = transforms.ToArray();
            return _msg;
        }
    }
}