using System;
using UnityEngine;
using UnityEngine.Events;

namespace RDG.UnityInput {

  // Create an axis [-1, 1] value from two key actions
  // - Returns to zero when disabled
  public class BotActionAxisBeh : MonoBehaviour {
    
    [SerializeField] private UnityEvent<float> onChange;
    
    public float Value { get; private set; }

    public void OnDisable() {
      var oldValue = Value;
      Value = 0;
      CheckChange(oldValue);
    }

    public void OnEnable() {
      SetValue(Value);
    }

    public void OnDestroy() {
      onChange?.RemoveAllListeners();
    }

    private void CheckChange(float oldValue) {
      if (Math.Abs(oldValue - Value) > float.Epsilon) {
        onChange.Invoke(Value);
      }
    }
    
    public void SetValue(float value) {
      if (!isActiveAndEnabled) {
        return;
      }

      var oldValue = Value;
      Value = Mathf.Min(1.0f, Mathf.Max(-1.0f, value));
      CheckChange(oldValue);
    }
    
  }
}
