using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnitySensors.Visualization
{
    [RequireComponent(typeof(IMUSensor))]
    public class IMUVisualizer : Visualizer<IMUSensor>
    {
        [SerializeField]
        private SphereSetting _position;

        [SerializeField]
        private LineSetting _velocity;

        [SerializeField]
        private LineSetting _acceleration;

        protected override void Visualize()
        {
            Vector3 pos = _target.position;
            Vector3 vel = _target.velocity;
            Vector3 acc = _target.acceleration;

            Gizmos.color = _position.color;
            Gizmos.DrawSphere(pos, _position.radius);

            if (_velocity.fixLineLength) vel.Normalize();
            Gizmos.color = _velocity.color;
            Gizmos.DrawLine(pos, pos + vel * _velocity.lineLengthFactor);

            if (_acceleration.fixLineLength) acc.Normalize();
            Gizmos.color = _acceleration.color;
            Gizmos.DrawLine(pos, pos + acc * _acceleration.lineLengthFactor);
        }
    }
}
