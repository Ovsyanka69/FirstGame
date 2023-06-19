using UnityEngine;
using UnityEngine.Serialization;

namespace CodeBase.Player
{
    public class MoveController : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private BoostSystem _boostSystem;
        [SerializeField] RopeGun _ropeGun;
        public MovementState State; // To property
        public MoveDirectionState DirectionState;

        [Header("Move control")] 
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _airSpeedMultiplier;
        [SerializeField] private float _moveSpeedTemp;
        private float moveDirectionX;


        [Header("Crouching")] 
        [SerializeField] private float _crouchYScale;
        [SerializeField] private float _startYScale;
        [SerializeField] private bool _isSliding;


        [Header("JumpControl")] [SerializeField]
        private float _jumpForce;

        [SerializeField] private float _jumpDirectionForce;
        [SerializeField] private float _wallJumpForce;
        [SerializeField] private bool _isDoubleJumpAvailable;
        [SerializeField] private float _doubleJumpForce;


        [Header("Surface Control")] [SerializeField]
        private int _playerHeight = 2;

        [SerializeField] private float _rayExtension;
        [SerializeField] private LayerMask WhatIsGround;
        [SerializeField] private LayerMask WhatIsWall;
        [SerializeField] private LayerMask WhatIsSlope;
        public bool WallDecectedRight { get; private set; }
        public bool WallDecectedLeft { get; private set; }
        [SerializeField] private float Radius;
        public bool Grounded { get; private set; }
        public bool OnSlope { get; private set; }

        [Header("Slope Control")] 
        [SerializeField] private float maxSlopeAngle = 45f;
        public RaycastHit SlopeHit; //как инкапсулировать?
        [SerializeField] private float slopeSpeedCompensator = 10f;

        [Header("Rope Control")] 
        [SerializeField] private float _ropeSpeedMultiplier;
        [SerializeField] private float _ropeJumpForce;
        [SerializeField] private float _kickForce;
        [SerializeField] private float _lastSpeed;


        public enum MoveDirectionState
        {
            toTheRight,
            toTheLeft
        }

        public enum MovementState
        {
            onFlat,
            sliding,
            air,
            onWall,
            onSlope,
            onRope
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _boostSystem = GetComponent<BoostSystem>();
            _startYScale = transform.localScale.y;
        }

        private void Update()
        {
            GroundCheck();
            SlopeCheck();
            StateHandler();
            SpeedControl();

            if (Grounded)
            {
                if (_rigidbody.velocity.x is > Assets.CodeBase.Constants.Epsilon
                    or < -Assets.CodeBase.Constants.Epsilon)
                {
                    _moveSpeedTemp = _rigidbody.velocity.x;
                }
            }

            if (!Grounded || !OnSlope)
            {
                WallDecectedRight = Physics.Raycast(transform.position, Vector3.right, Radius, WhatIsWall);
                WallDecectedLeft = Physics.Raycast(transform.position, -Vector3.right, Radius, WhatIsWall);
            }
            else
            {
                WallDecectedRight = false;
                WallDecectedLeft = false;
            }
        }

        public void StateHandler()
        {
            _lastSpeed = _rigidbody.velocity.x;

            if (moveDirectionX > 0)
            {
                DirectionState = MoveDirectionState.toTheRight;
            }
            else
            {
                DirectionState = MoveDirectionState.toTheLeft;
            }

            if (Grounded)
            {
                State = MovementState.onFlat;
                _rigidbody.useGravity = false;
            }

            if (!Grounded && !WallDecectedLeft && !WallDecectedRight &&
                _ropeGun.CurrentRopeState != RopeGun.RopeState.active)
            {
                State = MovementState.air;
                _rigidbody.useGravity = true;
            }

            if (_ropeGun.CurrentRopeState == RopeGun.RopeState.active)
            {
                State = MovementState.onRope;
            }

            if (!Grounded)
            {
                if (WallDecectedLeft || WallDecectedRight)
                {
                    State = MovementState.onWall;
                    _rigidbody.useGravity = true;
                }
            }

            if (_isSliding)
            {
                State = MovementState.sliding;
            }

            if (OnSlope)
            {
                State = MovementState.onSlope;
                _rigidbody.useGravity = false;
            }
        }

