using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Burst;
using Unity.Collections;
using UnityEngine.Jobs;
using Unity.Jobs;

using RosMessageTypes.Velodyne;

namespace UnitySensors.ROS
{
    /*
    [System.Serializable]
    public class VelodyneMsgSerializer : Serializer
    {
        private VelodyneScanMsg _msg;
        private AutoHeader _header;

        private JobHandle _handle;
        private DistancesToVelodyneScanMsgJob _job;

        private NativeArray<byte> _packets;

        private int _jobSize;

        public VelodyneScanMsg msg { get => _msg; }

        public void Init(string frame_id, ref NativeArray<float> distances, ref NativeArray<float> intensities, int pointsNum, int azimuthResolution)
        {
            _header = new AutoHeader();
            _header.Init(frame_id);

            _msg = new VelodyneScanMsg();
            _msg.packets = new VelodynePacketMsg[azimuthResolution/12];
            for (int i = 0; i < _msg.packets.Length; i++)
            {
                _msg.packets[i] = new VelodynePacketMsg();
                _msg.packets[i].data = new byte[1206];
            }

            _packets = new NativeArray<byte>(pointsNum / 12 * 1260, Allocator.Persistent);

            _jobSize = azimuthResolution / 12;

            _job = new DistancesToVelodyneScanMsgJob
            {
                azimuth_coef = 1200.0f * 360.0f / azimuthResolution,
                timeStamp = 0,
                distances = distances,
                intensities = intensities,
                packets = _packets
            };
        }

        public VelodyneScanMsg Serialize(float time)
        {
            _handle = _job.Schedule(_jobSize, 1);
            JobHandle.ScheduleBatchedJobs();
            _handle.Complete();
            
            _header.Serialize(time);
            
            _msg.header = _header.header;

            uint sec = _header.header.stamp.sec;
            uint nanosec = _header.header.stamp.nanosec;

            for (int i = 0; i < _msg.packets.Length; i++)
            {
                _msg.packets[i].data = _packets.GetSubArray(i * 1206, 1206).ToArray();
                _msg.packets[i].stamp.sec = sec;
                _msg.packets[i].stamp.nanosec = nanosec;
            }

            return _msg;
        }

        public void Dispose()
        {
            _handle.Complete();
            _packets.Dispose();
        }

        [BurstCompile]
        public struct DistancesToVelodyneScanMsgJob : IJobParallelFor
        {
            [ReadOnly]
            public float azimuth_coef;
            [ReadOnly]
            public float timeStamp;

            [ReadOnly, NativeDisableParallelForRestriction]
            public NativeArray<float> distances;
            [ReadOnly, NativeDisableParallelForRestriction]
            public NativeArray<float> intensities;

            [NativeDisableParallelForRestriction]
            public NativeArray<byte> packets;
            
            public void Execute(int index)
            {
                ushort azimuth = (ushort)Mathf.Round(index * azimuth_coef);
                byte azimuth_packet_u = (byte)((azimuth << 8) >> 8);
                byte azimuth_packet_d = (byte)(azimuth >> 8);

                for (int i = 0; i < 1200; i += 100)
                {
                    int index_i = index + i;

                    // Header
                    packets[index_i++] = 0xff;
                    packets[index_i++] = 0xee;

                    // Azimuth Data
                    packets[index_i++] = azimuth_packet_u;
                    packets[index_i++] = azimuth_packet_d;

                    for(int j = 0; j < 16; j++)
                    {
                        int index_j = index_i + j * 3;
                        int index_d = index_i + (j / 2) + (j % 2) * 8;
                        
                        // Distance
                        ushort distance = (ushort)Mathf.Round((distances[index_d] * 1e+3f) / 2.0f);
                        packets[index_j++] = (byte)((distance << 8) >> 8);
                        packets[index_j++] = (byte)(distance >> 8);

                        // Intensity
                        packets[index_j] = (byte)(intensities[index_d]);
                    }
                }
                // Time Stamp
                var time = (uint)Math.Truncate(timeStamp * 1e+6f);
                packets[index + 1200] = (byte)((time << 24) >> 24);
                packets[index + 1201] = (byte)((time << 16) >> 24);
                packets[index + 1202] = (byte)((time << 8) >> 24);
                packets[index + 1203] = (byte)(time >> 24);

                // Footer
                packets[index + 1204] = 0x37;
                packets[index + 1205] = 0x22;
            }
        }
    }
    */
}
