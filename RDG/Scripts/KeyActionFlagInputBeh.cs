using RDG.UnityInput;
using UnityEngine;
using UnityEngine.Events;

namespace RDG.UnityInput  {
  
  // Raises a unity event while a key action is held
  // - drops when disabled, raises if pressed when enabled
  public class KeyActionFlagInputBeh : MonoBehaviour {
    
    [SerializeField] private KeyActionsRegistrySo keyActions;
    [SerializeField] private KeyActionSo action;
    [SerializeField] private UnityEvent<bool> onChange;
    
    private KeyActionFilter filter;
    
    private bool isFlagged;
    private bool lastFlagged;
    public void Awake() {
      filter = new KeyActionFilter(keyActions, new []{ action });
      filter.OnDown += (_) => {
        isFlagged = true;
        UpdateFlagged();
      };
      filter.OnUp += (_) => {
        isFlagged = false;
        UpdateFlagged();
      };
    }

    public void OnDisable() {
      UpdateFlagged();
    }

    public void OnEnable() {
      UpdateFlagged();
    }

    private void UpdateFlagged() {
      var flagState = IsFlagged();
      if (flagState != lastFlagged) {
        onChange?.Invoke(flagState);
      }
      lastFlagged = flagState;
    }

    public bool IsFlagged() {
      return isFlagged && isActiveAndEnabled;
    }

    public void OnDestroy() {
      filter?.Release();
    }
  }
}
