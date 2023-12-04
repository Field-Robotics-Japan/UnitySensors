using System.Collections;
using System.Collections.Generic;
using RosMessageTypes.Geometry;
using RosMessageTypes.Tf2;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEngine;

using TFData = TFSensor.TFData;

namespace UnitySensors.ROS
{

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
}