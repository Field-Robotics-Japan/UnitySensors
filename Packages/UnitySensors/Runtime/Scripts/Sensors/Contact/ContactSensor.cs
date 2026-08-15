using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnitySensors.Attribute;
using UnitySensors.DataType.Sensor;
using UnitySensors.Interface.Sensor;
using UnitySensors.Interface.Std;

namespace UnitySensors.Sensor.Contact
{
    public class ContactSensor : UnitySensor, IContactDataInterface, IBoolStateInterface, IWrenchInterface
    {
        [SerializeField, ReadOnly]
        private bool _isContact;
        [SerializeField, ReadOnly]
        private List<ContactData> _contacts = new List<ContactData>();

        private Dictionary<Collider, ContactData> _activeContacts = new Dictionary<Collider, ContactData>();
        private List<Collider> _removeBuffer = new List<Collider>();
        // Colliders that belong to this sensor. Empty means "anything that reaches me".
        private HashSet<Collider> _ownColliders = new HashSet<Collider>();
        private Vector3 _totalForce;
        private Vector3 _totalTorque;

        public bool isContact { get => _isContact; }
        public Vector3 totalForce { get => _totalForce; }
        public Vector3 totalTorque { get => _totalTorque; }
        public Vector3 localTotalForce { get => transform.InverseTransformDirection(_totalForce); }
        public Vector3 localTotalTorque { get => transform.InverseTransformDirection(_totalTorque); }
        public IReadOnlyList<ContactData> contacts { get => _contacts; }

        // Generic serializer sources: bumper state and the net contact wrench
        // in the sensor's local frame.
        bool IBoolStateInterface.state { get => _isContact; }
        Vector3 IWrenchInterface.force { get => localTotalForce; }
        Vector3 IWrenchInterface.torque { get => localTotalTorque; }

        protected override void Init()
        {
            // Unity delivers OnCollision* only to the GameObject that carries the
            // Rigidbody/ArticulationBody - never to a collider-only child. Relays on the
            // children therefore fire for the case where the sensor sits on the body
            // itself and the colliders hang below it, but not for a sensor on a part that
            // shares a body with its parent. RegisterBodyRelay() covers the latter.
            AttachRelays(transform);
        }

        /// <summary>
        /// Listen to the collisions of a body this sensor does not own, keeping only the
        /// ones that touched a collider registered through <see cref="RegisterOwnCollider"/>.
        /// </summary>
        /// <remarks>
        /// A URDF link attached by a fixed joint has no body of its own - its colliders
        /// belong to the nearest ancestor that has one, and that ancestor is where Unity
        /// sends the collision. Without this the sensor hears nothing; with this but no
        /// own-collider filter it would report every contact of the whole assembly, so a
        /// bumper brushing a wall would read as a hit on the plate.
        /// </remarks>
        public void RegisterBodyRelay(GameObject bodyObject)
        {
            if (bodyObject == null || bodyObject == gameObject) return;
            foreach (ContactEventRelay existing in bodyObject.GetComponents<ContactEventRelay>())
            {
                if (existing.Target == this) return;
            }
            bodyObject.AddComponent<ContactEventRelay>().Initialize(this);
        }

        /// <summary>
        /// Declare a collider as belonging to this sensor. Once any is declared, contacts
        /// on other colliders of the same body are ignored.
        /// </summary>
        public void RegisterOwnCollider(Collider collider)
        {
            if (collider != null) _ownColliders.Add(collider);
        }

        /// <summary>
        /// Relay OnCollision* events from the given collider's GameObject to
        /// this sensor. Use this when the collider sits outside the default
        /// scan, e.g. on a child GameObject that carries its own body.
        /// </summary>
        public void RegisterCollider(Collider target)
        {
            if (target == null || target.gameObject == gameObject) return;
            if (target.GetComponent<ContactEventRelay>() == null)
            {
                target.gameObject.AddComponent<ContactEventRelay>().Initialize(this);
            }
        }

        private void AttachRelays(Transform target)
        {
            if (target != transform &&
                (target.GetComponent<Rigidbody>() != null || target.GetComponent<ArticulationBody>() != null))
            {
                return;
            }

            if (target != transform &&
                target.GetComponent<Collider>() != null &&
                target.GetComponent<ContactEventRelay>() == null)
            {
                target.gameObject.AddComponent<ContactEventRelay>().Initialize(this);
            }

            for (int i = 0; i < target.childCount; i++)
            {
                AttachRelays(target.GetChild(i));
            }
        }

        // Colliders on the same GameObject as the body report here directly;
        // collider-only children report through ContactEventRelay.
        private void OnCollisionEnter(Collision collision)
        {
            UpdateContact(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            UpdateContact(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            _activeContacts.Remove(collision.collider);
        }

        internal void HandleRelayedCollisionEnter(Collision collision)
        {
            UpdateContact(collision);
        }

        internal void HandleRelayedCollisionStay(Collision collision)
        {
            UpdateContact(collision);
        }

        internal void HandleRelayedCollisionExit(Collision collision)
        {
            _activeContacts.Remove(collision.collider);
        }

        private void UpdateContact(Collision collision)
        {
            if (collision.contactCount == 0) return;

            // Walk every contact point: one Collision can carry points on several of the
            // body's colliders, and only some of them may be ours.
            int matched = 0;
            ContactPoint mine = default;
            for (int i = 0; i < collision.contactCount; i++)
            {
                ContactPoint point = collision.GetContact(i);
                if (_ownColliders.Count > 0 && !_ownColliders.Contains(point.thisCollider)) continue;
                if (matched == 0) mine = point;
                matched++;
            }
            if (matched == 0)
            {
                // Nothing on our side of the assembly was touched.
                _activeContacts.Remove(collision.collider);
                return;
            }

            ContactData contact = new ContactData();
            contact.colliderName = collision.collider.name;
            contact.position = mine.point;
            contact.normal = mine.normal;
            // impulse is reported per collider pair, so when only part of the contact
            // points are ours the force is scaled by their share rather than measured.
            contact.force = collision.impulse / Time.fixedDeltaTime
                            * ((float)matched / collision.contactCount);
            _activeContacts[collision.collider] = contact;
        }

        protected override IEnumerator UpdateSensor()
        {
            // A collider destroyed while touching never sends OnCollisionExit,
            // so drop destroyed keys before aggregating.
            _removeBuffer.Clear();
            foreach (Collider collider in _activeContacts.Keys)
            {
                if (collider == null) _removeBuffer.Add(collider);
            }
            foreach (Collider collider in _removeBuffer)
            {
                _activeContacts.Remove(collider);
            }

            _contacts.Clear();
            _totalForce = Vector3.zero;
            _totalTorque = Vector3.zero;

            Vector3 origin = transform.position;
            foreach (ContactData contact in _activeContacts.Values)
            {
                _contacts.Add(contact);
                _totalForce += contact.force;
                _totalTorque += Vector3.Cross(contact.position - origin, contact.force);
            }
            _isContact = _contacts.Count > 0;

            yield return null;
        }

        protected override void OnSensorDestroy()
        {
        }
    }
}
