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
    /*
    [System.Serializable]
    public class TFSerializer : Serializer
    {

        [SerializeField]
        private TFMessageMsg _msg;

        private AutoHeader _header;

        public TFMessageMsg msg { get => _msg; }

        public void Init()
        {
            _msg = new TFMessageMsg();
            _header = new AutoHeader();

            _header.Init("");
        }

        public TFMessageMsg Serialize(float time, TFData[] tf)
        {
            _header.Serialize(time);
            List<TransformStampedMsg> transforms = new List<TransformStampedMsg>();
            foreach(TFData tfData in tf)
            {
                TransformStampedMsg transform = new TransformStampedMsg();
                transform.header = _header.header;
                transform.header.frame_id = tfData.frame_id_parent;
                transform.child_frame_id = tfData.frame_id_child;
                transform.transform.translation = tfData.position.To<FLU>();
                transform.transform.rotation = tfData.rotation.To<FLU>();
                transforms.Add(transform);
            }
            _msg.transforms = transforms.ToArray();
            return _msg;
        }
    }
    */
}