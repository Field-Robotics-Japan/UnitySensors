using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySensors
{
    [System.Serializable]
    public class NMEAFormatManager : AttachableScriptableObjectManager
    {
        private NMEAFormat _format;

        public override void Start()
        {
            base.Start();
            _format = base._scriptableObject as NMEAFormat;
            if (_format) _format.Init();
            else Debug.LogError("Type of NMEAFormat does not match.");
        }

        public override void Update()
        {
            base.Update();
        }

        public string Serialize(GeoCoordinate coordinate, Vector3 velocity)
        {
            return _format.Serialize(coordinate, velocity);
        }
    }
}
