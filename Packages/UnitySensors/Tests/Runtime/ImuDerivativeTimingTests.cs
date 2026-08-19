using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnitySensors.Sensor.IMU;

namespace UnitySensors.Tests.Runtime
{
    /// <summary>
    /// Regression tests for Field-Robotics-Japan/UnitySensors#156: the IMU's
    /// velocity and angular velocity must match the true motion even when the
    /// sensor's samples are spaced differently from the frame time. Before
    /// the fix the derivatives were divided by one frame's Time.deltaTime
    /// while the pose deltas spanned the whole sensor period, so with the
    /// 4 Hz sensor below running at test-runner frame rates the readings
    /// came out several times too large.
    /// </summary>
    [TestFixture]
    public class ImuDerivativeTimingTests
    {
        private const float kSensorHz = 4.0f;
        private const float kRunSeconds = 2.5f;

        private GameObject _go;

        [TearDown]
        public void TearDown()
        {
            if (_go != null)
                Object.DestroyImmediate(_go);
        }

        private IMUSensor CreateSensor()
        {
            _go = new GameObject("imu_under_test");
            var sensor = _go.AddComponent<IMUSensor>();
            // Applied before Start() runs (next frame), so the scheduler
            // paces the sensor well below the frame rate and every sample
            // spans several frames -- the condition of issue #156.
            sensor.frequency = kSensorHz;
            return sensor;
        }

        private static float Median(List<float> values)
        {
            values.Sort();
            return values[values.Count / 2];
        }

        [UnityTest]
        public IEnumerator AngularVelocity_MatchesConstantRotationRate()
        {
            const float rateDeg = 30.0f;
            IMUSensor sensor = CreateSensor();
            yield return null;  // let Start()/Init() run

            var samples = new List<float>();
            int updates = 0;
            float lastUpdateTime = sensor.time;
            float t0 = Time.time;
            while (Time.time - t0 < kRunSeconds)
            {
                _go.transform.Rotate(Vector3.up, rateDeg * Time.deltaTime, Space.World);
                yield return null;
                if (sensor.time != lastUpdateTime)
                {
                    lastUpdateTime = sensor.time;
                    updates++;
                    if (updates > 2)  // the first samples still contain start-up state
                        samples.Add(sensor.angularVelocity.magnitude);
                }
            }

            Assert.GreaterOrEqual(samples.Count, 3, "the sensor produced too few updates");
            float expected = rateDeg * Mathf.Deg2Rad;
            // Before the fix the reading scales with (sample spacing / frame
            // time), which is far outside this band at any realistic frame
            // rate; after it the median sits on the true rate.
            Assert.That(Median(samples), Is.InRange(expected * 0.7f, expected * 1.3f),
                "angular velocity must match the true constant rotation rate");
        }

        [UnityTest]
        public IEnumerator Velocity_MatchesConstantSpeed()
        {
            const float speed = 0.8f;  // [m/s]
            IMUSensor sensor = CreateSensor();
            yield return null;  // let Start()/Init() run

            var samples = new List<float>();
            int updates = 0;
            float lastUpdateTime = sensor.time;
            float t0 = Time.time;
            while (Time.time - t0 < kRunSeconds)
            {
                _go.transform.position += Vector3.right * (speed * Time.deltaTime);
                yield return null;
                if (sensor.time != lastUpdateTime)
                {
                    lastUpdateTime = sensor.time;
                    updates++;
                    if (updates > 2)
                        samples.Add(sensor.velocity.magnitude);
                }
            }

            Assert.GreaterOrEqual(samples.Count, 3, "the sensor produced too few updates");
            Assert.That(Median(samples), Is.InRange(speed * 0.7f, speed * 1.3f),
                "velocity must match the true constant speed");
        }
    }
}
