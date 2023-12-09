using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using RosMessageTypes.Nmea;

namespace UnitySensors.ROS
{
    /*
    [RequireComponent(typeof(GPSSensor))]
    [ExecuteAlways]
    public class NMEAPublisher : Publisher<GPSSensor, Serializer>
    {
        [SerializeField]
        private string _topicName = "gnss/raw_data";
        [SerializeField]
        private string _frame_id = "gnss_link";

        [SerializeField]
        private NMEASerializer _serializer_gps;

        protected override void Start()
        {
            base.Start();
            if (!Application.isPlaying) return;
            _serializer_gps.Start();
        }

        protected override void Init()
        {
            _ros.RegisterPublisher<SentenceMsg>(_topicName);
            _serializer_gps.Init(_frame_id);
        }

        protected override void Update()
        {
            base.Update();
            _serializer_gps.Update();
#if UNITY_EDITOR
            if (!Application.isPlaying && (_serializer_gps.format.updated))
            {
                EditorUtility.SetDirty(this);
            }
#endif
        }

        protected override void Publish(float time)
        {
            _serializer_gps.Serialize(time, _sensor.coordinate, _sensor.velocity);
            _ros.Publish(_topicName, _serializer_gps.msg);
        }
    }
    */
}
