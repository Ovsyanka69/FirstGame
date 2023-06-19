using System;
using System.Collections;
using System.Threading;
using Assets.CodeBase.Insfrastructure;
using CodeBase.Boosters_Loot;
using CodeBase.Services.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeBase.Player
{
    public class BoostSystem : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private MoveController _moveController;
        private RopeGun _ropeGun;

        [Header("Sprint Boost")] 
        public float SpeedBoost = 1f; // в свойство после теста
        [SerializeField] private float _sprintBoostIncrement = 0.1f;
        [SerializeField] private float _sprintBoostPeriod = 0.2f;
        [SerializeField] private float _speedTheshold = 9f;
        private float _timer;

        [Header("Slope Boost")] 
        public float SlopeBoost = 1f; // в свойство после теста
        [SerializeField] private float _slopeBoostDecrement = 0.007f;
        [SerializeField] private bool _slopeBoosted = false;
        [SerializeField] private float _lastFallSpeed;
        [SerializeField] float _FallOnSlopeDenominator;
        
        [Header("Boost Box")] 
        [SerializeField] private float _boostBoxEnergy = 0f;
        [SerializeField] private float _boostValueToAdd = 50f;
        public float _boostBoxBoost = 3f; // в свойство после теста
        [SerializeField] private float _boostBoxDecrement;
        [SerializeField] private float _boostBoxBoostValue;
        
        [Header("Rope Boost")] 
        public float _ropeBoostValue; // в свойство после теста
        [SerializeField] private float _ropeBoostDecrement;
        [SerializeField] float _ropeBoostDenominator;
        private bool _ropeBoosted;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _moveController = GetComponent<MoveController>();
            _ropeGun = GetComponent<RopeGun>();
        }

        private void Update()
        {
            _lastFallSpeed = FallSpeedRecorder();
        }

        private void FixedUpdate()
        {
            FallOnSlopeBoost();
            SprintBoost();
            RopeBoost();
        }

        private void SprintBoost()
        {
            _timer += Time.fixedDeltaTime;
            if (_rigidbody.velocity.magnitude >= _speedTheshold || _rigidbody.velocity.magnitude <= -_speedTheshold)
            {
                if (_timer > _sprintBoostPeriod)
                {
                    if (SpeedBoost >= 2f)
                    {
                        SpeedBoost = 2f;
                    }

                    _timer = 0;
                    SpeedBoost += _sprintBoostIncrement;
                }
            }

            if (_rigidbody.velocity.magnitude < 5 && _rigidbody.velocity.magnitude > -5)
            {
                _timer = 0;
                SpeedBoost = 1f;
            }
        }

        private float FallSpeedRecorder()
        {
            if (_rigidbody.velocity.y < -30)
            {
                return -_rigidbody.velocity.y;
            }
            else
            {
                if (_moveController.State == MoveController.MovementState.onFlat)
                {
                    return 1;
                }

                return _lastFallSpeed;
            }
        }

        private void FallOnSlopeBoost()
        {
            if (!_slopeBoosted)
            {
                if (_moveController.State == MoveController.MovementState.onSlope && _lastFallSpeed > 30)
                {
                    float angle = Vector3.Angle(Vector3.up, _moveController.SlopeHit.normal);
                    SlopeBoost = 1 + (_lastFallSpeed * angle) / _FallOnSlopeDenominator;
                    StartCoroutine(FallOnSlopeBoostCoroutine());
                }
            }
        }

        IEnumerator FallOnSlopeBoostCoroutine()
        {
            _lastFallSpeed = 1f;
            _slopeBoosted = true;
            while (SlopeBoost > 1)
            {
                SlopeBoost -= _slopeBoostDecrement;
                if (_rigidbody.velocity.magnitude < _speedTheshold && _rigidbody.velocity.magnitude > -_speedTheshold)
                {
                    SlopeBoost = 1;
                }

                yield return new WaitForSeconds(0.02f);
            }

            if (SlopeBoost <= 1f)
            {
                SlopeBoost = 1f;
            }

            _slopeBoosted = false;
            yield return null;
        }

        public void AddBoostEnergy()
        {
            _boostBoxEnergy += _boostValueToAdd;
            if (_boostBoxEnergy > 100f)
            {
                _boostBoxEnergy = 100f;
            }
        }

        public void Boost()
        {
            StartCoroutine("BoostKeyCoroutine");
        }

        public void BoostEnd()
        {
            _boostBoxBoost = 1f;
            StopCoroutine("BoostKeyCoroutine");
        }

        private IEnumerator BoostKeyCoroutine()
        {
            while (_boostBoxEnergy > 0)
            {
                _boostBoxBoost = _boostBoxBoostValue;
                _boostBoxEnergy -= _boostBoxDecrement;
                yield return new WaitForSeconds(0.5f);
            }

            if (_boostBoxEnergy <= 0)
            {
                _boostBoxBoost = 1f;
            }
        }
        
        private void RopeBoost()
        {
            if (_ropeGun.KickNeed && !_ropeBoosted)
            {
                _ropeBoosted = true;
                _ropeBoostValue = 1 + 0.2f + _lastFallSpeed/ _ropeBoostDenominator;
                _ropeGun.KickNeed = false;
                if (_ropeBoosted)
                {
                    StopCoroutine(RopeBoostCoroutine());
                }
                StartCoroutine(RopeBoostCoroutine());
            }

            if (_ropeGun.KickNeed && _ropeBoosted)
            {
                _ropeBoostValue += 0.1f + _lastFallSpeed/ _ropeBoostDenominator;
                _ropeGun.KickNeed = false;
                StopCoroutine(RopeBoostCoroutine());
                StartCoroutine(RopeBoostCoroutine());
            }
        }
        
        private IEnumerator RopeBoostCoroutine()
        {
            _lastFallSpeed = 1f;
            while (_ropeBoostValue > 1)
            {
                _ropeBoostValue -= _ropeBoostDecrement;
                yield return new WaitForSeconds(0.02f);
            }

            if (_ropeBoostValue <= 1f)
            {
                _ropeBoostValue = 1f;
            }

            _ropeBoosted = false;
            yield return null;
        }
    


    private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Booster>())
            {
                AddBoostEnergy();
            }
        }
    }
}