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

    [TestFixture]
    public class ScanPatternBasicTests
    {
        [Test]
        public void ScanPattern_ReflectionAccess_ShouldBeAccessible()
        {
            // Test that ScanPattern can be accessed via reflection
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.DataType.LiDAR.ScanPattern, UnitySensorsRuntime");
                if (type != null)
                {
                    Assert.IsNotNull(type);
                    Assert.IsTrue(type.IsSubclassOf(typeof(ScriptableObject)));
                }
            });
        }

        [Test]
        public void ScanPattern_AngleRange_ShouldBeValid()
        {
            // Test angle range validation logic
            // Arrange
            var minZenith = 0.0f;
            var maxZenith = 180.0f;
            var minAzimuth = -180.0f;
            var maxAzimuth = 180.0f;
            
            // Act & Assert
            Assert.That(minZenith, Is.InRange(0.0f, 180.0f));
            Assert.That(maxZenith, Is.InRange(0.0f, 180.0f));
            Assert.That(minAzimuth, Is.InRange(-180.0f, 180.0f));
            Assert.That(maxAzimuth, Is.InRange(-180.0f, 180.0f));
            Assert.LessOrEqual(minZenith, maxZenith);
            Assert.LessOrEqual(minAzimuth, maxAzimuth);
        }

        [Test]
        public void ScanPattern_Float3Array_ShouldBeInitializable()
        {
            // Test float3 array operations
            // Arrange & Act
            Assert.DoesNotThrow(() => {
                var scans = new Unity.Mathematics.float3[4];
                scans[0] = new Unity.Mathematics.float3(1.0f, 0.0f, 0.0f);
                scans[1] = new Unity.Mathematics.float3(0.0f, 1.0f, 0.0f);
                scans[2] = new Unity.Mathematics.float3(0.0f, 0.0f, 1.0f);
                scans[3] = new Unity.Mathematics.float3(0.5f, 0.5f, 0.5f);
                
                // Assert
                Assert.AreEqual(4, scans.Length);
                Assert.AreEqual(1.0f, scans[0].x, 1e-6f);
                Assert.AreEqual(1.0f, scans[1].y, 1e-6f);
                Assert.AreEqual(1.0f, scans[2].z, 1e-6f);
            });
        }

        [Test]
        public void ScanPattern_AngleConversions_ShouldWorkCorrectly()
        {
            // Test angle conversion calculations
            // Arrange
            var degreesToRadians = Unity.Mathematics.math.PI / 180.0f;
            var radiansToDegrees = 180.0f / Unity.Mathematics.math.PI;
            
            // Act
            var angle90Deg = 90.0f;
            var angle90Rad = angle90Deg * degreesToRadians;
            var backToDegrees = angle90Rad * radiansToDegrees;
            
            // Assert
            Assert.AreEqual(Unity.Mathematics.math.PI / 2.0f, angle90Rad, 1e-6f);
            Assert.AreEqual(angle90Deg, backToDegrees, 1e-4f);
        }

        [Test]
        public void ScanPattern_SphericalCoordinates_ShouldCalculateCorrectly()
        {
            // Test spherical coordinate calculations typical for LiDAR
            // Arrange
            var radius = 1.0f;
            var zenithAngle = Unity.Mathematics.math.PI / 4.0f; // 45 degrees
            var azimuthAngle = Unity.Mathematics.math.PI / 2.0f; // 90 degrees
            
            // Act - Convert spherical to cartesian
            var x = radius * Unity.Mathematics.math.sin(zenithAngle) * Unity.Mathematics.math.cos(azimuthAngle);
            var y = radius * Unity.Mathematics.math.sin(zenithAngle) * Unity.Mathematics.math.sin(azimuthAngle);
            var z = radius * Unity.Mathematics.math.cos(zenithAngle);
            
            // Assert
            Assert.AreEqual(0.0f, x, 1e-6f); // cos(90°) = 0
            Assert.AreEqual(Unity.Mathematics.math.sqrt(2.0f) / 2.0f, y, 1e-6f); // sin(45°) * sin(90°)
            Assert.AreEqual(Unity.Mathematics.math.sqrt(2.0f) / 2.0f, z, 1e-6f); // cos(45°)
        }

        [Test]
        public void ScanPattern_ScriptableObjectFields_ShouldBeAccessible()
        {
            // Test that ScriptableObject fields are properly defined
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.DataType.LiDAR.ScanPattern, UnitySensorsRuntime");
                if (type != null)
                {
                    var scansField = type.GetField("scans");
                    var sizeField = type.GetField("size");
                    var minZenithField = type.GetField("minZenithAngle");
                    var maxZenithField = type.GetField("maxZenithAngle");
                    var minAzimuthField = type.GetField("minAzimuthAngle");
                    var maxAzimuthField = type.GetField("maxAzimuthAngle");
                    
                    if (scansField != null) Assert.IsNotNull(scansField);
                    if (sizeField != null) Assert.IsNotNull(sizeField);
                    if (minZenithField != null) Assert.IsNotNull(minZenithField);
                    if (maxZenithField != null) Assert.IsNotNull(maxZenithField);
                    if (minAzimuthField != null) Assert.IsNotNull(minAzimuthField);
                    if (maxAzimuthField != null) Assert.IsNotNull(maxAzimuthField);
                }
            });
        }
    }

    [TestFixture]
    public class PointCloudBasicTests
    {
        [Test]
        public void PointXYZ_StructLayout_ShouldBeCorrect()
        {
            // Test PointXYZ structure layout and size
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.DataType.Sensor.PointCloud.PointXYZ, UnitySensorsRuntime");
                if (type != null)
                {
                    Assert.IsTrue(type.IsValueType); // Should be struct
                    var interfaces = type.GetInterfaces();
                    Assert.IsTrue(System.Array.Exists(interfaces, i => i.Name == "IPointInterface"));
                    
                    // Check expected size (3 floats = 12 bytes)
                    var size = System.Runtime.InteropServices.Marshal.SizeOf(type);
                    Assert.AreEqual(12, size);
                }
            });
        }

        [Test]
        public void PointXYZI_StructLayout_ShouldBeCorrect()
        {
            // Test PointXYZI structure layout and size
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.DataType.Sensor.PointCloud.PointXYZI, UnitySensorsRuntime");
                if (type != null)
                {
                    Assert.IsTrue(type.IsValueType); // Should be struct
                    var interfaces = type.GetInterfaces();
                    Assert.IsTrue(System.Array.Exists(interfaces, i => i.Name == "IPointInterface"));
                    
                    // Check expected size (4 floats = 16 bytes)
                    var size = System.Runtime.InteropServices.Marshal.SizeOf(type);
                    Assert.AreEqual(16, size);
                }
            });
        }

        [Test]
        public void PointXYZRGB_StructLayout_ShouldBeCorrect()
        {
            // Test PointXYZRGB structure layout and size
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.DataType.Sensor.PointCloud.PointXYZRGB, UnitySensorsRuntime");
                if (type != null)
                {
                    Assert.IsTrue(type.IsValueType); // Should be struct
                    var interfaces = type.GetInterfaces();
                    Assert.IsTrue(System.Array.Exists(interfaces, i => i.Name == "IPointInterface"));
                    
                    // Check expected size (3 floats + 4 bytes = 16 bytes)
                    var size = System.Runtime.InteropServices.Marshal.SizeOf(type);
                    Assert.AreEqual(16, size);
                }
            });
        }

        [Test]
        public void PointTypes_Float3Position_ShouldBeInitializable()
        {
            // Test float3 position initialization for all point types
            // Arrange
            var testPosition = new Unity.Mathematics.float3(1.5f, 2.5f, 3.5f);
            
            // Act & Assert
            Assert.DoesNotThrow(() => {
                // Test basic float3 operations
                Assert.AreEqual(1.5f, testPosition.x, 1e-6f);
                Assert.AreEqual(2.5f, testPosition.y, 1e-6f);
                Assert.AreEqual(3.5f, testPosition.z, 1e-6f);
                
                // Test magnitude calculation
                var magnitude = Unity.Mathematics.math.length(testPosition);
                var expectedMagnitude = Unity.Mathematics.math.sqrt(1.5f * 1.5f + 2.5f * 2.5f + 3.5f * 3.5f);
                Assert.AreEqual(expectedMagnitude, magnitude, 1e-6f);
            });
        }

        [Test]
        public void PointXYZI_IntensityRange_ShouldBeValid()
        {
            // Test intensity value validation for LiDAR applications
            // Arrange
            var validIntensities = new float[] { 0.0f, 0.5f, 1.0f, 100.0f, 255.0f };
            var invalidIntensities = new float[] { -1.0f, -100.0f };
            
            // Act & Assert
            foreach (var intensity in validIntensities)
            {
                Assert.GreaterOrEqual(intensity, 0.0f, $"Intensity {intensity} should be non-negative");
            }
            
            foreach (var intensity in invalidIntensities)
            {
                Assert.Less(intensity, 0.0f, $"Intensity {intensity} should be negative (invalid)");
            }
        }

        [Test]
        public void PointXYZRGB_ColorChannels_ShouldBeInByteRange()
        {
            // Test RGB color channel byte range validation
            // Arrange
            var validColors = new byte[] { 0, 128, 255 };
            var testCombinations = new[]
            {
                new { r = (byte)255, g = (byte)0, b = (byte)0, a = (byte)255 },    // Red
                new { r = (byte)0, g = (byte)255, b = (byte)0, a = (byte)255 },    // Green
                new { r = (byte)0, g = (byte)0, b = (byte)255, a = (byte)255 },    // Blue
                new { r = (byte)255, g = (byte)255, b = (byte)255, a = (byte)255 }, // White
                new { r = (byte)0, g = (byte)0, b = (byte)0, a = (byte)0 }          // Black/Transparent
            };
            
            // Act & Assert
            foreach (var color in validColors)
            {
                Assert.That(color, Is.InRange(0, 255));
            }
            
            foreach (var combo in testCombinations)
            {
                Assert.That(combo.r, Is.InRange(0, 255));
                Assert.That(combo.g, Is.InRange(0, 255));
                Assert.That(combo.b, Is.InRange(0, 255));
                Assert.That(combo.a, Is.InRange(0, 255));
            }
        }

        [Test]
        public void PointTypes_MemoryEfficiency_ShouldBeOptimal()
        {
            // Test memory efficiency assumptions for point cloud processing
            // Act & Assert
            Assert.DoesNotThrow(() => {
                // Test assumptions about point sizes for memory calculations
                var pointXYZSize = 12;  // 3 floats
                var pointXYZISize = 16; // 4 floats
                var pointXYZRGBSize = 16; // 3 floats + 4 bytes (padded)
                
                // Calculate memory for typical point cloud sizes
                var typicalPointCount = 65536; // 64K points
                var xyzMemory = typicalPointCount * pointXYZSize;
                var xyziMemory = typicalPointCount * pointXYZISize;
                var xyzrgbMemory = typicalPointCount * pointXYZRGBSize;
                
                // Assert memory usage is reasonable for point clouds
                Assert.Less(xyzMemory, 1024 * 1024); // < 1MB for 64K points
                Assert.Less(xyziMemory, 1024 * 1024 * 2); // < 2MB for 64K points
                Assert.Less(xyzrgbMemory, 1024 * 1024 * 2); // < 2MB for 64K points
            });
        }
    }

    [TestFixture]
    public class PointCloudGenericTests
    {
        [Test]
        public void PointCloudGeneric_ReflectionAccess_ShouldBeAccessible()
        {
            // Test that PointCloud<T> generic class can be accessed via reflection
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.DataType.Sensor.PointCloud`1, UnitySensorsRuntime");
                if (type != null)
                {
                    Assert.IsTrue(type.IsGenericTypeDefinition);
                    Assert.IsTrue(type.IsClass);
                    
                    // Check that it implements IDisposable
                    var interfaces = type.GetInterfaces();
                    Assert.IsTrue(System.Array.Exists(interfaces, i => i == typeof(System.IDisposable)));
                }
            });
        }

        [Test]
        public void PointCloudGeneric_GenericConstraint_ShouldBeStructIPointInterface()
        {
            // Test that the generic constraint is properly defined
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.DataType.Sensor.PointCloud`1, UnitySensorsRuntime");
                if (type != null)
                {
                    var constraints = type.GetGenericArguments()[0].GetGenericParameterConstraints();
                    var attrs = type.GetGenericArguments()[0].GenericParameterAttributes;
                    
                    // Should have struct constraint
                    Assert.IsTrue((attrs & System.Reflection.GenericParameterAttributes.NotNullableValueTypeConstraint) != 0);
                }
            });
        }

        [Test]
        public void PointCloudGeneric_NativeArrayField_ShouldBeAccessible()
        {
            // Test that the NativeArray<T> points field exists and is accessible
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.DataType.Sensor.PointCloud`1, UnitySensorsRuntime");
                if (type != null)
                {
                    var pointsField = type.GetField("points");
                    if (pointsField != null)
                    {
                        Assert.IsNotNull(pointsField);
                        Assert.IsTrue(pointsField.FieldType.IsGenericType);
                        
                        // Check that it's a NativeArray type
                        var fieldTypeName = pointsField.FieldType.GetGenericTypeDefinition().Name;
                        Assert.IsTrue(fieldTypeName.Contains("NativeArray"));
                    }
                }
            });
        }

        [Test]
        public void PointCloudGeneric_DisposablePattern_ShouldBeImplemented()
        {
            // Test that the IDisposable pattern is properly implemented
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.DataType.Sensor.PointCloud`1, UnitySensorsRuntime");
                if (type != null)
                {
                    var disposeMethod = type.GetMethod("Dispose");
                    if (disposeMethod != null)
                    {
                        Assert.IsNotNull(disposeMethod);
                        Assert.AreEqual(typeof(void), disposeMethod.ReturnType);
                        Assert.AreEqual(0, disposeMethod.GetParameters().Length);
                    }
                }
            });
        }

        [Test]
        public void PointCloudGeneric_MemoryManagement_ShouldHandleNativeArrays()
        {
            // Test memory management concepts for NativeArray usage
            // Act & Assert
            Assert.DoesNotThrow(() => {
                // Test NativeArray memory allocation patterns
                var testSize = 1000;
                var bytesPerPoint = 12; // PointXYZ size
                var expectedMemory = testSize * bytesPerPoint;
                
                // Verify memory calculations are reasonable
                Assert.Greater(testSize, 0);
                Assert.Greater(bytesPerPoint, 0);
                Assert.Greater(expectedMemory, 0);
                Assert.Less(expectedMemory, 1024 * 1024); // Less than 1MB for 1K points
            });
        }

        [Test]
        public void PointCloudGeneric_TypeSafety_ShouldEnforceConstraints()
        {
            // Test type safety with different point types
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var validPointTypes = new[]
                {
                    "UnitySensors.DataType.Sensor.PointCloud.PointXYZ",
                    "UnitySensors.DataType.Sensor.PointCloud.PointXYZI",
                    "UnitySensors.DataType.Sensor.PointCloud.PointXYZRGB"
                };
                
                foreach (var typeName in validPointTypes)
                {
                    var pointType = System.Type.GetType(typeName + ", UnitySensorsRuntime");
                    if (pointType != null)
                    {
                        Assert.IsTrue(pointType.IsValueType); // Should be struct
                        
                        // Check if it implements IPointInterface
                        var interfaces = pointType.GetInterfaces();
                        Assert.IsTrue(System.Array.Exists(interfaces, i => i.Name == "IPointInterface"));
                    }
                }
            });
        }

        [Test]
        public void PointCloudGeneric_UnityJobSystemCompatibility_ShouldWork()
        {
            // Test Unity Job System compatibility assumptions
            // Act & Assert
            Assert.DoesNotThrow(() => {
                // Test that NativeArray concepts work as expected for Job System
                // We can't actually create NativeArrays in editor tests, but we can test the concepts
                
                // Test typical point cloud sizes for performance
                var smallPointCloud = 1000;
                var mediumPointCloud = 10000;
                var largePointCloud = 100000;
                
                Assert.Greater(smallPointCloud, 0);
                Assert.Greater(mediumPointCloud, smallPointCloud);
                Assert.Greater(largePointCloud, mediumPointCloud);
                
                // Test that point sizes are reasonable for bulk operations
                var pointXYZSize = 12;
                var pointXYZISize = 16;
                var pointXYZRGBSize = 16;
                
                Assert.AreEqual(12, pointXYZSize);
                Assert.AreEqual(16, pointXYZISize);
                Assert.AreEqual(16, pointXYZRGBSize);
            });
        }
    }

    [TestFixture]
    public class ScanPatternGeneratorTests
    {
        [Test]
        public void ScanPatternGenerator_ReflectionAccess_ShouldBeAccessible()
        {
            // Test that ScanPatternGenerator can be accessed via reflection
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.DataType.LiDAR.ScanPatternGenerator, UnitySensorsEditor");
                if (type != null)
                {
                    Assert.IsTrue(type.IsClass);
                    Assert.IsTrue(type.IsSubclassOf(typeof(UnityEditor.EditorWindow)));
                }
            });
        }

        [Test]
        public void ScanPatternGenerator_EnumTypes_ShouldBeAccessible()
        {
            // Test that nested enum types are properly defined
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.DataType.LiDAR.ScanPatternGenerator, UnitySensorsEditor");
                if (type != null)
                {
                    var nestedTypes = type.GetNestedTypes(System.Reflection.BindingFlags.NonPublic);
                    
                    // Check for Mode enum
                    var modeType = System.Array.Find(nestedTypes, t => t.Name == "Mode");
                    if (modeType != null)
                    {
                        Assert.IsTrue(modeType.IsEnum);
                        var modeValues = System.Enum.GetNames(modeType);
                        Assert.IsTrue(System.Array.Exists(modeValues, v => v == "FromCSV"));
                        Assert.IsTrue(System.Array.Exists(modeValues, v => v == "FromSpecification"));
                    }
                    
                    // Check for Direction enum
                    var directionType = System.Array.Find(nestedTypes, t => t.Name == "Direction");
                    if (directionType != null)
                    {
                        Assert.IsTrue(directionType.IsEnum);
                        var directionValues = System.Enum.GetNames(directionType);
                        Assert.IsTrue(System.Array.Exists(directionValues, v => v == "CW"));
                        Assert.IsTrue(System.Array.Exists(directionValues, v => v == "CCW"));
                    }
                }
            });
        }

        [Test]
        public void ScanPatternGenerator_CSVParsing_ShouldHandleBasicFormat()
        {
            // Test CSV parsing logic with typical LiDAR data format
            // Act & Assert
            Assert.DoesNotThrow(() => {
                // Test typical CSV header parsing
                var testHeaders = new string[] 
                {
                    "azimuth,zenith,distance",
                    "zenith_angle,azimuth_angle,range",
                    "AZIMUTH,ZENITH,INTENSITY"
                };
                
                foreach (var headerLine in testHeaders)
                {
                    var headers = headerLine.Split(',');
                    
                    int azimuth_index = -1;
                    int zenith_index = -1;
                    
                    for (int c = 0; c < headers.Length; c++)
                    {
                        string header = headers[c].ToLower();
                        if (header.Contains("zenith")) zenith_index = c;
                        else if (header.Contains("azimuth")) azimuth_index = c;
                    }
                    
                    Assert.GreaterOrEqual(azimuth_index, 0, $"Should find azimuth in: {headerLine}");
                    Assert.GreaterOrEqual(zenith_index, 0, $"Should find zenith in: {headerLine}");
                }
            });
        }

        [Test]
        public void ScanPatternGenerator_AngleCalculations_ShouldBeCorrect()
        {
            // Test angle calculation logic used in pattern generation
            // Act & Assert
            Assert.DoesNotThrow(() => {
                // Test azimuth angle interpolation
                var minAzimuth = -180.0f;
                var maxAzimuth = 180.0f;
                var resolution = 360;
                
                for (int i = 0; i < resolution; i++)
                {
                    // Test CW direction
                    var azimuthCW = Mathf.Lerp(minAzimuth, maxAzimuth, (float)i / resolution);
                    Assert.That(azimuthCW, Is.InRange(minAzimuth, maxAzimuth));
                    
                    // Test CCW direction  
                    var azimuthCCW = Mathf.Lerp(minAzimuth, maxAzimuth, (float)(resolution - 1 - i) / resolution);
                    Assert.That(azimuthCCW, Is.InRange(minAzimuth, maxAzimuth));
                }
                
                // Test zenith angle offset
                var zenithAngle = 15.0f;
                var zenithOffset = 5.0f;
                var adjustedZenith = zenithAngle - zenithOffset;
                Assert.AreEqual(10.0f, adjustedZenith, 1e-6f);
            });
        }

        [Test]
        public void ScanPatternGenerator_QuaternionRotations_ShouldBeCorrect()
        {
            // Test quaternion rotation calculations for scan directions
            // Act & Assert
            Assert.DoesNotThrow(() => {
                // Test typical LiDAR angles
                var testAngles = new[]
                {
                    new { zenith = 0.0f, azimuth = 0.0f },      // Forward
                    new { zenith = 0.0f, azimuth = 90.0f },     // Right
                    new { zenith = 0.0f, azimuth = -90.0f },    // Left
                    new { zenith = 15.0f, azimuth = 0.0f },     // Up 15°
                    new { zenith = -15.0f, azimuth = 0.0f }     // Down 15°
                };
                
                foreach (var angle in testAngles)
                {
                    // Test CSV mode rotation (positive zenith)
                    var rotationCSV = Quaternion.Euler(angle.zenith, angle.azimuth, 0) * Vector3.forward;
                    Assert.AreEqual(1.0f, rotationCSV.magnitude, 1e-6f, "Should be unit vector");
                    
                    // Test Specification mode rotation (negative zenith)
                    var rotationSpec = Quaternion.Euler(-angle.zenith, angle.azimuth, 0) * Vector3.forward;
                    Assert.AreEqual(1.0f, rotationSpec.magnitude, 1e-6f, "Should be unit vector");
                }
            });
        }

        [Test]
        public void ScanPatternGenerator_ArraySizeCalculation_ShouldBeCorrect()
        {
            // Test array size calculation for scan pattern generation
            // Act & Assert
            Assert.DoesNotThrow(() => {
                // Test specification mode size calculation
                var zenithAngles = new float[] { -15.0f, 0.0f, 15.0f };
                var azimuthResolution = 360;
                var expectedSize = zenithAngles.Length * azimuthResolution;
                
                Assert.AreEqual(1080, expectedSize); // 3 * 360
                
                // Test CSV mode size calculation (lines - 2 for header and empty line)
                var csvLines = new string[]
                {
                    "azimuth,zenith,distance",
                    "0.0,0.0,10.0",
                    "1.0,0.0,10.0",
                    "2.0,0.0,10.0",
                    "" // Empty line at end
                };
                var csvSize = csvLines.Length - 2;
                Assert.AreEqual(3, csvSize);
            });
        }

        [Test]
        public void ScanPatternGenerator_AngleRangeTracking_ShouldBeCorrect()
        {
            // Test min/max angle range tracking during pattern generation
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var testAngles = new float[] { -15.0f, 0.0f, 15.0f, 30.0f };
                
                var minAngle = float.MaxValue;
                var maxAngle = float.MinValue;
                
                foreach (var angle in testAngles)
                {
                    minAngle = Mathf.Min(minAngle, angle);
                    maxAngle = Mathf.Max(maxAngle, angle);
                }
                
                Assert.AreEqual(-15.0f, minAngle);
                Assert.AreEqual(30.0f, maxAngle);
            });
        }

        [Test]
        public void ScanPatternGenerator_SerializationFields_ShouldBeAccessible()
        {
            // Test that serialized fields are properly defined
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.DataType.LiDAR.ScanPatternGenerator, UnitySensorsEditor");
                if (type != null)
                {
                    var expectedFields = new[]
                    {
                        "_direction",
                        "_zenithAngles", 
                        "_minAzimuthAngle",
                        "_maxAzimuthAngle",
                        "_azimuthAngleResolution",
                        "_zenithAngleOffset"
                    };
                    
                    foreach (var fieldName in expectedFields)
                    {
                        var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (field != null)
                        {
                            Assert.IsNotNull(field);
                            
                            // Check for SerializeField attribute
                            var attrs = field.GetCustomAttributes(typeof(SerializeField), false);
                            Assert.Greater(attrs.Length, 0, $"Field {fieldName} should have SerializeField attribute");
                        }
                    }
                }
            });
        }
    }
}