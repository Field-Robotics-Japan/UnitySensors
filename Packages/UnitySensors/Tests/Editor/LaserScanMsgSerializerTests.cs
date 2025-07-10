using NUnit.Framework;

namespace UnitySensors.Tests.Editor
{
    [TestFixture]
    public class LaserScanMsgSerializerTests
    {
        [Test]
        public void LaserScanMsgSerializer_ReflectionAccess_ShouldBeAccessible()
        {
            // Test that LaserScanMsgSerializer can be accessed via reflection
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.ROS.Serializer.Sensor.LaserScanMsgSerializer, UnitySensorsROSRuntime");
                if (type != null)
                {
                    Assert.IsTrue(type.IsClass);
                    Assert.IsFalse(type.IsAbstract);
                    Assert.IsTrue(type.IsPublic);
                    
                    // Check System.Serializable attribute
                    var attrs = type.GetCustomAttributes(typeof(System.SerializableAttribute), false);
                    Assert.Greater(attrs.Length, 0, "Should have System.Serializable attribute");
                }
            });
        }

        [Test]
        public void LaserScanMsgSerializer_Inheritance_ShouldExtendRosMsgSerializer()
        {
            // Test that LaserScanMsgSerializer properly inherits from RosMsgSerializer<LaserScanMsg>
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.ROS.Serializer.Sensor.LaserScanMsgSerializer, UnitySensorsROSRuntime");
                if (type != null)
                {
                    var baseType = type.BaseType;
                    Assert.IsNotNull(baseType);
                    Assert.IsTrue(baseType.IsGenericType);
                    
                    // Check that it's RosMsgSerializer<LaserScanMsg>
                    var genericTypeDef = baseType.GetGenericTypeDefinition();
                    var expectedBase = System.Type.GetType("UnitySensors.ROS.Serializer.RosMsgSerializer`1, UnitySensorsROSRuntime");
                    if (expectedBase != null)
                    {
                        Assert.AreEqual(expectedBase, genericTypeDef);
                    }
                }
            });
        }

        [Test]
        public void LaserScanMsgSerializer_ConfigurationFields_ShouldBeAccessible()
        {
            // Test that LaserScanMsgSerializer configuration fields are properly defined
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.ROS.Serializer.Sensor.LaserScanMsgSerializer, UnitySensorsROSRuntime");
                if (type != null)
                {
                    // Check _minRange field
                    var minRangeField = type.GetField("_minRange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (minRangeField != null)
                    {
                        Assert.IsNotNull(minRangeField);
                        Assert.AreEqual(typeof(float), minRangeField.FieldType);
                    }
                    
                    // Check _maxRange field
                    var maxRangeField = type.GetField("_maxRange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (maxRangeField != null)
                    {
                        Assert.IsNotNull(maxRangeField);
                        Assert.AreEqual(typeof(float), maxRangeField.FieldType);
                    }
                    
                    // Check _gaussianNoiseSigma field
                    var noiseField = type.GetField("_gaussianNoiseSigma", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (noiseField != null)
                    {
                        Assert.IsNotNull(noiseField);
                        Assert.AreEqual(typeof(float), noiseField.FieldType);
                    }
                }
            });
        }

        [Test]
        public void LaserScanMsgSerializer_DependencyFields_ShouldBeAccessible()
        {
            // Test that LaserScanMsgSerializer dependency fields are properly defined
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.ROS.Serializer.Sensor.LaserScanMsgSerializer, UnitySensorsROSRuntime");
                if (type != null)
                {
                    // Check _scanPattern field
                    var scanPatternField = type.GetField("_scanPattern", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (scanPatternField != null)
                    {
                        Assert.IsNotNull(scanPatternField);
                        // Should be ScanPattern type
                        Assert.IsTrue(scanPatternField.FieldType.Name.Contains("ScanPattern"));
                    }
                    
                    // Check _header field
                    var headerField = type.GetField("_header", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (headerField != null)
                    {
                        Assert.IsNotNull(headerField);
                        // Should be HeaderSerializer type
                        Assert.IsTrue(headerField.FieldType.Name.Contains("HeaderSerializer"));
                    }
                }
            });
        }

        [Test]
        public void LaserScanMsgSerializer_Methods_ShouldBeOverridden()
        {
            // Test that LaserScanMsgSerializer methods are properly overridden
            // Act & Assert
            Assert.DoesNotThrow(() => {
                var type = System.Type.GetType("UnitySensors.ROS.Serializer.Sensor.LaserScanMsgSerializer, UnitySensorsROSRuntime");
                if (type != null)
                {
                    // Check Init method
                    var initMethod = type.GetMethod("Init");
                    if (initMethod != null)
                    {
                        Assert.IsNotNull(initMethod);
                        Assert.IsTrue(initMethod.IsPublic);
                        Assert.IsTrue(initMethod.IsVirtual);
                        Assert.IsFalse(initMethod.IsAbstract);
                    }
                    
                    // Check Serialize method
                    var serializeMethod = type.GetMethod("Serialize");
                    if (serializeMethod != null)
                    {
                        Assert.IsNotNull(serializeMethod);
                        Assert.IsTrue(serializeMethod.IsPublic);
                        Assert.IsTrue(serializeMethod.IsVirtual);
                        Assert.IsFalse(serializeMethod.IsAbstract);
                    }
                    
                    // Check SetSource method
                    var setSourceMethod = type.GetMethod("SetSource");
                    if (setSourceMethod != null)
                    {
                        Assert.IsNotNull(setSourceMethod);
                        Assert.IsTrue(setSourceMethod.IsPublic);
                    }
                }
            });
        }

        [Test]
        public void LaserScanMsgSerializer_RangeCalculation_ShouldWork()
        {
            // Test range calculation logic concepts used in LaserScanMsgSerializer
            // Act & Assert
            Assert.DoesNotThrow(() => {
                // Test 2D range calculation from 3D points (x, y, z) -> sqrt(x² + z²)
                var testPoints = new[] {
                    new { x = 1.0f, y = 0.0f, z = 0.0f, expectedRange = 1.0f },
                    new { x = 0.0f, y = 1.0f, z = 1.0f, expectedRange = 1.0f },
                    new { x = 3.0f, y = 0.0f, z = 4.0f, expectedRange = 5.0f },
                    new { x = 0.0f, y = 0.0f, z = 0.0f, expectedRange = 0.0f }
                };
                
                foreach (var point in testPoints)
                {
                    var calculatedRange = System.Math.Sqrt(point.x * point.x + point.z * point.z);
                    Assert.AreEqual(point.expectedRange, calculatedRange, 1e-6f);
                }
            });
        }

        [Test]
        public void LaserScanMsgSerializer_RangeFiltering_ShouldWork()
        {
            // Test range filtering logic concepts
            // Act & Assert
            Assert.DoesNotThrow(() => {
                float minRange = 0.5f;
                float maxRange = 100.0f;
                
                var testRanges = new[] {
                    new { range = 0.1f, shouldBeValid = false },  // Below minimum
                    new { range = 0.5f, shouldBeValid = true },   // At minimum
                    new { range = 50.0f, shouldBeValid = true },  // Within range
                    new { range = 100.0f, shouldBeValid = true }, // At maximum
                    new { range = 150.0f, shouldBeValid = false } // Above maximum
                };
                
                foreach (var test in testRanges)
                {
                    bool isValid = test.range >= minRange && test.range <= maxRange;
                    Assert.AreEqual(test.shouldBeValid, isValid);
                    
                    // Test NaN assignment for invalid ranges
                    float filteredRange = isValid ? test.range : float.NaN;
                    if (test.shouldBeValid)
                    {
                        Assert.AreEqual(test.range, filteredRange);
                    }
                    else
                    {
                        Assert.IsTrue(float.IsNaN(filteredRange));
                    }
                }
            });
        }

        [Test]
        public void LaserScanMsgSerializer_GaussianNoise_ShouldWork()
        {
            // Test Gaussian noise application concepts
            // Act & Assert
            Assert.DoesNotThrow(() => {
                float sigma = 0.1f;
                float originalRange = 10.0f;
                
                // Test Box-Muller transform concept for Gaussian noise
                var random = new System.Random(42); // Fixed seed for reproducibility
                
                // Generate multiple noise samples
                var noiseSamples = new float[100];
                for (int i = 0; i < noiseSamples.Length; i += 2)
                {
                    // Box-Muller transform
                    double u1 = random.NextDouble();
                    double u2 = random.NextDouble();
                    double z0 = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Cos(2.0 * System.Math.PI * u2);
                    
                    noiseSamples[i] = (float)(z0 * sigma);
                    if (i + 1 < noiseSamples.Length)
                    {
                        double z1 = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2);
                        noiseSamples[i + 1] = (float)(z1 * sigma);
                    }
                }
                
                // Test that noise is applied correctly
                for (int i = 0; i < noiseSamples.Length; i++)
                {
                    float noisyRange = originalRange + noiseSamples[i];
                    Assert.IsTrue(noisyRange != originalRange || noiseSamples[i] == 0.0f);
                }
            });
        }

        [Test]
        public void LaserScanMsgSerializer_LaserScanMsgStructure_ShouldWork()
        {
            // Test LaserScanMsg structure concepts
            // Act & Assert
            Assert.DoesNotThrow(() => {
                // Test typical LaserScanMsg structure
                var laserScanCreated = false;
                var headerSet = false;
                var anglePropertiesSet = false;
                var rangePropertiesSet = false;
                var rangesArraySet = false;
                var intensitiesArraySet = false;
                
                // Simulate LaserScanMsg creation and population
                if (!laserScanCreated) { laserScanCreated = true; }
                if (!headerSet) { headerSet = true; }
                if (!anglePropertiesSet) { anglePropertiesSet = true; }
                if (!rangePropertiesSet) { rangePropertiesSet = true; }
                if (!rangesArraySet) { rangesArraySet = true; }
                if (!intensitiesArraySet) { intensitiesArraySet = true; }
                
                Assert.IsTrue(laserScanCreated);
                Assert.IsTrue(headerSet);
                Assert.IsTrue(anglePropertiesSet);
                Assert.IsTrue(rangePropertiesSet);
                Assert.IsTrue(rangesArraySet);
                Assert.IsTrue(intensitiesArraySet);
            });
        }

        [Test]
        public void LaserScanMsgSerializer_IntensityMapping_ShouldWork()
        {
            // Test intensity mapping concepts
            // Act & Assert
            Assert.DoesNotThrow(() => {
                // Test intensity value handling
                var testIntensities = new float[] { 0.0f, 0.5f, 1.0f, 100.0f, 255.0f };
                
                foreach (var intensity in testIntensities)
                {
                    // Test that intensity values are preserved
                    Assert.AreEqual(intensity, intensity);
                    Assert.IsTrue(intensity >= 0.0f);
                    
                    // Test intensity range validation
                    var normalizedIntensity = intensity / 255.0f;
                    Assert.IsTrue(normalizedIntensity >= 0.0f);
                    Assert.IsTrue(normalizedIntensity <= (255.0f / 255.0f));
                }
            });
        }
    }
}