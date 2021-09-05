using System;
using UnityEngine;
using UnityEngine.UI;

namespace RDG.UnityInput {
  public class InputKeyActionDemoBeh : MonoBehaviour {
    public KeyActionsSo bindings;
    public Text movementDisplay;
    public Text interactDisplay;

    private KeyActionStack movementStack;
    private KeyActionFilter interactFilter;
    
    public void Start() {
      interactFilter = new KeyActionFilter(bindings, new[]{
        KeyAction.Interact
      });
      interactFilter.OnUp += HandleUp;
      interactFilter.OnDown += HandleDown;
      
      movementStack = new KeyActionStack(bindings, new[]{
        KeyAction.MoveLeft, KeyAction.MoveRight
      });
      movementStack.OnStackChange += HandleMovementChange;
      
      HandleUp(KeyAction.Interact);
      HandleMovementChange(new KeyActionStack.State{Size = 0, Top = KeyAction.None});
    }

    public void OnDestroy() {
      movementStack.Release();
      interactFilter.Release();
    }

    private void HandleUp(KeyAction action) {
      interactDisplay.text = "Not Interacting";
    }

    private void HandleDown(KeyAction action) {
      interactDisplay.text = "Interacting";
    }

    private void HandleMovementChange(KeyActionStack.State state) {
      if (state.Top == KeyAction.None || state.Size > 1) {
        movementDisplay.text = "Not Moving";
        return;
      }
      
      movementDisplay.text = "Movement: " + state.Top;
    }

  }
}
