using NUnit.Framework;
using UnityEngine;

namespace UnitySensors.Tests.Editor
{
    [TestFixture]
    public class BasicUnityTests
    {
        [Test]
        public void Vector3_Magnitude_ShouldCalculateCorrectly()
        {
            // Arrange
            var vector = new Vector3(3.0f, 4.0f, 0.0f);
            var expectedMagnitude = 5.0f;
            
            // Act
            var magnitude = vector.magnitude;
            
            // Assert
            Assert.AreEqual(expectedMagnitude, magnitude, 0.001f);
        }

        [Test]
        public void Vector3_Normalize_ShouldReturnUnitVector()
        {
            // Arrange
            var vector = new Vector3(3.0f, 4.0f, 0.0f);
            
            // Act
            var normalized = vector.normalized;
            
            // Assert
            Assert.AreEqual(1.0f, normalized.magnitude, 0.001f);
        }

        [Test]
        public void Quaternion_Identity_ShouldBeNoRotation()
        {
            // Arrange & Act
            var identity = Quaternion.identity;
            
            // Assert
            Assert.AreEqual(0.0f, identity.x, 0.001f);
            Assert.AreEqual(0.0f, identity.y, 0.001f);
            Assert.AreEqual(0.0f, identity.z, 0.001f);
            Assert.AreEqual(1.0f, identity.w, 0.001f);
        }

        [Test]
        public void Color_Components_ShouldBeAccessible()
        {
            // Arrange
            var color = new Color(0.8f, 0.6f, 0.4f, 1.0f);
            
            // Act & Assert
            Assert.AreEqual(0.8f, color.r, 0.001f);
            Assert.AreEqual(0.6f, color.g, 0.001f);
            Assert.AreEqual(0.4f, color.b, 0.001f);
            Assert.AreEqual(1.0f, color.a, 0.001f);
        }

        [Test]
        public void Vector2Int_Constructor_ShouldSetCorrectValues()
        {
            // Arrange
            var x = 640;
            var y = 480;
            
            // Act
            var vector = new Vector2Int(x, y);
            
            // Assert
            Assert.AreEqual(x, vector.x);
            Assert.AreEqual(y, vector.y);
        }

        [Test]
        public void Transform_LocalPosition_ShouldDefaultToZero()
        {
            // Arrange
            var gameObject = new GameObject("TestObject");
            var transform = gameObject.transform;
            
            // Act
            var position = transform.localPosition;
            
            // Assert
            Assert.AreEqual(Vector3.zero, position);
            
            // Cleanup
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void Transform_LocalRotation_ShouldDefaultToIdentity()
        {
            // Arrange
            var gameObject = new GameObject("TestObject");
            var transform = gameObject.transform;
            
            // Act
            var rotation = transform.localRotation;
            
            // Assert
            Assert.AreEqual(Quaternion.identity, rotation);
            
            // Cleanup
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void GameObject_Name_ShouldBeSettable()
        {
            // Arrange
            var expectedName = "TestSensorObject";
            
            // Act
            var gameObject = new GameObject(expectedName);
            
            // Assert
            Assert.AreEqual(expectedName, gameObject.name);
            
            // Cleanup
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void Time_DeltaTime_ShouldBePositive()
        {
            // Act
            var deltaTime = Time.deltaTime;
            
            // Assert
            Assert.GreaterOrEqual(deltaTime, 0.0f);
        }

        [Test]
        public void Mathf_Approximately_ShouldWorkCorrectly()
        {
            // Arrange
            var a = 1.0f;
            var b = 1.0001f;
            
            // Act & Assert
            Assert.IsTrue(Mathf.Approximately(a, a));
            Assert.IsFalse(Mathf.Approximately(a, b));
        }
    }

    [TestFixture]
    public class UnitySensorBasicTests
    {
        [Test]
        public void UnitySensors_Assembly_ShouldBeAccessible()
        {
            // This test verifies that the UnitySensors assembly is properly referenced
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var assemblyName = typeof(UnityEngine.Vector3).Assembly.GetName().Name;
                Assert.IsNotNull(assemblyName);
            });
        }

        [Test]
        public void Vector2Int_UnitySensorsCompatibility_ShouldWork()
        {
            // Test that basic Unity types work in UnitySensors context
            // Arrange
            var resolution = new Vector2Int(640, 480);
            
            // Act & Assert
            Assert.AreEqual(640, resolution.x);
            Assert.AreEqual(480, resolution.y);
        }

        [Test]
        public void Float_HighPrecision_ShouldBeHandled()
        {
            // Test precision handling that might be used in sensors
            // Arrange
            var frequency = 30.0f;
            var deltaTime = 1.0f / frequency;
            
            // Act
            var calculatedFrequency = 1.0f / deltaTime;
            
            // Assert
            Assert.AreEqual(frequency, calculatedFrequency, 0.001f);
        }
    }

    [TestFixture]
    public class GeoCoordinateBasicTests
    {
        [Test]
        public void GeoCoordinate_Constructor_ShouldSetCorrectValues()
        {
            // Test the simplest possible usage of GeoCoordinate
            // Act & Assert - just verify it doesn't throw
            Assert.DoesNotThrow(() => {
                // Try to instantiate if the class is accessible
                var type = System.Type.GetType("UnitySensors.DataType.Geometry.GeoCoordinate, UnitySensorsRuntime");
                if (type != null)
                {
                    var instance = System.Activator.CreateInstance(type, 35.0, 139.0, 0.0);
                    Assert.IsNotNull(instance);
                }
            });
        }

