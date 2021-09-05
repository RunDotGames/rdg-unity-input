using System.Collections.Generic;
using System;
using System.Collections;
using System.Diagnostics;

namespace RDG.UnityInput {

  internal class InputPressEvent {
    public readonly KeyAction KeyAction;
    public bool IsPressed;
    public long At;

    public InputPressEvent(KeyAction keyAction) {
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
      public KeyAction Top;

    }

    private static readonly InputPressEventComparator Comparer = new InputPressEventComparator();
    
    private readonly List<InputPressEvent> events = new List<InputPressEvent>();
    private readonly Dictionary<KeyAction, InputPressEvent> eventMap = new Dictionary<KeyAction, InputPressEvent>();
    private readonly KeyActionsSo bindings;

    public event Action<State> OnStackChange;
    public event Action<KeyAction> OnStackTopChange;
    public event Action<int> OnStackSizeChange;

    

    public KeyActionStack(KeyActionsSo bindings) {
      this.bindings = bindings;
      NewKeyActionStack(Enum.GetValues(typeof(KeyAction)));
    }
    
    public KeyActionStack(KeyActionsSo bindings, KeyAction[] keys) {
      this.bindings = bindings;
      NewKeyActionStack(keys);
    }

    private void NewKeyActionStack(IEnumerable keys) {
      bindings.OnDown += HandleActionDown;
      bindings.OnUp += HandleActionUp;
      StackSize = 0;
      foreach (var value in keys) {
        var asAction = (KeyAction)value;
        var keyEvent = new InputPressEvent(asAction);
        events.Add(keyEvent);
        eventMap[asAction] = keyEvent;
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

    private void HandleActionUp(KeyAction action) {
      EvalMostRecent(action, () => {
        eventMap[action].IsPressed = false;
        events.Sort(Comparer);
        StackSize = Math.Max(StackSize - 1, 0);
      });
    }

    private void HandleActionDown(KeyAction action) {
      EvalMostRecent(action, () => {
        eventMap[action].IsPressed = true;
        eventMap[action].At = Stopwatch.GetTimestamp();
        events.Sort(Comparer);
        StackSize++;
      });
      
    }

    private void EvalMostRecent(KeyAction action, Action evaluation) {
      if (!eventMap.ContainsKey(action)) {
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
      
      OnStackChange?.Invoke(new State(){
        Size = currentSize,
        Top = currentTop
      });
      if (priorTop != currentTop) {
        OnStackTopChange?.Invoke(currentTop);
      }
      if (priorSize != currentSize) {
        OnStackSizeChange?.Invoke(StackSize);
      }

    }

    public KeyAction GetTop() {
      return (events?.Count ?? 0) < 1
        ? KeyAction.None
        : ( (events?[0].IsPressed ?? false) ? events[0].KeyAction : KeyAction.None);
    }
    
    public int StackSize { get; private set; }
  }
}