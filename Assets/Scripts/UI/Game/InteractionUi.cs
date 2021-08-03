using UnityEngine;

namespace Assets.Scripts.UI.Game
{
    [DisallowMultipleComponent]
    public class InteractionUi : MonoBehaviour
    {
        public bool BlockingInput { get; set; } = false;

        public void SendInteraction()
        {
            if (BlockingInput) return;
            GameManager.Ui!.InteractionButtonClicked();
        }
    }
}
