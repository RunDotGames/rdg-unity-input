using RDG.UnityInput;
using UnityEngine;
using UnityEngine.Events;

namespace RDG.UnityInput {
  
  // Fires a unity events for a given key action event (up or down)
  // - does not fire when disabled
  public class KeyActionFireInputBeh : MonoBehaviour {
    
    [SerializeField] private KeyActionsRegistrySo keyActions;
    [SerializeField] private KeyActionSo action;
    [SerializeField] private UnityEvent onUp;
    [SerializeField] private UnityEvent onDown;
    
    private KeyActionFilter filter;
    
    public void Awake() {
      filter = new KeyActionFilter(keyActions, new []{ action });
      filter.OnDown += (_) => {
        if (!isActiveAndEnabled) {
          return;
        }
        onDown?.Invoke();
      };
      filter.OnUp += (_) => {
        if (!isActiveAndEnabled) {
          return;
        }
        onUp?.Invoke();
      };
    }

    public void OnDestroy() {
      filter?.Release();
    }
  }
}
