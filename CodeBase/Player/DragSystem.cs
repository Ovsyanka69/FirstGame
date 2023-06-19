using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeBase.Player
{
    public class DragSystem : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private MoveController _moveController;
        
        [Header("Drag System")]
        [SerializeField] private float _groundDrag;
        [SerializeField] private float _airDrag;
        [SerializeField] private float _wallDrag;
        [SerializeField] private float _slopeDrag;
        [SerializeField] private float _ropeDrag;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _moveController = GetComponent<MoveController>();
        }

        private void FixedUpdate()
        {
            DragControl();
        }

        private void DragControl()
        {

            if (_moveController.State is MoveController.MovementState.onFlat or MoveController.MovementState.sliding or MoveController.MovementState.onSlope)
            {
                _rigidbody.drag = _groundDrag;
            }
            else if (_moveController.WallDecectedRight || _moveController.WallDecectedLeft)
            {
                _rigidbody.drag = _wallDrag;
            }
            else if (_moveController.OnSlope)
            {
                _rigidbody.drag = _slopeDrag;
            }
            else if (_moveController.State == MoveController.MovementState.onRope)
            {
                _rigidbody.drag = _ropeDrag;
            }
            else
            {
                _rigidbody.drag = _airDrag;
            }
        }


    }
}