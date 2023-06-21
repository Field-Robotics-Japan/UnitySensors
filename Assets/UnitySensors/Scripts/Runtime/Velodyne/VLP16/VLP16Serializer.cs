using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;

namespace FRJ.Sensor
{
    [RequireComponent(typeof(RotateLidar))]
    public class VLP16Serializer
    {
        public updateVLP16Packets job;
        private NativeArray<byte> _packets;

        public NativeArray<byte> packets { get => this._packets; }
    
        public VLP16Serializer(int numOfLayers,
                               int numOfIncrements,
                               float minAzimuthAngle,
                               float maxAzimuthAngle)
        {
            this._packets = new NativeArray<byte>(numOfIncrements/12*1260, Allocator. Persistent);

            this.job = new updateVLP16Packets();
            this.job.numOfLayers     = numOfLayers;
            this.job.numOfIncrements = numOfIncrements;
            this.job.minAzimuthAngle = minAzimuthAngle;
            this.job.maxAzimuthAngle = maxAzimuthAngle;
            this.job.packets         = _packets;
            this.job.timeStamp       = 0f;
        }

        public void Dispose()
        {
            _packets.Dispose();
        }
    
        [BurstCompile]
        public struct updateVLP16Packets : IJob
        {
            [ReadOnly] public int numOfLayers;
            [ReadOnly] public int numOfIncrements;
            [ReadOnly] public float minAzimuthAngle;
            [ReadOnly] public float maxAzimuthAngle;

            [ReadOnly] public float timeStamp;    
            [ReadOnly] public NativeArray<float> distances;
            [ReadOnly] public NativeArray<float> intensities;

            public NativeArray<byte> packets;

            void IJob.Execute()
            {
                float azimuthIncAngle;
                if(numOfIncrements == 1)
                    azimuthIncAngle = 0;
                else
                    azimuthIncAngle =
                        (maxAzimuthAngle-minAzimuthAngle)/(float)numOfIncrements;

                for(int index=0; index<numOfIncrements/12; index++){      
                    int azIdx = index;
                    int stIdx = index * 1206;
                    int dbIdx = 0;
                    int distIdx = 0;

                    for(int db=0; db<12; db++)
                    {
                        dbIdx = db*100;
                        //Debug.Log(stIdx + dbIdx)
                        packets[stIdx + dbIdx] = 0xff;
                        packets[stIdx + dbIdx+1] = 0xee;
        
        
                        // write Azimuth data
                        ushort azimuth = (ushort)Math.Round(
                                                            (float)index * 12f * azimuthIncAngle * 100f );
                        packets[stIdx + dbIdx+2] = (byte)((azimuth << 8) >> 8);
                        packets[stIdx + dbIdx+2+1] = (byte)(azimuth >> 8);

                        // write distances and intensities data
                        distIdx = index*12*numOfLayers;
                        for(int i=0; i<16; i++)
                        {
                            // distance
                            var distance = (ushort)Math.Round(
                                                              (distances[distIdx+(i%2)*8+(i/2)] * 1e+3f) / 2f );
                            packets[stIdx + dbIdx+4 + i*3] = (byte)((distance << 8) >> 8);
                            packets[stIdx + dbIdx+4 + i*3+1] = (byte)(distance >> 8);
                            // intensities
                            var intensity = (byte)(intensities[distIdx+(i%2)*8+(i/2)]);
                            packets[stIdx + dbIdx+4 + i*3+2] = intensity;
                        }
                    }
                    // write time stamp
                    var time = (uint)Math.Truncate(timeStamp*1e+6f);
                    packets[stIdx + 1200] = (byte)((time << 24) >> 24);
                    packets[stIdx + 1201] = (byte)((time << 16) >> 24);
                    packets[stIdx + 1202] = (byte)((time << 8) >> 24);
                    packets[stIdx + 1203] = (byte)(time >> 24);

                    // write footer
                    packets[stIdx + 1204] = 0x37;
                    packets[stIdx + 1205] = 0x22;      
                }
            }
        }
    }
}
