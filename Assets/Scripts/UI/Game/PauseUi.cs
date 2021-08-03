using UnityEngine;

namespace Assets.Scripts.UI.Game
{
    [DisallowMultipleComponent]
    public class PauseUi : MonoBehaviour
    {
        public bool BlockingInput { get; set; } = false;

        public void SendPause()
        {
            if (BlockingInput) return;
            GameManager.Ui!.PauseButtonClicked();
        }
    }
}
