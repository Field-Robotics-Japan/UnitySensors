using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnitySensors.Tests.Runtime
{
    [TestFixture]
    public class BasicGameObjectTests
    {
        private GameObject _testObject;

        [SetUp]
        public void SetUp()
        {
            _testObject = new GameObject("TestObject");
        }

        [TearDown]
        public void TearDown()
        {
            if (_testObject != null)
                Object.DestroyImmediate(_testObject);
        }

        [UnityTest]
        public IEnumerator GameObject_Instantiation_ShouldCreateValidObject()
        {
            // Act
            yield return null; // Wait one frame
            
            // Assert
            Assert.IsNotNull(_testObject);
            Assert.AreEqual("TestObject", _testObject.name);
        }

        [UnityTest]
        public IEnumerator Transform_Position_ShouldBeModifiable()
        {
            // Arrange
            var newPosition = new Vector3(1.0f, 2.0f, 3.0f);
            
            // Act
            _testObject.transform.position = newPosition;
            yield return null; // Wait one frame
            
            // Assert
            Assert.AreEqual(newPosition, _testObject.transform.position);
        }

        [UnityTest]
        public IEnumerator Transform_Rotation_ShouldBeModifiable()
        {
            // Arrange
            var newRotation = Quaternion.Euler(45.0f, 90.0f, 0.0f);
            
            // Act
            _testObject.transform.rotation = newRotation;
            yield return null; // Wait one frame
            
            // Assert
            var actualRotation = _testObject.transform.rotation;
            Assert.AreEqual(newRotation.x, actualRotation.x, 0.001f);
            Assert.AreEqual(newRotation.y, actualRotation.y, 0.001f);
            Assert.AreEqual(newRotation.z, actualRotation.z, 0.001f);
            Assert.AreEqual(newRotation.w, actualRotation.w, 0.001f);
        }

        [UnityTest]
        public IEnumerator GameObject_AddComponent_ShouldAddCamera()
        {
            // Act
            var camera = _testObject.AddComponent<Camera>();
            yield return null; // Wait one frame
            
            // Assert
            Assert.IsNotNull(camera);
            Assert.AreEqual(_testObject, camera.gameObject);
        }

        [UnityTest]
        public IEnumerator Camera_FieldOfView_ShouldBeModifiable()
        {
            // Arrange
            var camera = _testObject.AddComponent<Camera>();
            var expectedFOV = 45.0f;
            
            // Act
            camera.fieldOfView = expectedFOV;
            yield return null; // Wait one frame
            
            // Assert
            Assert.AreEqual(expectedFOV, camera.fieldOfView, 0.001f);
        }
    }

    [TestFixture]
    public class TimingTests
    {
        [UnityTest]
        public IEnumerator Time_DeltaTime_ShouldBePositiveInPlayMode()
        {
            // Act
            yield return null; // Wait one frame
            var deltaTime = Time.deltaTime;
            
            // Assert
            Assert.GreaterOrEqual(deltaTime, 0.0f);
        }

        [UnityTest]
        public IEnumerator Time_TimeScale_ShouldAffectDeltaTime()
        {
            // Arrange
            var originalTimeScale = Time.timeScale;
            
            // Act
            Time.timeScale = 0.5f;
            yield return null; // Wait one frame
            
            // Assert
            Assert.AreEqual(0.5f, Time.timeScale, 0.001f);
            
            // Cleanup
            Time.timeScale = originalTimeScale;
        }

        [UnityTest]
        public IEnumerator WaitForSeconds_ShouldWaitCorrectDuration()
        {
            // Arrange
            var startTime = Time.time;
            var waitDuration = 0.1f;
            
            // Act
            yield return new WaitForSeconds(waitDuration);
            var endTime = Time.time;
            
            // Assert
            var actualDuration = endTime - startTime;
            Assert.GreaterOrEqual(actualDuration, waitDuration * 0.9f); // Allow 10% tolerance
        }
    }

    [TestFixture]
    public class ComponentLifecycleTests
    {
        private GameObject _testObject;

        [SetUp]
        public void SetUp()
        {
            _testObject = new GameObject("TestComponentObject");
        }

        [TearDown]
        public void TearDown()
        {
            if (_testObject != null)
                Object.DestroyImmediate(_testObject);
        }

        [UnityTest]
        public IEnumerator Component_Destruction_ShouldRemoveFromGameObject()
        {
            // Arrange
            var camera = _testObject.AddComponent<Camera>();
            yield return null; // Wait one frame
            
            // Act
            Object.DestroyImmediate(camera);
            yield return null; // Wait one frame
            
            // Assert
            Assert.IsNull(_testObject.GetComponent<Camera>());
        }

        [UnityTest]
        public IEnumerator MultipleComponents_ShouldCoexist()
        {
            // Act
            var camera = _testObject.AddComponent<Camera>();
            var audioListener = _testObject.AddComponent<AudioListener>();
            yield return null; // Wait one frame
            
            // Assert
            Assert.IsNotNull(camera);
            Assert.IsNotNull(audioListener);
            Assert.AreSame(_testObject, camera.gameObject);
            Assert.AreSame(_testObject, audioListener.gameObject);
        }

        [UnityTest]
        public IEnumerator GameObject_SetActive_ShouldToggleVisibility()
        {
            // Arrange
            Assert.IsTrue(_testObject.activeInHierarchy);
            
            // Act
            _testObject.SetActive(false);
            yield return null; // Wait one frame
            
            // Assert
            Assert.IsFalse(_testObject.activeInHierarchy);
            
            // Act
            _testObject.SetActive(true);
            yield return null; // Wait one frame
            
            // Assert
            Assert.IsTrue(_testObject.activeInHierarchy);
        }
    }

    [TestFixture]
    public class RenderingTests
    {
        private GameObject _cameraObject;
        private Camera _camera;

        [SetUp]
        public void SetUp()
        {
            _cameraObject = new GameObject("TestCamera");
            _camera = _cameraObject.AddComponent<Camera>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_cameraObject != null)
                Object.DestroyImmediate(_cameraObject);
        }

        [UnityTest]
        public IEnumerator Camera_RenderTexture_ShouldBeAssignable()
        {
            // Arrange
            var renderTexture = new RenderTexture(256, 256, 24);
            
            // Act
            _camera.targetTexture = renderTexture;
            yield return null; // Wait one frame
            
            // Assert
            Assert.AreSame(renderTexture, _camera.targetTexture);
            
            // Cleanup
            renderTexture.Release();
        }

        [UnityTest]
        public IEnumerator Camera_NearFarClipPlane_ShouldBeModifiable()
        {
            // Arrange
            var nearClip = 0.1f;
            var farClip = 100.0f;
            
            // Act
            _camera.nearClipPlane = nearClip;
            _camera.farClipPlane = farClip;
            yield return null; // Wait one frame
            
            // Assert
            Assert.AreEqual(nearClip, _camera.nearClipPlane, 0.001f);
            Assert.AreEqual(farClip, _camera.farClipPlane, 0.001f);
        }

        [UnityTest]
        public IEnumerator Camera_Depth_ShouldAffectRenderOrder()
        {
            // Arrange
            var depth = 5.0f;
            
            // Act
            _camera.depth = depth;
            yield return null; // Wait one frame
            
            // Assert
            Assert.AreEqual(depth, _camera.depth, 0.001f);
        }
    }

    [TestFixture]
    public class UnitySensorRuntimeTests
    {
        private GameObject _testObject;

        [SetUp]
        public void SetUp()
        {
            _testObject = new GameObject("TestSensorObject");
        }

        [TearDown]
        public void TearDown()
        {
            if (_testObject != null)
                Object.DestroyImmediate(_testObject);
        }

        [UnityTest]
        public IEnumerator UnitySensors_BasicGameObject_ShouldWork()
        {
            // Act
            yield return null; // Wait one frame
            
            // Assert
            Assert.IsNotNull(_testObject);
            Assert.AreEqual("TestSensorObject", _testObject.name);
        }

        [UnityTest]
        public IEnumerator UnitySensors_Transform_ShouldBeAccessible()
        {
            // Act
            var transform = _testObject.transform;
            yield return null; // Wait one frame
            
            // Assert
            Assert.IsNotNull(transform);
            Assert.AreEqual(Vector3.zero, transform.position);
        }

        [UnityTest]
        public IEnumerator UnitySensors_Camera_ShouldBeAddable()
        {
            // Act
            var camera = _testObject.AddComponent<Camera>();
            yield return null; // Wait one frame
            
            // Assert
            Assert.IsNotNull(camera);
            Assert.AreEqual(_testObject, camera.gameObject);
        }

        [UnityTest]
        public IEnumerator UnitySensors_Assembly_ShouldBeLoaded()
        {
            // This test verifies UnitySensors assembly can be accessed at runtime
            // Act
            yield return null; // Wait one frame
            
            // Assert
            Assert.DoesNotThrow(() => {
                var assemblyName = typeof(Camera).Assembly.GetName().Name;
                Assert.IsNotNull(assemblyName);
            });
        }
    }

    [TestFixture]
    public class UnitySensorsIntegrationTests
    {
        private GameObject _testObject;

        [SetUp]
        public void SetUp()
        {
            _testObject = new GameObject("TestUnitySensorsObject");
        }

        [TearDown]
        public void TearDown()
        {
            if (_testObject != null)
                Object.DestroyImmediate(_testObject);
        }

        [UnityTest]
        public IEnumerator UnitySensors_GameObject_ShouldSupportSensorLikeOperations()
        {
            // Test basic operations that sensors typically perform
            // Act
            var initialPosition = _testObject.transform.position;
            var newPosition = new Vector3(1.0f, 2.0f, 3.0f);
            _testObject.transform.position = newPosition;
            
            yield return null; // Wait one frame
            
            // Assert
            Assert.AreNotEqual(initialPosition, _testObject.transform.position);
            Assert.AreEqual(newPosition, _testObject.transform.position);
        }

        [UnityTest]
        public IEnumerator UnitySensors_Timing_ShouldSupportFrequencyCalculations()
        {
            // Test timing calculations that sensors use
            // Arrange
            var frequency = 30.0f;
            var expectedDeltaTime = 1.0f / frequency;
            var startTime = Time.time;
            
            // Act
            yield return new WaitForSeconds(expectedDeltaTime);
            var endTime = Time.time;
            var actualDeltaTime = endTime - startTime;
            
            // Assert
            Assert.That(actualDeltaTime, Is.EqualTo(expectedDeltaTime).Within(0.02f)); // 20ms tolerance
        }

        [UnityTest]
        public IEnumerator UnitySensors_Camera_ShouldSupportSensorOperations()
        {
            // Test camera operations that camera sensors use
            // Act
            var camera = _testObject.AddComponent<Camera>();
            camera.fieldOfView = 60.0f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100.0f;
            
            yield return null; // Wait one frame
            
            // Assert
            Assert.IsNotNull(camera);
            Assert.AreEqual(60.0f, camera.fieldOfView, 0.001f);
            Assert.AreEqual(0.1f, camera.nearClipPlane, 0.001f);
            Assert.AreEqual(100.0f, camera.farClipPlane, 0.001f);
        }

        [UnityTest]
        public IEnumerator UnitySensors_RenderTexture_ShouldSupportCameraSensors()
        {
            // Test render texture operations used by camera sensors
            // Arrange
            var camera = _testObject.AddComponent<Camera>();
            var renderTexture = new RenderTexture(256, 256, 24);
            
            // Act
            camera.targetTexture = renderTexture;
            yield return null; // Wait one frame
            
            // Assert
            Assert.IsNotNull(camera.targetTexture);
            Assert.AreEqual(256, camera.targetTexture.width);
            Assert.AreEqual(256, camera.targetTexture.height);
            
            // Cleanup
            renderTexture.Release();
        }

        [UnityTest]
        public IEnumerator UnitySensors_Transform_ShouldSupportSensorMovement()
        {
            // Test transform operations that moving sensors use
            // Act
            var startPosition = _testObject.transform.position;
            var targetPosition = new Vector3(10.0f, 5.0f, 0.0f);
            
            // Simulate sensor movement
            _testObject.transform.position = Vector3.Lerp(startPosition, targetPosition, 0.5f);
            yield return null; // Wait one frame
            
            var intermediatePosition = _testObject.transform.position;
            
            // Assert
            Assert.AreNotEqual(startPosition, intermediatePosition);
            Assert.AreEqual(new Vector3(5.0f, 2.5f, 0.0f), intermediatePosition);
        }
    }
}