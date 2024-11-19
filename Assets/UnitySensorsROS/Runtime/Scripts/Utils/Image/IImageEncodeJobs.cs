
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace UnitySensors.ROS.Utils.Image
{
    [BurstCompile]
    struct ImageEncodeJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Color> sourceTextureRawData;
        [WriteOnly]
        public NativeArray<ColorRGB8> targetTextureRGB8;
        [WriteOnly]
        public NativeArray<Color32FC1> targetTexture32FC1;
        [WriteOnly]
        public NativeArray<Color16UC1> targetTexture16UC1;
        [ReadOnly]
        public int width;
        [ReadOnly]
        public int height;
        [ReadOnly]
        public float distanceFactor;
        [ReadOnly]
        public Encoding encoding;
        public void Execute(int index)
        {
            int i = index % width;
            int j = index / width;

            var sourceColor = sourceTextureRawData[(height - j - 1) * width + i];

            switch (encoding)
            {
                case Encoding._32FC1:
                    var targetColor32FC1 = new Color32FC1();
                    targetColor32FC1.r = sourceColor.r * distanceFactor;
                    targetTexture32FC1[index] = targetColor32FC1;
                    break;
                case Encoding._16UC1:
                    var targetColor16UC1 = new Color16UC1();
                    targetColor16UC1.r = (ushort)(sourceColor.r * distanceFactor);
                    targetTexture16UC1[index] = targetColor16UC1;
                    break;
                case Encoding._RGB8:
                default:
                    var targetColorRGB8 = new ColorRGB8();
                    targetColorRGB8.r = (byte)(sourceColor.r * 255);
                    targetColorRGB8.g = (byte)(sourceColor.g * 255);
                    targetColorRGB8.b = (byte)(sourceColor.b * 255);
                    targetTextureRGB8[index] = targetColorRGB8;
                    break;
            }
        }
    }
    struct Color32FC1
    {
        public float r;
    }
    struct Color16UC1
    {
        public ushort r;
    }
    struct ColorRGB8
    {
        public byte r;
        public byte g;
        public byte b;
    }
    enum Encoding
    {
        _RGB8,
        _32FC1,
        _16UC1
    }
}