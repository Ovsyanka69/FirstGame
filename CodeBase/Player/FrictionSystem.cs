using UnityEngine;

namespace CodeBase.Player
{
    public class FrictionSystem : MonoBehaviour
    {
        private MoveController _moveController;
        [SerializeField] private Collider _playerCollider;
        private void Start()
        {
            _moveController = GetComponent<MoveController>();
        }

        private void Update()
        {
            if (_moveController.State is MoveController.MovementState.onSlope or MoveController.MovementState.onWall)
            {
                PhysicMaterial material = _playerCollider.material;
                material.dynamicFriction = 0.6f;
                material.staticFriction = 0.6f;
                material.frictionCombine = PhysicMaterialCombine.Average;
            }
            else
            {
                PhysicMaterial material = _playerCollider.material;
                material.dynamicFriction = 0f;
                material.staticFriction = 0f;
                material.frictionCombine = PhysicMaterialCombine.Minimum;
            }

        }
    }
}