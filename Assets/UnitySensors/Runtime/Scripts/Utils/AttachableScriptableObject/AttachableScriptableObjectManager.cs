using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnitySensors
{
    [System.Serializable]
    public class AttachableScriptableObjectManager
    {
        public AttachableScriptableObject scriptableObject;

        public string _parametersCache;
        [ReadableScriptableObject]
        [SerializeField]
        protected AttachableScriptableObject _scriptableObject;

        public bool updated { get => GetUpdated(); }
        private bool _updated;

        public virtual void Start()
        {
            if (!_scriptableObject && scriptableObject) _scriptableObject = GameObject.Instantiate(scriptableObject);
            if (_scriptableObject) JsonUtility.FromJsonOverwrite(_parametersCache, _scriptableObject);
        }

        public virtual void Update()
        {
            if (!Application.isPlaying)
            {
                if (!scriptableObject) return;
                if (!_scriptableObject || _scriptableObject.GetType() != scriptableObject.GetType())
                {
                    _scriptableObject = GameObject.Instantiate(scriptableObject);
                    JsonUtility.FromJsonOverwrite(_parametersCache, _scriptableObject);
                }
                if (!_scriptableObject) return;
                string _cache_old = _parametersCache;
                _parametersCache = JsonUtility.ToJson(_scriptableObject);
                if (_cache_old != _parametersCache) { _updated = true; }
                return;
            }
            if (_scriptableObject) _scriptableObject.Update();
        }

        private bool GetUpdated()
        {
            if (!_updated) return false;
            _updated = false;
            return true;
        }
    }
}