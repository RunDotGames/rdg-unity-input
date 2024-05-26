using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RDG.UnityInput {
  interface GesturePlatform {
    void Update();
    void Release();
  }

  [AddComponentMenu("RDG/Input/Gesture Producer")]
  public class GestureProducerBeh : MonoBehaviour {
    private static readonly HashSet<RuntimePlatform> MousePlatforms = new HashSet<RuntimePlatform>{
      RuntimePlatform.WindowsEditor,
      RuntimePlatform.WindowsPlayer,
      RuntimePlatform.WebGLPlayer,
    };

    public GesturesSo gestures;
    public EventSystem eventSystem;

    private GesturePlatform platform;

    private GestureAction currentAction;
    
    public GestureState State { get; private set; }

    public void Awake() {
      currentAction = GestureAction.Idle;
      State = new GestureState(){
        Zoom = new GestureZoomState(),
        Drag = new GestureDragState(),
        Twist = new GestureTwistState(),
        Tap = new GestureTapState(),
      };

      if (MousePlatforms.Contains(Application.platform)) {
        platform = new GesturePlatformMouse(gestures.config.mouse, State, eventSystem);
        State.PlatformName = "Mouse";
        return;
      }

      platform = new GesturePlatformTouch(State, gestures.config.touch);
      State.PlatformName = "Touch";

    }

    public void OnDestroy() {
      platform?.Release();
    }

    private void InvokeStops(GestureAction lastAction) {
      switch (lastAction) {
        case GestureAction.Tap:
          break;
        case GestureAction.Idle:
          break;
        case GestureAction.Dragging:
          gestures.FireDragStop(State.Drag);
          break;
        case GestureAction.Zooming:
          gestures.FireZoomStop(State.Zoom);
          break;
        case GestureAction.Twisting:
          gestures.FireTwistStop(State.Twist);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private void InvokeContinues() {
      switch (currentAction) {
        case GestureAction.Tap:
          break;
        case GestureAction.Idle:
          break;
        case GestureAction.Dragging:
          gestures.FireDragContinue(State.Drag);
          break;
        case GestureAction.Zooming:
          gestures.FireZoomContinue(State.Zoom);
          break;
        case GestureAction.Twisting:
          gestures.FireTwistContinue(State.Twist);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void Update() {
      if (State == null) {
        return;
      }
      platform?.Update();
      var lastAction = currentAction;
      currentAction = State.Action;
      if (lastAction == currentAction) {
        InvokeContinues();
        return;
      }
      InvokeStops(lastAction);
      switch (currentAction) {
        case GestureAction.Tap:
          gestures.FireTap(State.Tap);
          break;
        case GestureAction.Idle:
          break;
        case GestureAction.Dragging:
          gestures.FireDragStart(State.Drag);
          break;
        case GestureAction.Zooming:
          gestures.FireZoomStart(State.Zoom);
          break;
        case GestureAction.Twisting:
          gestures.FireTwistStart(State.Twist);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

    }

  }
}
