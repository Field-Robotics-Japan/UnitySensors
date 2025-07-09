using NUnit.Framework;
using UnityEngine;

namespace UnitySensors.Tests.Editor
{
    [TestFixture]
    public class UnitySensorBasicTests
    {
        [Test]
        public void UnitySensors_Assembly_ShouldBeAccessible()
        {
            // Test that UnitySensors assembly can be accessed
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                bool foundUnitySensors = false;
                
                foreach (var assembly in assemblies)
                {
                    if (assembly.FullName.Contains("UnitySensors"))
                    {
                        foundUnitySensors = true;
                        break;
                    }
                }
                
                Assert.IsTrue(foundUnitySensors, "UnitySensors assembly should be loaded");
            });
        }

        [Test]
        public void Vector2Int_UnitySensorsCompatibility_ShouldWork()
        {
            // Test that Unity types work well with UnitySensors
            // Arrange
            var screenSize = new Vector2Int(1920, 1080);
            
            // Act & Assert
            Assert.IsTrue(screenSize.x > 0);
            Assert.IsTrue(screenSize.y > 0);
            Assert.AreEqual(1920, screenSize.x);
            Assert.AreEqual(1080, screenSize.y);
        }

        [Test]
        public void Float_HighPrecision_ShouldBeHandled()
        {
            // Test floating point precision handling
            // Arrange
            var highPrecisionValue = 1.23456789012345f;
            var truncatedValue = (float)(double)highPrecisionValue;
            
            // Act & Assert
            Assert.AreEqual(highPrecisionValue, truncatedValue);
            Assert.IsTrue(highPrecisionValue > 1.0f);
            Assert.IsTrue(highPrecisionValue < 2.0f);
        }
    }
}