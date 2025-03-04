using System;

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

using Random = Unity.Mathematics.Random;

namespace UnitySensors.Utils.Noise
{
    [BurstCompile]
    public struct IUpdateGaussianNoisesJob : IJobParallelFor
    {
        public float sigma;
        public Random random;
        public NativeArray<float> noises;

        public void Execute(int index)
        {
            var rand2 = random.NextFloat();
            var rand3 = random.NextFloat();
            float normrand =
                (float)Math.Sqrt(-2.0f * Math.Log(rand2)) *
                (float)Math.Cos(2.0f * Math.PI * rand3);
            noises[index] = sigma * normrand;
        }
    }
}
