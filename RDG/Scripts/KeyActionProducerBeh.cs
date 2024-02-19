using UnityEngine;
using System.Collections.Generic;

namespace RDG.UnityInput {

  [AddComponentMenu("RDG/Input/Key Action Producer")]
  public class KeyActionProducerBeh : MonoBehaviour {

    public KeyActionsRegistrySo keyBindings;
    private Dictionary<int, KeyActionBinding> actionMap;

    public void Awake() {
      actionMap = new Dictionary<int, KeyActionBinding>();
      foreach (var binding in keyBindings.registeredBindings) {
        if (binding.key == KeyCode.None && binding.mouseButton == MouseButton.None) { // Actions must have input trigger
          continue;
        }
        if (binding.action == null) { // binding must have action
          continue;
        }
        actionMap[binding.action.GetHashCode()] = binding;
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

    private bool IsActionDown(KeyActionSo action) {
      if (!actionMap.TryGetValue(action.GetHashCode(), out var binding)) {
        return false;
      }

      if (binding.key != KeyCode.None) {
        return Input.GetKeyDown(binding.key);
      }

      return binding.mouseButton != MouseButton.None && Input.GetMouseButtonDown((int)binding.mouseButton);
    }

    private bool IsActionUp(KeyActionSo action) {
      if (!actionMap.TryGetValue(action.GetHashCode(), out var binding)) {
        return false;
      }

      if (binding.key != KeyCode.None) {
        return Input.GetKeyUp(binding.key);
      }

      return binding.mouseButton != MouseButton.None && Input.GetMouseButtonUp((int)binding.mouseButton);
    }
  }

}