        public void AnyJump()
        {
            if (Grounded || WallDecectedRight || WallDecectedLeft || OnSlope || State == MovementState.onRope)
            {
                if (!Grounded && WallDecectedLeft)
                {
                    State = MovementState.air;
                    _rigidbody.AddForce(new Vector3(1 * _jumpDirectionForce, 1 * _wallJumpForce, 0), ForceMode.Impulse);
                    _isDoubleJumpAvailable = true;
                }

                else if (!Grounded && WallDecectedRight)
                {
                    State = MovementState.air;
                    _rigidbody.AddForce(new Vector3(-1 * _jumpDirectionForce, 1 * _wallJumpForce, 0),
                        ForceMode.Impulse);
                    _isDoubleJumpAvailable = true;
                }
                else if (State == MovementState.onRope)
                {
                    State = MovementState.air;
                    _rigidbody.AddForce(Vector3.up * _ropeJumpForce, ForceMode.Impulse);
                    _isDoubleJumpAvailable = true;
                }
                else
                {
                    State = MovementState.air;
                    _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
                    _isDoubleJumpAvailable = true;
                }
            }
            else if (!Grounded && _isDoubleJumpAvailable)
            {
                State = MovementState.air;
                _rigidbody.AddForce(Vector3.up * _doubleJumpForce, ForceMode.Impulse);
                _isDoubleJumpAvailable = false;
            }
        }


        public void Move(float moveDirection)
        {
            moveDirectionX = moveDirection;
            if (State == MovementState.onSlope)
            {
                _rigidbody.AddForce(
                    GetSlopeMoveDirection(moveDirection) * (_moveSpeed * slopeSpeedCompensator *
                                                             _boostSystem.SpeedBoost * _boostSystem.SlopeBoost *
                                                             _boostSystem._boostBoxBoost * _boostSystem._ropeBoostValue), ForceMode.VelocityChange);
            }

            if (State == MovementState.onFlat)
            {
                _rigidbody.AddForce(
                    moveDirection * _moveSpeed * _boostSystem.SpeedBoost * _boostSystem.SlopeBoost *
                    _boostSystem._boostBoxBoost * _boostSystem._ropeBoostValue, 0f, 0f, ForceMode.VelocityChange);
            }

            if (State == MovementState.air || State == MovementState.onWall)
            {
                _rigidbody.AddForce(
                    moveDirection * _moveSpeed * _boostSystem.SpeedBoost * _boostSystem.SlopeBoost *
                    _airSpeedMultiplier * _boostSystem._boostBoxBoost  * _boostSystem._ropeBoostValue, 0f, 0f, ForceMode.VelocityChange);
            }

            if (State == MovementState.onRope)
            {
                _rigidbody.AddForce(
                    moveDirection * _moveSpeed * _boostSystem.SpeedBoost * _boostSystem.SlopeBoost *
                    _ropeSpeedMultiplier * _boostSystem._boostBoxBoost * _boostSystem._ropeBoostValue, 0f, 0f, ForceMode.VelocityChange);
            }

            if (_ropeGun.KickNeed)
            {
                _rigidbody.AddForce(moveDirection * _moveSpeed * _kickForce, 0f, 0f, ForceMode.Impulse);
    

            }
        }

        public void Slide()
        {
            _isSliding = true;
            transform.localScale = new Vector3(transform.localScale.x, _crouchYScale, transform.localScale.z);
            if (Grounded)
            {
                _rigidbody.AddForce(Vector3.down * 30f, ForceMode.Impulse);
            }
        }

        public void GetUp()
        {
            _isSliding = false;
            transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
        }

        private void SpeedControl()
        {
            _airSpeedMultiplier = 0.1f;
            if (State == MovementState.air)
            {
                if (_rigidbody.velocity.x > 10 && _rigidbody.velocity.x > _moveSpeedTemp * 1.2f)
                {
                    _airSpeedMultiplier = 0f;
                }
                else if (_rigidbody.velocity.x < -10 && _rigidbody.velocity.x < _moveSpeedTemp * 1.2f)
                {
                    _airSpeedMultiplier = 0f;
                }
            }
        }

        private bool GroundCheck() // Сделать "cayot time"
        {
            bool groundFound = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + _rayExtension,
                WhatIsGround);

            if (groundFound)
            {
                Grounded = true;
                return Grounded;
            }
            else
            {
                Grounded = false;
                return Grounded;
            }
        }

        public bool SlopeCheck()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out SlopeHit, _playerHeight * 0.5f + _rayExtension,
                    WhatIsSlope))
            {
                float angle = Vector3.Angle(Vector3.up, SlopeHit.normal);
                OnSlope = angle < maxSlopeAngle && angle != 0;
                return OnSlope;
            }
            else
            {
                OnSlope = false;
                return OnSlope;
            }
        }

        private Vector3 GetSlopeMoveDirection(float moveDirection)
        {
            return (Vector3.ProjectOnPlane(new Vector3(moveDirection, 0f, 0f), SlopeHit.normal)).normalized;
        }
    }
}