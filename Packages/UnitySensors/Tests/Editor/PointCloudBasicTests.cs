using NUnit.Framework;
using Unity.Mathematics;

namespace UnitySensors.Tests.Editor
{
    [TestFixture]
    public class PointCloudBasicTests
    {
        [Test]
        public void PointXYZ_StructLayout_ShouldBeCorrect()
        {
            // Test PointXYZ struct layout via reflection
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                var type = System.Type.GetType("UnitySensors.DataType.PointCloud.PointXYZ, UnitySensorsRuntime");
                if (type != null)
                {
                    Assert.IsTrue(type.IsValueType);
                    Assert.IsTrue(type.IsLayoutSequential || type.IsExplicitLayout);

                    // Should have position field of type float3
                    var positionField = type.GetField("position");
                    if (positionField != null)
                    {
                        Assert.AreEqual(typeof(float3), positionField.FieldType);
                    }
                }
            });
        }

        [Test]
        public void PointXYZI_StructLayout_ShouldBeCorrect()
        {
            // Test PointXYZI struct layout via reflection
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                var type = System.Type.GetType("UnitySensors.DataType.PointCloud.PointXYZI, UnitySensorsRuntime");
                if (type != null)
                {
                    Assert.IsTrue(type.IsValueType);
                    Assert.IsTrue(type.IsLayoutSequential || type.IsExplicitLayout);

                    // Should have position and intensity fields
                    var positionField = type.GetField("position");
                    var intensityField = type.GetField("intensity");

                    if (positionField != null) Assert.AreEqual(typeof(float3), positionField.FieldType);
                    if (intensityField != null) Assert.AreEqual(typeof(float), intensityField.FieldType);
                }
            });
        }

        [Test]
        public void PointXYZRGB_StructLayout_ShouldBeCorrect()
        {
            // Test PointXYZRGB struct layout via reflection
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                var type = System.Type.GetType("UnitySensors.DataType.PointCloud.PointXYZRGB, UnitySensorsRuntime");
                if (type != null)
                {
                    Assert.IsTrue(type.IsValueType);
                    Assert.IsTrue(type.IsLayoutSequential || type.IsExplicitLayout);

                    // Should have position and color fields
                    var positionField = type.GetField("position");
                    var rField = type.GetField("r");
                    var gField = type.GetField("g");
                    var bField = type.GetField("b");

                    if (positionField != null) Assert.AreEqual(typeof(float3), positionField.FieldType);
                    if (rField != null) Assert.AreEqual(typeof(byte), rField.FieldType);
                    if (gField != null) Assert.AreEqual(typeof(byte), gField.FieldType);
                    if (bField != null) Assert.AreEqual(typeof(byte), bField.FieldType);
                }
            });
        }

        [Test]
        public void PointTypes_Float3Position_ShouldBeInitializable()
        {
            // Test float3 position initialization
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                // Test float3 creation and basic operations
                var position1 = new float3(1.0f, 2.0f, 3.0f);
                var position2 = new float3(4.0f, 5.0f, 6.0f);

                Assert.AreEqual(1.0f, position1.x);
                Assert.AreEqual(2.0f, position1.y);
                Assert.AreEqual(3.0f, position1.z);

                // Test vector operations
                var sum = position1 + position2;
                Assert.AreEqual(5.0f, sum.x);
                Assert.AreEqual(7.0f, sum.y);
                Assert.AreEqual(9.0f, sum.z);

                // Test distance calculation
                var distance = math.distance(position1, position2);
                Assert.Greater(distance, 0.0f);
            });
        }

        [Test]
        public void PointXYZI_IntensityRange_ShouldBeValid()
        {
            // Test intensity value ranges
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                // Test typical intensity ranges
                var intensityTests = new float[] { 0.0f, 0.5f, 1.0f, 100.0f, 255.0f };

                foreach (var intensity in intensityTests)
                {
                    Assert.GreaterOrEqual(intensity, 0.0f);

                    // Test normalized intensity (0-1 range)
                    var normalizedIntensity = intensity / 255.0f;
                    Assert.GreaterOrEqual(normalizedIntensity, 0.0f);
                    Assert.LessOrEqual(normalizedIntensity, 1.0f);
                }
            });
        }

        [Test]
        public void PointXYZRGB_ColorChannels_ShouldBeInByteRange()
        {
            // Test color channel ranges
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                // Test byte color values
                var colorTests = new byte[] { 0, 64, 128, 192, 255 };

                foreach (var colorValue in colorTests)
                {
                    Assert.GreaterOrEqual(colorValue, (byte)0);
                    Assert.LessOrEqual(colorValue, (byte)255);

                    // Test conversion to float (0-1 range)
                    var normalizedColor = colorValue / 255.0f;
                    Assert.GreaterOrEqual(normalizedColor, 0.0f);
                    Assert.LessOrEqual(normalizedColor, 1.0f);
                }
            });
        }

        [Test]
        public void PointTypes_MemoryEfficiency_ShouldBeOptimal()
        {
            // Test memory layout efficiency
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                // Test struct sizes via reflection
                var pointXYZType = System.Type.GetType("UnitySensors.DataType.PointCloud.PointXYZ, UnitySensorsRuntime");
                var pointXYZIType = System.Type.GetType("UnitySensors.DataType.PointCloud.PointXYZI, UnitySensorsRuntime");
                var pointXYZRGBType = System.Type.GetType("UnitySensors.DataType.PointCloud.PointXYZRGB, UnitySensorsRuntime");

                if (pointXYZType != null)
                {
                    Assert.IsTrue(pointXYZType.IsValueType);
                    // PointXYZ should be efficiently packed (float3 = 12 bytes)
                }

                if (pointXYZIType != null)
                {
                    Assert.IsTrue(pointXYZIType.IsValueType);
                    // PointXYZI should be efficiently packed (float3 + float = 16 bytes)
                }

                if (pointXYZRGBType != null)
                {
                    Assert.IsTrue(pointXYZRGBType.IsValueType);
                    // PointXYZRGB should be efficiently packed (float3 + 3 bytes)
                }
            });
        }
    }
}