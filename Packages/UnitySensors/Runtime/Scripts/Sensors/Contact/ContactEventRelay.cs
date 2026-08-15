using UnityEngine;

namespace UnitySensors.Sensor.Contact
{
    /// <summary>
    /// Forwards OnCollision* messages to a ContactSensor that does not sit on the
    /// GameObject Unity delivers them to. Added automatically by ContactSensor; not meant
    /// to be attached by hand.
    /// </summary>
    [AddComponentMenu("")]
    public class ContactEventRelay : MonoBehaviour
    {
        private ContactSensor _sensor;

        /// <summary>Which sensor this relay feeds, so duplicates can be spotted.</summary>
        public ContactSensor Target { get => _sensor; }

        public void Initialize(ContactSensor sensor)
        {
            _sensor = sensor;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_sensor != null) _sensor.HandleRelayedCollisionEnter(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            if (_sensor != null) _sensor.HandleRelayedCollisionStay(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (_sensor != null) _sensor.HandleRelayedCollisionExit(collision);
        }
    }
}
