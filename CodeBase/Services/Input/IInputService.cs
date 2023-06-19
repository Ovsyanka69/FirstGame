using UnityEngine;

namespace CodeBase.Services.Input
{
    public interface IInputService
    {
        Vector2 Axis { get; }
        bool IsRopeGunButtonUp();
        bool IsRopeGunButtonDown();

        bool IsJumpButton();
        bool IsSlideButtonDown();
        bool IsSlideButtonUp();
        bool IsBoostButtonDown();
        bool IsBoostButtonUp();
    }
}