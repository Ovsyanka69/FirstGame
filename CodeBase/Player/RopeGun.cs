using System;
using CodeBase.HookMachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeBase.Player
{
    public class RopeGun : MonoBehaviour
    {
        [SerializeField] private BoostSystem _boostSystem;
        [SerializeField] private MoveController _moveController;
        [SerializeField] private RopeRenderer _ropeRenderer;
        [SerializeField] private Transform _ropeGun;
        [SerializeField] private Hook _hook;
        [SerializeField] private float _speed;
        private SpringJoint _springJoint;
        [SerializeField] private float _springValue = 100f;
        [SerializeField] private float _damperValue = 5f;
        [SerializeField] private float _maxDistance = 30f;
        private float _ropeLength;

        public RopeState CurrentRopeState;

        public enum RopeState
        {
            active,
            disable,
            fly
        }

        public bool KickNeed { get; set; }

        private void Start()
        {
            _moveController = GetComponent<MoveController>();
        }

        private void Update()
        {
            if (CurrentRopeState == RopeState.fly)
            {
                float distance = Vector3.Distance(transform.position, _hook.transform.position);
                if (distance > _maxDistance)
                {
                    _hook.gameObject.SetActive(false);
                    CurrentRopeState = RopeState.disable;
                    _ropeRenderer.Hide();
                }
            }
            
            if (CurrentRopeState is RopeState.fly or RopeState.active)
            {
                _ropeRenderer.DrawLine(transform.position, _hook.transform.position, _ropeLength);
            }

        }

        public void RopeDeactivate()
        {
            _hook.gameObject.SetActive(false);
            CurrentRopeState = RopeState.disable;
            _ropeRenderer.Hide();
            KickNeed = true;

        }

        public void Shot()
        {
            _ropeLength = 1f;
            if (_springJoint)
            {
                Destroy(_springJoint);
            }
            _hook.gameObject.SetActive(true);
            CurrentRopeState = RopeState.fly;

            _hook.StopFix();
            _hook.transform.position = transform.position;
            _hook.transform.rotation = transform.rotation;

            if (_moveController.DirectionState == MoveController.MoveDirectionState.toTheRight)
            {
                _hook.Rigidbody.velocity = new Vector3(1,1,0) * _speed;
            }
            else
            {
                _hook.Rigidbody.velocity = new Vector3(-1,1,0) * _speed;
            }
        }
        public void CreateSpring()
        {
            if (_springJoint == null)
            {
                _springJoint = gameObject.AddComponent<SpringJoint>();
                _springJoint.connectedBody = _hook.Rigidbody;
                _springJoint.anchor = _ropeGun.localPosition;
                _springJoint.autoConfigureConnectedAnchor = false;
                _springJoint.connectedAnchor = Vector3.zero;
                _springJoint.spring = _springValue;
                _springJoint.damper = _damperValue;
                _ropeLength = Vector3.Distance(transform.position, _hook.transform.position);
                if (_moveController.Grounded)
                {
                    _ropeLength -= 2f;
                }
                _springJoint.maxDistance = _ropeLength;
                CurrentRopeState = RopeState.active;
                _moveController.State = MoveController.MovementState.onRope;
            }
        }
        public void DestroySpring()
        {
            if (_springJoint)
            {
                Destroy(_springJoint);
                CurrentRopeState = RopeState.disable;
                _ropeRenderer.Hide();
            }
        }

    }
}