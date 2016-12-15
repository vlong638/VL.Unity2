using UnityEngine;

namespace Assets.VL.VL.Unity.Core.Utilities
{
    public class InputHelper
    {
        public static float GetHorizontal()
        {
            return Input.GetAxis("Horizontal");
        }
        public static float GetVertical()
        {
            return Input.GetAxis("Vertical");
        }
    }
}
