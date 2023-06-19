using System;
using CodeBase.Player;
using UnityEngine;

namespace CodeBase.HookMachine
{
    public class Hook : MonoBehaviour
    {
        private FixedJoint _fixJoint;
        public Rigidbody Rigidbody;
        private Collider _collider;
        [SerializeField] private Collider _playerCollider;
        [SerializeField] private RopeGun _ropeGun;

        private void Start()
        {
            _collider = GetComponent<Collider>();
            Physics.IgnoreCollision(_collider, _playerCollider);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_fixJoint == null)
            {
                if (collision.gameObject.GetComponent<HookPlatform>())
                {
                    _fixJoint = gameObject.AddComponent<FixedJoint>();
                    if (collision.rigidbody)
                    {
                        _fixJoint.connectedBody = collision.rigidbody;
                    }
                    _ropeGun.CreateSpring();
                }
            }
        }

        public void StopFix()
        {
            if (_fixJoint)
            {
                Destroy(_fixJoint);
            }
        }
    }
}
