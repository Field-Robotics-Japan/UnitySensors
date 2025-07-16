
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace UnitySensors.ROS.Serializer.Image
{
    [BurstCompile]
    struct ImageEncodeJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Color> sourceTextureRawDataColorRGBAF;
        [ReadOnly]
        public NativeArray<ColorRGBA32> sourceTextureRawDataColorRGBA32;
        [WriteOnly]
        public NativeArray<byte> targetTextureRawData;
        [ReadOnly]
        public int width;
        [ReadOnly]
        public int height;
        [ReadOnly]
        public float distanceFactor;
        [ReadOnly]
        public Encoding encoding;
        [ReadOnly]
        public int bytesPerPixel;
        public void Execute(int index)
        {
            int i = index % width;
            int j = index / width;
            int targetIndex = index * bytesPerPixel;

            var sourceColorRGBAF = encoding != Encoding._RGB8 ? sourceTextureRawDataColorRGBAF[(height - j - 1) * width + i] : Color.white;

            switch (encoding)
            {
                case Encoding._32FC1:
                    var targetColor32FC1 = new Color32FC1();
                    targetColor32FC1.r = sourceColorRGBAF.r * distanceFactor;
                    targetTextureRawData.ReinterpretStore(targetIndex, targetColor32FC1);
                    break;
                case Encoding._16UC1:
                    var targetColor16UC1 = new Color16UC1();
                    targetColor16UC1.r = (ushort)(sourceColorRGBAF.r * distanceFactor);
                    targetTextureRawData.ReinterpretStore(targetIndex, targetColor16UC1);
                    break;
                case Encoding._RGB8:
                default:
                    var sourceColorRGBA32 = sourceTextureRawDataColorRGBA32[(height - j - 1) * width + i];
                    var targetColorRGB8 = new ColorRGB8();
                    targetColorRGB8.r = sourceColorRGBA32.r;
                    targetColorRGB8.g = sourceColorRGBA32.g;
                    targetColorRGB8.b = sourceColorRGBA32.b;
                    targetTextureRawData.ReinterpretStore(targetIndex, targetColorRGB8);
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
    struct ColorRGBA32
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;
    }
    enum Encoding
    {
        _RGB8,
        _32FC1,
        _16UC1
    }
}