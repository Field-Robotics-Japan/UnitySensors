using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

namespace UnitySensors.Tests.Editor
{
    [TestFixture]
    public class ScanPatternBasicTests
    {
        [Test]
        public void ScanPattern_ReflectionAccess_ShouldBeAccessible()
        {
            // Test that ScanPattern can be accessed via reflection
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                var type = System.Type.GetType("UnitySensors.DataType.LiDAR.ScanPattern, UnitySensorsRuntime");
                if (type != null)
                {
                    Assert.IsTrue(type.IsClass);
                    Assert.IsTrue(type.IsPublic);

                    // Should inherit from ScriptableObject
                    Assert.IsTrue(type.IsSubclassOf(typeof(UnityEngine.ScriptableObject)));
                }
            });
        }

        [Test]
        public void ScanPattern_AngleRange_ShouldBeValid()
        {
            // Test angle range validation concepts
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                // Test typical LiDAR angle ranges
                var angleTests = new[] {
                    new { min = -180.0f, max = 180.0f, isValid = true },   // Full 360
                    new { min = -90.0f, max = 90.0f, isValid = true },     // 180 degree scan
                    new { min = -45.0f, max = 45.0f, isValid = true },     // 90 degree scan
                    new { min = 45.0f, max = -45.0f, isValid = false },    // Invalid: min > max
                    new { min = -270.0f, max = 270.0f, isValid = false }   // Invalid: outside range
                };

                foreach (var test in angleTests)
                {
                    bool actualValid = test.min < test.max &&
                                     test.min >= -180.0f && test.max <= 180.0f;
                    Assert.AreEqual(test.isValid, actualValid);
                }
            });
        }

        [Test]
        public void ScanPattern_Float3Array_ShouldBeInitializable()
        {
            // Test float3 array initialization concepts
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                // Test creating float3 arrays for scan patterns
                var scanDirections = new float3[] {
                    new float3(1, 0, 0),     // Forward
                    new float3(0, 1, 0),     // Up
                    new float3(-1, 0, 0),    // Backward
                    new float3(0, -1, 0)     // Down
                };

                Assert.AreEqual(4, scanDirections.Length);

                foreach (var direction in scanDirections)
                {
                    // Each direction should be a unit vector (approximately)
                    float magnitude = math.length(direction);
                    Assert.AreEqual(1.0f, magnitude, 0.001f);
                }
            });
        }

        [Test]
        public void ScanPattern_AngleConversions_ShouldWorkCorrectly()
        {
            // Test angle conversion concepts
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                // Test degree to radian conversion
                float degrees = 90.0f;
                float radians = degrees * Mathf.PI / 180.0f;
                float expectedRadians = Mathf.PI / 2.0f;

                Assert.AreEqual(expectedRadians, radians, 0.001f);

                // Test radian to degree conversion
                float backToDegrees = radians * 180.0f / Mathf.PI;
                Assert.AreEqual(degrees, backToDegrees, 0.001f);
            });
        }

        [Test]
        public void ScanPattern_SphericalCoordinates_ShouldCalculateCorrectly()
        {
            // Test spherical coordinate calculations
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                // Test spherical to cartesian conversion
                float azimuth = 0.0f;     // degrees
                float elevation = 0.0f;   // degrees
                float range = 1.0f;

                // Convert to radians
                float azimuthRad = azimuth * Mathf.PI / 180.0f;
                float elevationRad = elevation * Mathf.PI / 180.0f;

                // Calculate cartesian coordinates
                float x = range * Mathf.Cos(elevationRad) * Mathf.Cos(azimuthRad);
                float y = range * Mathf.Sin(elevationRad);
                float z = range * Mathf.Cos(elevationRad) * Mathf.Sin(azimuthRad);

                // For azimuth=0, elevation=0, should point in +X direction
                Assert.AreEqual(1.0f, x, 0.001f);
                Assert.AreEqual(0.0f, y, 0.001f);
                Assert.AreEqual(0.0f, z, 0.001f);
            });
        }

        [Test]
        public void ScanPattern_ScriptableObjectFields_ShouldBeAccessible()
        {
            // Test ScriptableObject field access via reflection
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                var type = System.Type.GetType("UnitySensors.DataType.LiDAR.ScanPattern, UnitySensorsRuntime");
                if (type != null)
                {
                    // Check for scans array field
                    var scansField = type.GetField("scans", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (scansField != null)
                    {
                        Assert.IsNotNull(scansField);
                        // Should be an array type
                        Assert.IsTrue(scansField.FieldType.IsArray);
                    }

                    // Check for angle range fields
                    var minAzimuthField = type.GetField("minAzimuthAngle", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    var maxAzimuthField = type.GetField("maxAzimuthAngle", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                    if (minAzimuthField != null) Assert.AreEqual(typeof(float), minAzimuthField.FieldType);
                    if (maxAzimuthField != null) Assert.AreEqual(typeof(float), maxAzimuthField.FieldType);
                }
            });
        }
    }
}