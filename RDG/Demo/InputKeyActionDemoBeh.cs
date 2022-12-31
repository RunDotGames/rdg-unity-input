using System;
using UnityEngine;
using UnityEngine.UI;

namespace RDG.UnityInput {
  public class InputKeyActionDemoBeh : MonoBehaviour {
    public KeyActionsRegistrySo bindings;
    public KeyActionSo interact;
    public KeyActionSo moveLeft;
    public KeyActionSo moveRight;
    
    public Text movementDisplay;
    public Text interactDisplay;

    private KeyActionStack movementStack;
    private KeyActionFilter interactFilter;
    
    public void Start() {
      interactFilter = new KeyActionFilter(bindings, new[]{
        interact
      });
      interactFilter.OnUp += HandleUp;
      interactFilter.OnDown += HandleDown;
      
      movementStack = new KeyActionStack(bindings, new[]{
        moveLeft, moveRight
      });
      movementStack.OnStackChange += HandleMovementChange;
      
      HandleUp(interact);
      HandleMovementChange(new KeyActionStack.State{Size = 0, Top = null});
    }

    public void OnDestroy() {
      movementStack.Release();
      interactFilter.Release();
    }

    private void HandleUp(KeyActionSo action) {
      interactDisplay.text = "Not Interacting";
    }

    private void HandleDown(KeyActionSo action) {
      interactDisplay.text = "Interacting";
    }

    private void HandleMovementChange(KeyActionStack.State state) {
      if (state.Top == null || state.Size > 1) {
        movementDisplay.text = "Not Moving";
        return;
      }
      
      movementDisplay.text = "Movement: " +  ((state.Top == moveLeft) ? "Left" : "Right");
    }

  }
}
