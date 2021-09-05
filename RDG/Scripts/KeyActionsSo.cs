using System;
using System.Collections.Generic;
using UnityEngine;

namespace RDG.UnityInput {


    public enum KeyAction {
        None = -1, MoveLeft = 0, MoveRight = 1, MoveForward = 2,
        MoveBack = 3, Interact = 4, Modify = 5,
    }

    public enum MouseButton {
        None  = -1, Left = 0, Right = 1, Middle = 2,
    }

    [Serializable]
    public class KeyActionBinding {
        public KeyAction action = KeyAction.Interact;
        public KeyCode key = KeyCode.None;

        // Index < 0 indicates binding is keyCode instead
        public MouseButton mouseButton = MouseButton.None;
    }

    [CreateAssetMenu(menuName = "RDG/Input/Key Actions")]
    public class KeyActionsSo : ScriptableObject {
        public List<KeyActionBinding> bindings;

        public event Action<KeyAction> OnDown;
        public event Action<KeyAction> OnUp;

        internal void FireDown(KeyAction action) {
            OnDown?.Invoke(action);
        }
        
        internal void FireUp(KeyAction action) {
            OnUp?.Invoke(action);
        }

    }

}