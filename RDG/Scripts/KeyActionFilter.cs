using System;
using System.Collections.Generic;

namespace RDG.UnityInput {
  public class KeyActionFilter {
    private readonly KeyActionsSo bindings;

    public event Action<KeyAction> OnUp;
    public event Action<KeyAction> OnDown;

    private readonly HashSet<KeyAction> allowSet;
    
    public KeyActionFilter(KeyActionsSo bindings, IEnumerable<KeyAction> allow) {
      this.bindings = bindings;
      bindings.OnDown += HandleDown;
      bindings.OnUp += HandleUp;
      allowSet = new HashSet<KeyAction>(allow);
    }
    
    public void Release() {
      OnUp = null;
      OnDown = null;
      bindings.OnDown -= HandleDown;
      bindings.OnUp -= HandleUp;
    }
    private void HandleDown(KeyAction action) {
      if (!allowSet.Contains(action)) {
        return;
      }
      
      OnDown?.Invoke(action);
    }

    private void HandleUp(KeyAction action) {
      if (!allowSet.Contains(action)) {
        return;
      }
      
      OnUp?.Invoke(action);
    }

  }
}
