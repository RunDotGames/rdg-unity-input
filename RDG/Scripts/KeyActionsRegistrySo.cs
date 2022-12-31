using System;
using System.Collections.Generic;
using UnityEngine;

namespace RDG.UnityInput {

    public enum MouseButton {
        None  = -1, Left = 0, Right = 1, Middle = 2,
    }

    [Serializable]
    public class KeyActionBinding {
        public KeyActionSo action;
        
        public KeyCode key = KeyCode.None;

        // Index < 0 indicates binding is keyCode instead
        public MouseButton mouseButton = MouseButton.None;
    }

    [CreateAssetMenu(menuName = "RDG/Input/Key Actions Registry")]
    public class KeyActionsRegistrySo : ScriptableObject {
        [SerializeField] public List<KeyActionBinding> registeredBindings;

        public IEnumerable<KeyActionBinding> RegisteredBindings => registeredBindings;

        public event Action<KeyActionSo> OnDown;
        public event Action<KeyActionSo> OnUp;

        internal void FireDown(KeyActionSo action) {
            OnDown?.Invoke(action);
        }
        
        internal void FireUp(KeyActionSo action) {
            OnUp?.Invoke(action);
        }

    }

}