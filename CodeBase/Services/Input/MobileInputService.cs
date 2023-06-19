using UnityEngine;

namespace CodeBase.Services.Input
{
    public class MobileInputService : InputService
    {
        public override Vector2 Axis
        {
            get
            {
                return SimpleInputAxis();
            }
        }


        public override bool IsJumpButton()
        {
            return SimpleInput.GetButtonDown(JumpButton);
        }

        public override bool IsSlideButtonDown()
        {
            return SimpleInput.GetButtonDown(SlideButton);
        }

        public override bool IsSlideButtonUp()
        {
            return SimpleInput.GetButtonUp(SlideButton);
        }
        
        public override bool IsBoostButtonDown()
        {
            return SimpleInput.GetButtonDown(BoostKey);
        }

        public override bool IsBoostButtonUp()
        {
            return SimpleInput.GetButtonUp(BoostKey);
        }

        public override bool IsRopeGunButtonUp()
        {
            return SimpleInput.GetButtonDown(RopeGunKey);
        }
        public override bool IsRopeGunButtonDown()
        {
            return SimpleInput.GetButtonUp(RopeGunKey);
        }
    }
}
