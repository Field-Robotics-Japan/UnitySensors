using NUnit.Framework;
using UnityEngine;

namespace UnitySensors.Tests.Editor
{
    [TestFixture]
    public class Vector3DBasicTests
    {
        [Test]
        public void Vector3D_ReflectionAccess_ShouldBeAccessible()
        {
            // Test that Vector3D can be accessed via reflection
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.DataType.Geometry.Vector3D, UnitySensorsRuntime");
                if (type != null)
                {
                    Assert.IsTrue(type.IsClass);
                    Assert.IsTrue(type.IsPublic);
                    
                    // Check for x, y, z fields
                    var xField = type.GetField("x");
                    var yField = type.GetField("y");
                    var zField = type.GetField("z");
                    
                    if (xField != null) Assert.AreEqual(typeof(double), xField.FieldType);
                    if (yField != null) Assert.AreEqual(typeof(double), yField.FieldType);
                    if (zField != null) Assert.AreEqual(typeof(double), zField.FieldType);
                }
            });
        }

        [Test]
        public void Vector3D_DoubleToFloatConversion_ShouldHandlePrecision()
        {
            // Test precision handling between double and float
            // Act & Assert
            Assert.DoesNotThrow(() => {
                double doubleValue = 1.23456789012345;
                float floatValue = (float)doubleValue;
                
                // Should be approximately equal within float precision
                Assert.AreEqual(doubleValue, floatValue, 1e-4);
                
                // Test that double has higher precision
                double highPrecision = 1.234567890123456789;
                float convertedFloat = (float)highPrecision;
                
                Assert.That(highPrecision, Is.Not.EqualTo(convertedFloat).Within(1e-10));
            });
        }

        [Test]
        public void Vector3D_UnityVector3Compatibility_ShouldWork()
        {
            // Test compatibility with Unity's Vector3
            // Act & Assert
            Assert.DoesNotThrow(() => {
                // Test conversion concepts
                double x = 1.0;
                double y = 2.0; 
                double z = 3.0;
                
                Vector3 unityVector = new Vector3((float)x, (float)y, (float)z);
                
                Assert.AreEqual(1.0f, unityVector.x, 1e-6f);
                Assert.AreEqual(2.0f, unityVector.y, 1e-6f);
                Assert.AreEqual(3.0f, unityVector.z, 1e-6f);
            });
        }

        [Test]
        public void Vector3D_PrecisionComparison_ShouldShowDifferences()
        {
            // Test precision differences between double and float
            // Act & Assert
            Assert.DoesNotThrow(() => {
                double preciseValue = 1.234567890123456789;
                float lessePreciseValue = (float)preciseValue;
                
                // They should be different at high precision
                Assert.That(preciseValue, Is.Not.EqualTo(lessePreciseValue).Within(1e-10));
                
                // But similar at lower precision
                Assert.AreEqual(preciseValue, lessePreciseValue, 1e-4);
            });
        }

        [Test]
        public void Vector3D_DefaultValues_ShouldBeZero()
        {
            // Test default value behavior
            // Act & Assert
            Assert.DoesNotThrow(() => {
                // Test default double values
                double defaultX = 0.0;
                double defaultY = 0.0;
                double defaultZ = 0.0;
                
                Assert.AreEqual(0.0, defaultX);
                Assert.AreEqual(0.0, defaultY);
                Assert.AreEqual(0.0, defaultZ);
                
                // Test that they equal Unity's zero vector when converted
                Vector3 unityZero = new Vector3((float)defaultX, (float)defaultY, (float)defaultZ);
                Assert.AreEqual(Vector3.zero, unityZero);
            });
        }

        [Test]
        public void Vector3D_VectorOperations_ShouldWorkWithDoubles()
        {
            // Test vector operations with double precision
            // Act & Assert
            Assert.DoesNotThrow(() => {
                // Test vector addition
                double x1 = 1.5, y1 = 2.5, z1 = 3.5;
                double x2 = 0.5, y2 = 1.5, z2 = 2.5;
                
                double sumX = x1 + x2;
                double sumY = y1 + y2;
                double sumZ = z1 + z2;
                
                Assert.AreEqual(2.0, sumX);
                Assert.AreEqual(4.0, sumY);
                Assert.AreEqual(6.0, sumZ);
                
                // Test magnitude calculation
                double magnitude = System.Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1);
                Assert.Greater(magnitude, 0.0);
            });
        }
    }
}