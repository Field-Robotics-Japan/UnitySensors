using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FRJ.Sensor
{
    [RequireComponent(typeof(Rigidbody))]
    public class IMU : MonoBehaviour
    {
        private Rigidbody _rb;
        private Transform _trans;

        // Previous value
        private Vector3 _lastVelocity = Vector3.zero;

        private Vector4 _geometryQuaternion;
        private Vector3 _angularVelocity;
        private Vector3 _linearAcceleration;

        private Noise.Gaussian gaussianNoise;
        private Noise.Bias biasNoise;

        [SerializeField] private float _scanRate = 100f;
        public float scanRate { get => this._scanRate; }
        
        public bool enableGaussianNoise;
        public bool enableBiasNoise;
        public NoiseSetting setting = new NoiseSetting();

        public Vector4 GeometryQuaternion { get => _geometryQuaternion; }
        public Vector3 AngularVelocity { get => _angularVelocity; }
        public Vector3 LinearAcceleration { get => _linearAcceleration; }

        [System.Serializable]
        public class NoiseSetting
        {
            public Vector4 quatSigma;
            public Vector4 quatBias;
            public Vector3 angVelSigma;
            public Vector3 angVelBias;
            public Vector3 linAccSigma;
            public Vector3 linAccBias;
        }

        private void Start()
        {
            this._trans = this.GetComponent<Transform>();
            this._rb = this.GetComponent<Rigidbody>();
            this._geometryQuaternion = new Vector4();
            this._angularVelocity = new Vector3();
            this._linearAcceleration = new Vector3();
        }

        public void UpdateIMU()
        {
            // Update Object State //
            // Calculate Move Element
            Vector3 localLinearVelocity = this._trans.InverseTransformDirection(this._rb.velocity);
            Vector3 acceleration = (localLinearVelocity - this._lastVelocity) / Time.deltaTime;
            this._lastVelocity = localLinearVelocity;
            // Add Gravity Element
            acceleration += this._trans.InverseTransformDirection(Physics.gravity);

            // Update //

            // Raw
            this._geometryQuaternion = new Vector4(this._trans.rotation.x, this._trans.rotation.y, this._trans.rotation.z, this._trans.rotation.w);
            this._angularVelocity = -1 * this.transform.InverseTransformVector(this._rb.angularVelocity);
            this._linearAcceleration = acceleration;

            // Apply Gaussian Noise
            // if (this.enableGaussianNoise) { this._geometryQuaternion = this.gaussianNoise.Apply(this._geometryQuaternion, this.setting.quatSigma); }
            // if (this.enableGaussianNoise) { this._angularVelocity = this.gaussianNoise.Apply(this._angularVelocity, this.setting.angVelSigma); }
            // if (this.enableGaussianNoise) { this._linearAcceleration = this.gaussianNoise.Apply(this._linearAcceleration, this.setting.linAccSigma); }

            // // Apply Bias Noise
            // if (this.enableBiasNoise) { this._geometryQuaternion = this.biasNoise.Apply(this._geometryQuaternion, this.setting.quatSigma); }
            // if (this.enableBiasNoise) { this._angularVelocity = this.biasNoise.Apply(this._angularVelocity, this.setting.angVelSigma); }
            // if (this.enableBiasNoise) { this._linearAcceleration = this.biasNoise.Apply(this._linearAcceleration, this.setting.linAccSigma); }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(IMU))]
        public class IMUEditor : Editor
        {
            private IMU variables;

            private void Awake()
            {
                this.variables = target as IMU;
            }

            // inspectorÇÃGUIê›íË
            public override void OnInspectorGUI()
            {
                EditorGUI.BeginChangeCheck();

                this.variables.enableGaussianNoise = EditorGUILayout.ToggleLeft("Enable Gaussian Noise", this.variables.enableGaussianNoise);
                if (this.variables.enableGaussianNoise)
                {
                    EditorGUILayout.LabelField("Gaussian Noise Setting");
                    this.variables.setting.quatSigma = EditorGUILayout.Vector4Field("->Quaternion Sigma", this.variables.setting.quatSigma);
                    this.variables.setting.angVelSigma = EditorGUILayout.Vector3Field("->AngularVelocity Sigma", this.variables.setting.angVelSigma);
                    this.variables.setting.linAccSigma = EditorGUILayout.Vector3Field("->LinearAcceleration Sigma", this.variables.setting.linAccSigma);
                }
                this.variables.enableBiasNoise = EditorGUILayout.ToggleLeft("Enable Bias Noise", this.variables.enableBiasNoise);
                if (this.variables.enableBiasNoise)
                {
                    EditorGUILayout.LabelField("Bias Noise Setting");
                    this.variables.setting.quatBias = EditorGUILayout.Vector4Field("->Quaternion Bias", this.variables.setting.quatBias);
                    this.variables.setting.angVelBias = EditorGUILayout.Vector3Field("->AngularVelocity Bias", this.variables.setting.angVelBias);
                    this.variables.setting.linAccBias = EditorGUILayout.Vector3Field("->LinearAcceleration Bias", this.variables.setting.linAccBias);
                }

                // GUIÇÃçXêVÇ™Ç†Ç¡ÇΩÇÁé¿çs
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(this.variables);
                }
            }
        }
#endif

    }
}
