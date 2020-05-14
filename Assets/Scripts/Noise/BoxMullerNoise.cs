using System;

namespace SensorNoise
{
    public class BoxMullerNoise
    {
        private Random random;

        public BoxMullerNoise()
        {
            random = new Random(Environment.TickCount);
        }

        public BoxMullerNoise(int seed)
        {
            random = new Random(seed);
        }

        public double next(double mu = 0.0, double sigma = 1.0, bool getCos = true)
        {
            if (getCos)
            {
                double rand = 0.0;
                while ((rand = random.NextDouble()) == 0.0) ;
                double rand2 = random.NextDouble();
                double normrand = Math.Sqrt(-2.0 * Math.Log(rand)) * Math.Cos(2.0 * Math.PI * rand2);
                normrand = normrand * sigma + mu;
                return normrand;
            }
            else
            {
                double rand;
                while ((rand = random.NextDouble()) == 0.0) ;
                double rand2 = random.NextDouble();
                double normrand = Math.Sqrt(-2.0 * Math.Log(rand)) * Math.Sin(2.0 * Math.PI * rand2);
                normrand = normrand * sigma + mu;
                return normrand;
            }
        }

        public double[] nextPair(double mu = 0.0, double sigma = 1.0)
        {
            double[] normrand = new double[2];
            double rand = 0.0;
            while ((rand = random.NextDouble()) == 0.0) ;
            double rand2 = random.NextDouble();
            normrand[0] = Math.Sqrt(-2.0 * Math.Log(rand)) * Math.Cos(2.0 * Math.PI * rand2);
            normrand[0] = normrand[0] * sigma + mu;
            normrand[1] = Math.Sqrt(-2.0 * Math.Log(rand)) * Math.Sin(2.0 * Math.PI * rand2);
            normrand[1] = normrand[1] * sigma + mu;
            return normrand;
        }
    }
}