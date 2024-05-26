using System;
using System.Collections.Generic;
using System.Linq;
using RDG.UnityInput;
using UnityEngine;
using UnityEngine.Events;

namespace RDG.UnityInput {

  // Create an axis [-1, 1] value from two key actions (A/S, W/D, <-/->, etc...)
  // - Returns to zero when disabled
  public class KeyActionAxisBeh : MonoBehaviour {
    
    [Header("Config")]
    [SerializeField] private KeyActionsRegistrySo keyActions;
    [SerializeField] private KeyActionSo negAction;
    [SerializeField] private KeyActionSo posAction;
    
    [Header("Events")]
    [SerializeField] private UnityEvent<float> onChange;
    
    public float Value { get; private set; }
    
    private KeyActionStack stack;
    private Dictionary<KeyActionSo, float> axisMap;

    public UnityEvent<float> OnChange => onChange;

    public void Awake() {
      axisMap = new Dictionary<KeyActionSo, float>{
        {
          negAction, -1
        },{
          posAction, 1
        },  
      };
     
      stack = new KeyActionStack(keyActions, axisMap.Keys.ToArray());
      stack.OnStackChange += HandleChange;
    }

    public void OnDisable() {
      var oldValue = Value;
      Value = 0;
      CheckChange(oldValue);
    }

    public void OnEnable() {
      HandleChange(stack.MakeState());
    }

    public void OnDestroy() {
      stack?.Release();
      onChange?.RemoveAllListeners();
    }

    private void CheckChange(float oldValue) {
      if (Math.Abs(oldValue - Value) > float.Epsilon) {
        onChange.Invoke(Value);
      }
    }
    
    private void HandleChange(KeyActionStack.State state) {
      if (!isActiveAndEnabled) {
        return;
      }
      
      var oldValue = Value;
      if (state.Size == 0 || state.Top == null) {
        Value = 0;
      }
      else {
        Value = axisMap[state.Top]; 
      }
      CheckChange(oldValue);
    }
    
  }
}
