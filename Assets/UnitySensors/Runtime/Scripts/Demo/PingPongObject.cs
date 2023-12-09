using UnityEngine;

namespace UnitySensors.Demo
{

    public class PingPongObject : MonoBehaviour
    {
        [SerializeField]
        private float _speed;
        [SerializeField]
        private Vector3 _start;
        [SerializeField]
        private Vector3 _end;

        [SerializeField]
        private float _speed_coef;
        private Transform _transform;

        private void Start()
        {
            _speed_coef = _speed / Vector3.Distance(_start, _end);
            _transform = this.transform;
        }

        void Update()
        {
            float t = Mathf.PingPong(Time.time * _speed_coef, 1.0f);
            _transform.position = Vector3.Lerp(_start, _end, t);
        }
    }
}