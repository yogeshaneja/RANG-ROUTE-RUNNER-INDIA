using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace IndianRunner
{
    public sealed class RunnerInput
    {
        public int Horizontal { get; private set; }
        public bool Jump { get; private set; }
        public bool Slide { get; private set; }
        public bool Pause { get; private set; }
        private Vector2 start;
        private bool tracking;
        public void ResetGesture() { tracking = false; }
        public void Read(float usableTop)
        {
            Horizontal = 0; Jump = Slide = Pause = false;
#if ENABLE_INPUT_SYSTEM
            var k = Keyboard.current;
            if (k != null)
            {
                if (k.leftArrowKey.wasPressedThisFrame || k.aKey.wasPressedThisFrame) Horizontal = -1;
                if (k.rightArrowKey.wasPressedThisFrame || k.dKey.wasPressedThisFrame) Horizontal = 1;
                Jump = k.upArrowKey.wasPressedThisFrame || k.wKey.wasPressedThisFrame || k.spaceKey.wasPressedThisFrame;
                Slide = k.downArrowKey.wasPressedThisFrame || k.sKey.wasPressedThisFrame;
                Pause = k.escapeKey.wasPressedThisFrame || k.pKey.wasPressedThisFrame;
            }
            var touch = Touchscreen.current;
            if (touch != null && (touch.primaryTouch.press.isPressed || touch.primaryTouch.press.wasReleasedThisFrame))
            {
                var t = touch.primaryTouch;
                if (t.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Canceled) tracking = false;
                else Pointer(t.position.ReadValue(), t.press.wasPressedThisFrame, t.press.wasReleasedThisFrame, usableTop);
            }
            else if (Mouse.current != null)
            {
                var m = Mouse.current;
                Pointer(m.position.ReadValue(), m.leftButton.wasPressedThisFrame, m.leftButton.wasReleasedThisFrame, usableTop);
            }
#elif ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) Horizontal = -1;
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) Horizontal = 1;
            Jump = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space);
            Slide = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
            Pause = Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P);
            if (Input.touchCount > 0)
            {
                var t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Canceled) tracking = false;
                else Pointer(t.position,t.phase == TouchPhase.Began,t.phase == TouchPhase.Ended,usableTop);
            }
            else Pointer(Input.mousePosition, Input.GetMouseButtonDown(0), Input.GetMouseButtonUp(0), usableTop);
#endif
        }
        private void Pointer(Vector2 p, bool began, bool ended, float usableTop)
        {
            if (began) { start = p; tracking = p.y < usableTop; }
            if (!tracking) return;
            Vector2 delta = p - start;
            if (delta.magnitude < Mathf.Max(24f,Screen.width*.035f))
            {
                if (ended) tracking = false;
                return;
            }
            // Commit once on crossing the threshold, not on finger release.
            tracking = false;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) Horizontal = delta.x > 0 ? 1 : -1;
            else { Jump = delta.y > 0; Slide = delta.y < 0; }
        }
    }
}
