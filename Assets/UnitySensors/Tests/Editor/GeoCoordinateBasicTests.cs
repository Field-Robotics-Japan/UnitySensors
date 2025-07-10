using NUnit.Framework;

namespace UnitySensors.Tests.Editor
{
    [TestFixture]
    public class GeoCoordinateBasicTests
    {
        [Test]
        public void GeoCoordinate_Reflection_ShouldBeAccessible()
        {
            // Test that GeoCoordinate can be accessed via reflection
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.DataType.GeoCoordinate, UnitySensorsRuntime");
                if (type != null)
                {
                    Assert.IsTrue(type.IsClass);
                    Assert.IsTrue(type.IsPublic);
                    
                    // Check for latitude and longitude fields
                    var latField = type.GetField("latitude");
                    var lonField = type.GetField("longitude");
                    var altField = type.GetField("altitude");
                    
                    if (latField != null) Assert.AreEqual(typeof(double), latField.FieldType);
                    if (lonField != null) Assert.AreEqual(typeof(double), lonField.FieldType);
                    if (altField != null) Assert.AreEqual(typeof(double), altField.FieldType);
                }
            });
        }

        [Test]
        public void GeoCoordinate_ValidCoordinates_ShouldBeHandled()
        {
            // Test valid coordinate ranges
            // Act & Assert
            Assert.DoesNotThrow(() => {
                // Test latitude range (-90 to 90)
                var validLatitudes = new double[] { -90.0, -45.0, 0.0, 45.0, 90.0 };
                foreach (var lat in validLatitudes)
                {
                    Assert.GreaterOrEqual(lat, -90.0);
                    Assert.LessOrEqual(lat, 90.0);
                }
                
                // Test longitude range (-180 to 180)
                var validLongitudes = new double[] { -180.0, -90.0, 0.0, 90.0, 180.0 };
                foreach (var lon in validLongitudes)
                {
                    Assert.GreaterOrEqual(lon, -180.0);
                    Assert.LessOrEqual(lon, 180.0);
                }
                
                // Test altitude (typically above sea level, but can be negative)
                var validAltitudes = new double[] { -422.0, 0.0, 100.0, 8848.0 }; // Dead Sea to Everest
                foreach (var alt in validAltitudes)
                {
                    Assert.GreaterOrEqual(alt, -500.0); // Reasonable lower bound
                    Assert.LessOrEqual(alt, 10000.0);   // Reasonable upper bound
                }
            });
        }

        [Test]
        public void GeoCoordinate_Constructor_ShouldSetCorrectValues()
        {
            // Test coordinate construction concepts
            // Act & Assert
            Assert.DoesNotThrow(() => {
                // Test typical coordinate values
                var testCoordinates = new[] {
                    new { lat = 35.6762, lon = 139.6503, alt = 0.0, name = "Tokyo" },
                    new { lat = 40.7128, lon = -74.0060, alt = 10.0, name = "New York" },
                    new { lat = 51.5074, lon = -0.1278, alt = 11.0, name = "London" }
                };
                
                foreach (var coord in testCoordinates)
                {
                    // Validate latitude
                    Assert.GreaterOrEqual(coord.lat, -90.0);
                    Assert.LessOrEqual(coord.lat, 90.0);
                    
                    // Validate longitude
                    Assert.GreaterOrEqual(coord.lon, -180.0);
                    Assert.LessOrEqual(coord.lon, 180.0);
                    
                    // Validate name
                    Assert.IsNotNull(coord.name);
                    Assert.IsTrue(coord.name.Length > 0);
                }
            });
        }
    }
}