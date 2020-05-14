using System;

namespace SensorNoise
{
    public class BiasNoise
    {
        public BiasNoise()
        {
        }

        public BiasNoise(int seed)
        {
        }

        public double apply(double mu, double bias = 0.0)
        {
            return mu + bias;
        }
    }
}