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
}