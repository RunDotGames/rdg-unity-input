using RDG.UnityInput;
using UnityEngine;
using UnityEngine.Events;

namespace RDG.UnityInput  {
  
  // Raises a unity event while a key action is held
  // - drops when disabled, raises if pressed when enabled
  public class BotActionFlagInputBeh : MonoBehaviour {
    
    [SerializeField] private UnityEvent<bool> onChange;
    
    private bool isFlagged;
    private bool lastFlagged;

    public bool IsFlagged => isFlagged && isActiveAndEnabled;
    
    public void OnDisable() {
      UpdateFlagged();
    }

    public void OnEnable() {
      UpdateFlagged();
    }

    private void UpdateFlagged() {
      var flagState = IsFlagged;
      if (flagState != lastFlagged) {
        onChange?.Invoke(flagState);
      }
      lastFlagged = flagState;
    }

    public void SetFlagged(bool value) {
      isFlagged = value;
      UpdateFlagged();
    }

  }
}
