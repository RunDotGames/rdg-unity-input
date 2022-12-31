using System;
using System.Collections.Generic;
using System.Linq;

namespace RDG.UnityInput {
  public class KeyActionFilter {
    private readonly KeyActionsRegistrySo bindings;

    public event Action<KeyActionSo> OnUp;
    public event Action<KeyActionSo> OnDown;

    private readonly HashSet<int> allowSet;
    
    public KeyActionFilter(KeyActionsRegistrySo bindings, IEnumerable<KeyActionSo> allow) {
      this.bindings = bindings;
      bindings.OnDown += HandleDown;
      bindings.OnUp += HandleUp;
      allowSet = new HashSet<int>(allow.Select(a => a.GetHashCode()));
    }
    
    public void Release() {
      OnUp = null;
      OnDown = null;
      bindings.OnDown -= HandleDown;
      bindings.OnUp -= HandleUp;
    }
    private void HandleDown(KeyActionSo action) {
      if (!allowSet.Contains(action.GetHashCode())) {
        return;
      }
      
      OnDown?.Invoke(action);
    }

    private void HandleUp(KeyActionSo action) {
      if (!allowSet.Contains(action.GetHashCode())) {
        return;
      }
      
      OnUp?.Invoke(action);
    }

  }
}
