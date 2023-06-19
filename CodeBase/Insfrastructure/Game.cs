using CodeBase.Services.Input;
using UnityEngine;

namespace Assets.CodeBase.Insfrastructure
{
    public class Game
    {
        public static IInputService IInputService;

        public Game()
        {
            RegisterInputService();
        }

        private static void RegisterInputService()
        {
            if (Application.isEditor)
            {
                IInputService = new StandaloneInputService();
            }
            else
            {
                IInputService = new MobileInputService();
            }
        }
    }
}