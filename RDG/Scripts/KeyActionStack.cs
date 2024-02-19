using System.Collections.Generic;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace RDG.UnityInput {

  internal class InputPressEvent {
    public readonly KeyActionSo KeyAction;
    public bool IsPressed;
    public long At;

    public InputPressEvent(KeyActionSo keyAction) {
      KeyAction = keyAction;
      At = long.MinValue;
    }
  }

  internal class InputPressEventComparator : IComparer<InputPressEvent> {
    public int Compare(InputPressEvent x, InputPressEvent y) {
      if ((x?.IsPressed ?? false) != (y?.IsPressed ?? false)) {
        return (x?.IsPressed ?? false) ? -1 : 1;
      }
      
      var dif = (y?.At ?? long.MaxValue) - (x?.At ?? long.MaxValue);
      return dif == 0 ? 0 : Math.Sign(dif);
    }
  }
  public class KeyActionStack {
    
    public class State {
      public int Size;
      public KeyActionSo Top;

    }

    private static readonly InputPressEventComparator Comparer = new InputPressEventComparator();
    
    private readonly List<InputPressEvent> events = new List<InputPressEvent>();
    private readonly Dictionary<int, InputPressEvent> eventMap = new Dictionary<int, InputPressEvent>();
    private readonly KeyActionsRegistrySo bindings;

    public event Action<State> OnStackChange;
    public event Action<KeyActionSo> OnStackTopChange;
    public event Action<int> OnStackSizeChange;

    

    public KeyActionStack(KeyActionsRegistrySo bindings) {
      this.bindings = bindings;
      NewKeyActionStack(bindings.RegisteredBindings.Select(b => b.action));
    }
    
    public KeyActionStack(KeyActionsRegistrySo bindings, IEnumerable<KeyActionSo> keys) {
      this.bindings = bindings;
      NewKeyActionStack(keys);
    }

    private void NewKeyActionStack(IEnumerable<KeyActionSo> keys) {
      bindings.OnDown += HandleActionDown;
      bindings.OnUp += HandleActionUp;
      StackSize = 0;
      foreach (var value in keys) {
        var keyEvent = new InputPressEvent(value);
        var hash = value.GetHashCode();
        events.Add(keyEvent);
        eventMap[hash] = keyEvent;
      }
      
    }

    public void Release() {
      OnStackChange = null;
      OnStackSizeChange = null;
      OnStackTopChange = null;
      if (bindings == null) {
        return;
      }
      
      bindings.OnDown -= HandleActionDown;
      bindings.OnUp -= HandleActionUp;
    }

    private void HandleActionUp(KeyActionSo action) {
      EvalMostRecent(action, () => {
        eventMap[action.GetHashCode()].IsPressed = false;
        events.Sort(Comparer);
        StackSize = Math.Max(StackSize - 1, 0);
      });
    }

    private void HandleActionDown(KeyActionSo action) {
      EvalMostRecent(action, () => {
        eventMap[action.GetHashCode()].IsPressed = true;
        eventMap[action.GetHashCode()].At = Stopwatch.GetTimestamp();
        events.Sort(Comparer);
        StackSize++;
      });
      
    }

    private void EvalMostRecent(KeyActionSo action, Action evaluation) {
      if (!eventMap.ContainsKey(action.GetHashCode())) {
        return;
      }
      
      var priorTop = GetTop();
      var priorSize = StackSize;
      evaluation();
      var currentTop = GetTop();
      var currentSize = StackSize;
      if (currentSize == priorSize && currentTop == priorTop) {
        return;
      }
      
      OnStackChange?.Invoke(MakeState());
      if (priorTop != currentTop) {
        OnStackTopChange?.Invoke(currentTop);
      }
      if (priorSize != currentSize) {
        OnStackSizeChange?.Invoke(StackSize);
      }

    }

    public State MakeState() {
      return new State(){
        Size = StackSize,
        Top = GetTop()
      };
    }

    public KeyActionSo GetTop() {
      return (events?.Count ?? 0) < 1
        ? null
        : ( (events?[0].IsPressed ?? false) ? events[0].KeyAction : null);
    }
    
    public int StackSize { get; private set; }
  }
}