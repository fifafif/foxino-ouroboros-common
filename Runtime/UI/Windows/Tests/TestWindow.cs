using UnityEngine;

namespace Ouroboros.Common.UI.Windows
{
    public class TestWindow : Window
    {
        public void OnClickDebugOne()
        {
            Debug.Log("[TestWindow] Click Debug One");
        }

        public void OnClickDebugTwo()
        {
            Debug.Log("[TestWindow] Click Debug Two");
        }
    }
}