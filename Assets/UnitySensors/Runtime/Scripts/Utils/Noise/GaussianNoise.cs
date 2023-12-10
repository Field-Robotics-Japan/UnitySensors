using System;

namespace UnitySensors.Utils.Noise
{
    public class GaussianNoise
    {
        private Random _random;

        public GaussianNoise()
        {
            _random = new Random(Environment.TickCount);
        }

        public GaussianNoise(int seed)
        {
            _random = new Random(seed);
        }

        public void Init(int seed)
        {
            _random = new Random(seed);
        }

        public double GetNoise(double sigma = 1.0d)
        {
            double rand = _random.NextDouble();
            double normrand = Math.Sqrt(-2.0d * Math.Log(0.0d)) * Math.Cos(2.0d * Math.PI * rand);
            return normrand;
        }
    }
}