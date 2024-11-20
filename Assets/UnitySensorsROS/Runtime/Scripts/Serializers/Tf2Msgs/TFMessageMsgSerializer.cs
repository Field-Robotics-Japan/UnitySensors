using System.Collections.Generic;

using UnityEngine;

using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Std;
using RosMessageTypes.Geometry;
using RosMessageTypes.Tf2;

using UnitySensors.Sensor.TF;
using UnitySensors.ROS.Serializer.Std;

namespace UnitySensors.ROS.Serializer.Tf2
{

    [System.Serializable]
    public class TFMessageMsgSerializer : RosMsgSerializer<TFMessageMsg>
    {
        [SerializeField]
        private TFLink _source;
        [SerializeField]
        private HeaderSerializer _header;

        public override void Init()
        {
            base.Init();
            _header.Init();
        }

        public override TFMessageMsg Serialize()
        {
            HeaderMsg headerMsg = _header.Serialize();
            List<TransformStampedMsg> transforms = new List<TransformStampedMsg>();

            TFData[] tfData = _source.GetTFData();
            foreach (TFData data in tfData)
            {
                TransformStampedMsg transform = new TransformStampedMsg();
                transform.header = new HeaderMsg();
                transform.header.stamp = headerMsg.stamp;
                transform.header.seq = headerMsg.seq;
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