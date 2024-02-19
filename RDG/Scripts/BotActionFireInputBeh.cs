using RDG.UnityInput;
using UnityEngine;
using UnityEngine.Events;

namespace RDG.UnityInput {
  
  // Fires a unity events for a given key action event (up or down)
  // - does not fire when disabled
  public class BotActionFireInputBeh : MonoBehaviour {
    
    [SerializeField] private UnityEvent onFire;
    
    public void OnDestroy() {
      onFire.RemoveAllListeners();
    }

    public void Fire() {
      if (!isActiveAndEnabled) {
        return;
      }
      
      onFire.Invoke();
    }
  } 
}
