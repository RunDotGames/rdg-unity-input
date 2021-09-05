using UnityEngine;
using System.Collections.Generic;

namespace RDG.UnityInput {

  [AddComponentMenu("RDG/Input/Key Action Producer")]
  public class KeyActionProducerBeh : MonoBehaviour {

    public KeyActionsSo keyBindings;
    private Dictionary<KeyAction, KeyActionBinding> actionMap;

    public void Start() {
      actionMap = new Dictionary<KeyAction, KeyActionBinding>();
      foreach (var binding in keyBindings.bindings) {
        if (binding.key == KeyCode.None && binding.action == KeyAction.None) { // Actions must have input
          continue;
        }
        if (binding.action == KeyAction.None) { // None may not be bound
          continue;
        }
        actionMap[binding.action] = binding;
      }
    }

    public void Update() {
      if (actionMap == null) {
        return;
      }

      foreach (var binding in actionMap.Values) {
        if (IsActionDown(binding.action)) {
          keyBindings.FireDown(binding.action);
        }

        if (IsActionUp(binding.action)) {
          keyBindings.FireUp(binding.action);
        }
      }
    }

    private bool IsActionDown(KeyAction action) {
      if (!actionMap.TryGetValue(action, out var binding)) {
        return false;
      }

      if (binding.key != KeyCode.None) {
        return Input.GetKeyDown(binding.key);
      }

      return binding.mouseButton != MouseButton.None && Input.GetMouseButtonDown((int)binding.mouseButton);
    }

    private bool IsActionUp(KeyAction action) {
      if (!actionMap.TryGetValue(action, out var binding)) {
        return false;
      }

      if (binding.key != KeyCode.None) {
        return Input.GetKeyUp(binding.key);
      }

      return binding.mouseButton != MouseButton.None && Input.GetMouseButtonUp((int)binding.mouseButton);
    }
  }

}