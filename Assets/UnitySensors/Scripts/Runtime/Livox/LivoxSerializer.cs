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
    [RequireComponent(typeof(CSVLidar))]
    public class LivoxSerializer : MonoBehaviour
    {
        public UpdateLivoxPackets job;

        public UInt64 timebase;

        private NativeArray<uint> _offset_time;
        private NativeArray<float> _x;
        private NativeArray<float> _y;
        private NativeArray<float> _z;

        private NativeArray<byte> _reflectivity;
        private NativeArray<byte> _tag;
        private NativeArray<byte> _line;

        private NativeArray<byte> _data;

        public NativeArray<float> x { get => this._x; }
        public NativeArray<float> y { get => this._y; }
        public NativeArray<float> z { get => this._z; }
        public NativeArray<byte> reflectivity { get => this._reflectivity; }
        public NativeArray<byte> tag { get => this._tag; }
        public NativeArray<byte> line { get => this._line; }

        public NativeArray<byte> data { get => this._data; }

        public LivoxSerializer(int numOfLasersPerScan)
        {
            this._offset_time = new NativeArray<uint>(numOfLasersPerScan, Allocator.Persistent);
            this._x = new NativeArray<float>(numOfLasersPerScan, Allocator.Persistent);
            this._y = new NativeArray<float>(numOfLasersPerScan, Allocator.Persistent);
            this._z = new NativeArray<float>(numOfLasersPerScan, Allocator.Persistent);
            this._reflectivity = new NativeArray<byte>(numOfLasersPerScan, Allocator.Persistent);
            this._tag = new NativeArray<byte>(numOfLasersPerScan, Allocator.Persistent);
            this._line = new NativeArray<byte>(numOfLasersPerScan, Allocator.Persistent);

            this._data = new NativeArray<byte>(numOfLasersPerScan * 12, Allocator.Persistent);

            this.job = new UpdateLivoxPackets();
            this.job.numOfLasersPerScan = numOfLasersPerScan;
            this.job.timebase = 0;
            this.job.time = 0;

            this.job.offset_time = this._offset_time;
            this.job.x = this._x;
            this.job.y = this._y;
            this.job.z = this._z;
            this.job.reflectivity = this._reflectivity;
            this.job.tag = this._tag;
            this.job.line = this._line;

            this.job.data = this._data;

            this.job.tmp = new NativeArray<float>(numOfLasersPerScan * 3, Allocator.Persistent);
        }

        public void Dispose()
        {
            this._offset_time.Dispose();
            this._x.Dispose();
            this._y.Dispose();
            this._z.Dispose();
            this._reflectivity.Dispose();
            this._tag.Dispose();
            this._line.Dispose();
            this._data.Dispose();
            this.job.tmp.Dispose();
        }

        [BurstCompile]
        public struct UpdateLivoxPackets : IJob
        {
            [ReadOnly] public int numOfLasersPerScan;
            [ReadOnly] public UInt64 timebase;
            [ReadOnly] public UInt64 time;

            [ReadOnly] public NativeArray<Vector3> point;
            [ReadOnly] public NativeArray<byte> intensities;

            public NativeArray<uint> offset_time;
            public NativeArray<float> x;
            public NativeArray<float> y;
            public NativeArray<float> z;
            public NativeArray<byte> reflectivity;
            public NativeArray<byte> tag;
            public NativeArray<byte> line;

            public NativeArray<byte> data;

            public NativeArray<float> tmp;

            void IJob.Execute()
            {
                for(int index = 0; index < numOfLasersPerScan; index++)
                {
                    offset_time[index] = (uint)(time - timebase);
                    x[index] = point[index].x;
                    y[index] = point[index].z;
                    z[index] = point[index].y;
                    reflectivity[index] = intensities[index];
                    tag[index] = 0;
                    line[index] = 0;
                    tmp[index * 3]     = point[index].x;
                    tmp[index * 3 + 1] = point[index].z;
                    tmp[index * 3 + 2] = point[index].y;
                }
                var slice = new NativeSlice<float>(tmp).SliceConvert<byte>();
                slice.CopyTo(data);
            }
        }
    }
}