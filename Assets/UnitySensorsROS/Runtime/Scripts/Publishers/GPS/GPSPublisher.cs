using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using RosMessageTypes.Nmea;

namespace UnitySensors.ROS
{
    [RequireComponent(typeof(GPSSensor))]
    [ExecuteAlways]
    public class GPSPublisher : Publisher<GPSSensor, GPSSerializer>
    {
        [SerializeField]
        private string _topicName = "gnss/raw_data";
        [SerializeField]
        private string _frame_id = "gnss_link";

        protected override void Start()
        {
            base.Start();
            if (!Application.isPlaying) return;
            _serializer.Start();
        }

        protected override void Init()
        {
            _ros.RegisterPublisher<SentenceMsg>(_topicName);
            _serializer.Init(_frame_id);
        }

        protected override void Update()
        {
            base.Update();
            _serializer.Update();
            if (!Application.isPlaying && (_serializer.format.updated))
            {
                EditorUtility.SetDirty(this);
            }
        }

        protected override void Publish(float time)
        {
            _serializer.Serialize(time, _sensor.coordinate, _sensor.velocity);
            _ros.Publish(_topicName, _serializer.msg);
        }
    }
}