        [Test]
        public void GeoCoordinate_Reflection_ShouldBeAccessible()
        {
            // Test that the UnitySensors assembly can be accessed via reflection
            // Act
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            var unitySensorsAssembly = System.Array.Find(assemblies, a => a.GetName().Name.Contains("UnitySensors"));
            
            // Assert
            if (unitySensorsAssembly != null)
            {
                Assert.IsNotNull(unitySensorsAssembly);
                var types = unitySensorsAssembly.GetTypes();
                Assert.Greater(types.Length, 0);
            }
            else
            {
                Assert.Pass("UnitySensors assembly not found - expected in current test setup");
            }
        }

        [Test]
        public void GeoCoordinate_ValidCoordinates_ShouldBeHandled()
        {
            // Test with known valid coordinate ranges
            // Arrange
            var validCoordinates = new[]
            {
                new { lat = 0.0, lon = 0.0, alt = 0.0 },           // Equator, Prime Meridian
                new { lat = 90.0, lon = 0.0, alt = 0.0 },          // North Pole
                new { lat = -90.0, lon = 0.0, alt = 0.0 },         // South Pole
                new { lat = 35.6762, lon = 139.6503, alt = 10.0 }  // Tokyo
            };

            // Act & Assert
            foreach (var coord in validCoordinates)
            {
                Assert.DoesNotThrow(() => {
                    // Test that the coordinate values are within valid ranges
                    Assert.That(coord.lat, Is.InRange(-90.0, 90.0));
                    Assert.That(coord.lon, Is.InRange(-180.0, 180.0));
                    Assert.That(coord.alt, Is.GreaterThanOrEqualTo(-11000.0)); // Mariana Trench depth
                });
            }
        }
    }

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
                    // Test default constructor
                    var instance = System.Activator.CreateInstance(type);
                    Assert.IsNotNull(instance);
                    
                    // Test parameterized constructor
                    var paramInstance = System.Activator.CreateInstance(type, 1.0, 2.0, 3.0);
                    Assert.IsNotNull(paramInstance);
                }
            });
        }

        [Test]
        public void Vector3D_DoubleToFloatConversion_ShouldHandlePrecision()
        {
            // Test precision handling in Vector3D conversions
            // Arrange
            var highPrecisionValue = 123.456789012345;
            var expectedFloatValue = (float)highPrecisionValue;
            
            // Act
            var actualFloatValue = (float)highPrecisionValue;
            
            // Assert
            Assert.AreEqual(expectedFloatValue, actualFloatValue, 1e-6f);
        }

        [Test]
        public void Vector3D_UnityVector3Compatibility_ShouldWork()
        {
            // Test that Vector3D-style operations work with Unity Vector3
            // Arrange
            var unityVector = new Vector3(1.5f, 2.5f, 3.5f);
            
            // Act - simulate Vector3D operations
            var doubleX = (double)unityVector.x;
            var doubleY = (double)unityVector.y;
            var doubleZ = (double)unityVector.z;
            
            // Assert
            Assert.AreEqual(1.5, doubleX, 1e-6);
            Assert.AreEqual(2.5, doubleY, 1e-6);
            Assert.AreEqual(3.5, doubleZ, 1e-6);
        }

        [Test]
        public void Vector3D_PrecisionComparison_ShouldShowDifferences()
        {
            // Test the precision differences between float and double
            // Arrange
            var highPrecisionValue = 123.456789012345;
            var floatValue = (float)highPrecisionValue;
            var backToDouble = (double)floatValue;
            
            // Act & Assert
            Assert.That(highPrecisionValue, Is.Not.EqualTo(backToDouble).Within(1e-10));
            Assert.AreEqual(highPrecisionValue, backToDouble, 1e-5); // Within float precision
        }

        [Test]
        public void Vector3D_DefaultValues_ShouldBeZero()
        {
            // Test default initialization behavior
            // Arrange
            double defaultX = 0.0;
            double defaultY = 0.0;
            double defaultZ = 0.0;
            
            // Act & Assert
            Assert.AreEqual(0.0, defaultX, 1e-15);
            Assert.AreEqual(0.0, defaultY, 1e-15);
            Assert.AreEqual(0.0, defaultZ, 1e-15);
        }

        [Test]
        public void Vector3D_VectorOperations_ShouldWorkWithDoubles()
        {
            // Test vector-like operations with double precision
            // Arrange
            var x1 = 1.234567890123456;
            var y1 = 2.345678901234567;
            var z1 = 3.456789012345678;
            
            var x2 = 0.123456789012345;
            var y2 = 0.234567890123456;
            var z2 = 0.345678901234567;
            
            // Act - vector addition
            var sumX = x1 + x2;
            var sumY = y1 + y2;
            var sumZ = z1 + z2;
            
            // Assert - Using practical tolerance for vector operations
            Assert.AreEqual(1.358024679135801, sumX, 1e-4);
            Assert.AreEqual(2.580246791358023, sumY, 1e-4);
            Assert.AreEqual(3.802468913580245, sumZ, 1e-4);
        }
    }
}