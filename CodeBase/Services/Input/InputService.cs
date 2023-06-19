using UnityEngine;

namespace CodeBase.Services.Input
{
    public abstract class InputService : IInputService
    {
        protected const string Horizontal = "Horizontal";
        protected const string Vertical = "Vertical";
        protected const string JumpButton = "Jump";
        protected const string SlideButton = "Slide";
        protected const string BoostKey = "BoostKey";
        protected const string RopeGunKey = "RopeGunKey";
        public abstract Vector2 Axis { get; }

        public static Vector2 SimpleInputAxis()
        {
            return new Vector2(SimpleInput.GetAxis(Horizontal), SimpleInput.GetAxis(Vertical));
        }

        public abstract bool IsJumpButton();
        public abstract bool IsSlideButtonDown();

        public abstract bool IsSlideButtonUp();
        public abstract bool IsBoostButtonDown();
        public abstract bool IsBoostButtonUp();
        public abstract bool IsRopeGunButtonUp();
        public abstract bool IsRopeGunButtonDown();
    }
}

