using Assets.CodeBase;
using Assets.CodeBase.Insfrastructure;
using CodeBase.Services.Input;
using UnityEngine;

namespace CodeBase.Player
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private MoveController MoveController;
        [SerializeField] private BoostSystem _boostSystem;
        [SerializeField] private RopeGun _ropeGun;
        private IInputService _inputService;
        private Vector3 _movementVector;
        private bool JumpKeyDown;

        private void Awake()
        {
            _inputService = Game.IInputService;
        }

        private void Update()
        {
            if (_inputService.IsJumpButton())
            {
                JumpKeyDown = true;
            }

            if (_inputService.IsSlideButtonDown())
            {
                MoveController.Slide();
            }
            if (_inputService.IsSlideButtonUp())
            {
                MoveController.GetUp();
            }

            if (_inputService.IsRopeGunButtonDown())
            {
                _ropeGun.Shot();
            }

            if (_inputService.IsRopeGunButtonUp())
            {
                _ropeGun.RopeDeactivate();
            }

            if (_inputService.IsBoostButtonDown())
            {
                _boostSystem.Boost();
            }
            
            if (_inputService.IsBoostButtonUp())
            {
                _boostSystem.BoostEnd();
            }

            _movementVector = Vector3.zero;
            if (_inputService.Axis.sqrMagnitude > Constants.Epsilon)
            {
                _movementVector = _inputService.Axis;
                _movementVector.Normalize();
            }
            else
            {
                _movementVector = Vector3.zero;
            }
        }
        

        private void FixedUpdate()
        {
            MoveController.Move(_movementVector.x * 51 * Time.fixedDeltaTime);
            if (JumpKeyDown)
            {
                MoveController.AnyJump();
                _ropeGun.DestroySpring();
                JumpKeyDown = false;
            }

        }
    }

    
}