using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Nmea;

namespace UnitySensors.ROS
{
    [System.Serializable]
    public class GPSSerializer : Serializer
    {
        [SerializeField]
        private NMEAFormatManager _format;

        private SentenceMsg _msg;
        private AutoHeader _header;

        public NMEAFormatManager format { get => _format; }
        public SentenceMsg msg { get => _msg; }

        public void Start()
        {
            _format.Start();
        }

        public void Init(string frame_id)
        {
            _msg = new SentenceMsg();
            _header = new AutoHeader();

            _header.Init(frame_id);
        }

        public void Update()
        {
            _format.Update();
        }

        public void Serialize(float time, GeoCoordinate coordinate, Vector3 velocity)
        {
            _header.Serialize(time);
            _msg.sentence = _format.Serialize(coordinate, velocity);
            _msg.header = _header.header;
        }
    }
}
