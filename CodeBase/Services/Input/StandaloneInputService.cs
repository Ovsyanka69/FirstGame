using UnityEngine;

namespace CodeBase.Services.Input
{
    public class StandaloneInputService : InputService
    {
        public KeyCode JumpKey = KeyCode.Space;
        public KeyCode CrouchKey = KeyCode.LeftControl;
        public KeyCode BoostKey = KeyCode.LeftShift;
        public KeyCode RopeGunKey = KeyCode.E;

        public override Vector2 Axis
        {
            get
            {
                Vector2 axis = SimpleInputAxis();
                if (axis == Vector2.zero)
                {
                    axis = UnityInputAxis();
                }
                return axis;
            }
        }

        private static Vector2 UnityInputAxis()
        {
            return new Vector2(UnityEngine.Input.GetAxis(Horizontal), UnityEngine.Input.GetAxis(Vertical));
        }

        public override bool IsJumpButton()
        {
            return UnityEngine.Input.GetKeyDown(JumpKey);
        }

        public override bool IsSlideButtonDown()
        {
            return UnityEngine.Input.GetKeyDown(CrouchKey);
        }

        public override bool IsSlideButtonUp()
        {
            return UnityEngine.Input.GetKeyUp(CrouchKey);
        }
        
        public override bool IsBoostButtonDown()
        {
            return UnityEngine.Input.GetKeyDown(BoostKey);
        }

        public override bool IsBoostButtonUp()
        {
            return UnityEngine.Input.GetKeyUp(BoostKey);
        }

        public override bool IsRopeGunButtonUp()
        {
            return UnityEngine.Input.GetKeyUp(RopeGunKey);
        }
        public override bool IsRopeGunButtonDown()
        {
            return UnityEngine.Input.GetKeyDown(RopeGunKey);
        }
    }
}