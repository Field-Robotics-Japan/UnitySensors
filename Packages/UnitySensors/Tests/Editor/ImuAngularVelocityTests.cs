using NUnit.Framework;
using UnityEngine;
using UnitySensors.Sensor.IMU;

namespace UnitySensors.Tests.Editor
{
    /// <summary>
    /// Regression tests for Field-Robotics-Japan/UnitySensors#155: the
    /// angular velocity must stay on the short arc even when consecutive
    /// rotation samples land on opposite hemispheres of the quaternion
    /// double cover (which happens every time the accumulated rotation
    /// passes a multiple of 2*pi).
    /// </summary>
    [TestFixture]
    public class ImuAngularVelocityTests
    {
        private const float kDt = 0.05f;

        private static void AssertRate(Vector3 actual, Vector3 expected, string message)
        {
            Assert.That((actual - expected).magnitude, Is.LessThan(1e-3f), message +
                $" (actual {actual}, expected {expected})");
        }

        [Test]
        public void SmallStep_ReportsTheRotationRate()
        {
            Quaternion previous = Quaternion.identity;
            Quaternion current = Quaternion.AngleAxis(2.0f, Vector3.up);

            Vector3 omega = IMUSensor.AngularVelocityBetween(previous, current, kDt);

            AssertRate(omega, Vector3.up * (2.0f * Mathf.Deg2Rad / kDt),
                "a plain small step must come out as angle / dt");
        }

        [Test]
        public void FullTurnBoundary_DoesNotSpike()
        {
            // Crossing 360 deg of accumulated rotation: AngleAxis(359) sits on
            // the w < 0 hemisphere, AngleAxis(1) ( = 361) on w > 0. Before the
            // fix the delta was read the long way around and the rate spiked
            // to ~(360 deg - step) / dt -- the exact symptom of issue #155.
            Quaternion previous = Quaternion.AngleAxis(359.0f, Vector3.up);
            Quaternion current = Quaternion.AngleAxis(1.0f, Vector3.up);

            Vector3 omega = IMUSensor.AngularVelocityBetween(previous, current, kDt);

            AssertRate(omega, Vector3.up * (2.0f * Mathf.Deg2Rad / kDt),
                "a 2 deg step across the full-turn boundary must stay a 2 deg step");
        }

        [Test]
        public void NegatedRepresentation_IsTheSameRotation()
        {
            // q and -q describe the same attitude; feeding the negated
            // representation must not change the measured rate.
            Quaternion previous = Quaternion.AngleAxis(10.0f, Vector3.up);
            Quaternion step = Quaternion.AngleAxis(12.0f, Vector3.up);
            Quaternion negated = new Quaternion(-step.x, -step.y, -step.z, -step.w);

            Vector3 fromPlain = IMUSensor.AngularVelocityBetween(previous, step, kDt);
            Vector3 fromNegated = IMUSensor.AngularVelocityBetween(previous, negated, kDt);

            AssertRate(fromNegated, fromPlain,
                "the negated quaternion is the same rotation and must give the same rate");
        }
    }
}
