using System;
using UnityEngine;
using UnityEngine.Events;

namespace RDG.UnityInput {

  // Create an axis [-1, 1] value from two key actions
  // - Returns to zero when disabled
  public class BotActionAxis2DBeh : MonoBehaviour {
    
    [SerializeField] private UnityEvent<Vector2> onChange;
    
    public Vector2 Value { get; private set; }

    public void OnDisable() {
      var oldValue = Value;
      Value = Vector2.zero;
      CheckChange(oldValue);
    }

    public void OnEnable() {
      SetValue(Value);
    }

    public void OnDestroy() {
      onChange?.RemoveAllListeners();
    }

    private void CheckChange(Vector2 oldValue) {
      if (Math.Abs(oldValue.x - Value.x) <= float.Epsilon) {
        return;
      }
      
      if (Math.Abs(oldValue.y - Value.y) <= float.Epsilon) {
        return;
      }
      
      onChange.Invoke(Value);
    }
    
    public void SetValue(Vector2 value) {
      if (!isActiveAndEnabled) {
        return;
      }

      var oldValue = Value;
      Value = new Vector2(Mathf.Min(1.0f, Mathf.Max(-1.0f, value.x)), Mathf.Min(1.0f, Mathf.Max(-1.0f, value.y)));
      CheckChange(oldValue);
    }
    
  }
}
