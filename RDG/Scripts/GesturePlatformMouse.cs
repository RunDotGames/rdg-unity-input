using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace RDG.UnityInput {
    
    internal enum MouseGestureState {
        Up, Down, Dragging, Zooming, Twisting, Tap,
    }

    internal class GesturePlatformMouse : GesturePlatform {

        private static readonly Vector2 HalfPort = Vector2.one * 0.5f;

        private static readonly Dictionary<MouseGestureState, GestureAction> StateMapping =
            new Dictionary<MouseGestureState, GestureAction>(){
                {
                    MouseGestureState.Up, GestureAction.Idle
                },{
                    MouseGestureState.Down, GestureAction.Idle
                },{
                    MouseGestureState.Dragging, GestureAction.Dragging
                },{
                    MouseGestureState.Zooming, GestureAction.Zooming
                },{
                    MouseGestureState.Twisting, GestureAction.Twisting
                },{
                    MouseGestureState.Tap, GestureAction.Tap
                }
            };

        private static readonly HashSet<MouseGestureState> CanStartDrag = new HashSet<MouseGestureState>(){
            MouseGestureState.Up,
            MouseGestureState.Zooming
        };

        private readonly GesturePlatformMouseConfig config;
        private readonly GestureState state;
        private readonly Camera camera;
        private readonly EventSystem eventSystem;
        
        private MouseGestureState currentState = MouseGestureState.Up;
        private Vector3 downStartPosition;
        private float zoomLastScrollTime;

        public GesturePlatformMouse(GesturePlatformMouseConfig config, GestureState state, Camera camera,
            EventSystem eventSystem) {
            this.config = config;
            this.state = state;
            this.camera = camera;
            this.eventSystem = eventSystem;
        }
        
        public void Release(){}
        
        private void UpdateState(MouseGestureState aState) {
            currentState = aState;
            state.Action = StateMapping[aState];
        }

        private Ray MousePositionToCameraRay() {
            var pos = Input.mousePosition;
            return camera.ScreenPointToRay(pos);
        }

        private void UpdateUp() {
            var isPrimaryUp = Input.GetMouseButtonUp(0);
            var isSecondaryUp = Input.GetMouseButtonUp(1);
            
            if (!(isPrimaryUp || isSecondaryUp)){
                return;
            }

            if (isPrimaryUp && currentState == MouseGestureState.Down) {
                state.Tap.TapWorldRay = MousePositionToCameraRay();
                state.Tap.TapScreenPoint = Input.mousePosition;
                UpdateState(MouseGestureState.Tap);
                return;
            }

            UpdateState(MouseGestureState.Up);
        }

        private void UpdateDown() {
            var isPrimaryDown = Input.GetMouseButtonDown(0);
            var isSecondaryDown = Input.GetMouseButtonDown(1);
            
            if (!(isPrimaryDown || isSecondaryDown)){
                return;
            }
            
            if (eventSystem.IsPointerOverGameObject()) {
                return;
            }

            if (isSecondaryDown) {
                state.Twist.StartScreenDirection = ((Vector2)camera.ScreenToViewportPoint(Input.mousePosition) - HalfPort).normalized;
                state.Twist.CurrentScreenDirection = state.Twist.StartScreenDirection;
                UpdateState(MouseGestureState.Twisting);
                return;
            }

            // isPrimaryDown interaction
            if (CanStartDrag.Contains(currentState)) {
                downStartPosition = Input.mousePosition;
                UpdateState(MouseGestureState.Down);
            }
        }

        public void Update() {
            UpdateDown();
            UpdateUp();
            var scrollDelta = Input.mouseScrollDelta.y;
            var isScrolling = Mathf.Abs(scrollDelta) > Mathf.Epsilon;
            if (currentState != MouseGestureState.Zooming && isScrolling) {
                UpdateState(MouseGestureState.Zooming);
                state.Zoom.StartZoomLevel = state.Zoom.CurrentZoomLevel;
            }

            switch (currentState) {
                case MouseGestureState.Tap:
                    currentState = MouseGestureState.Up;
                    state.Action = GestureAction.Tap;
                    break;
                case MouseGestureState.Up:
                    state.Action = GestureAction.Idle;
                    break;
                case MouseGestureState.Down:
                    var currentVector = (Input.mousePosition - downStartPosition);
                    var currentDistance = currentVector.magnitude;
                    if (currentDistance > config.minDragDistance) {
                        UpdateState(MouseGestureState.Dragging);
                        state.Drag.StartWorldRay = MousePositionToCameraRay();
                        state.Drag.StartScreenPoint = Input.mousePosition;
                    }
                    break;
                case MouseGestureState.Dragging:
                    state.Drag.CurrentWorldRay = MousePositionToCameraRay();
                    state.Drag.CurrentScreenPoint = Input.mousePosition;
                    break;
                case MouseGestureState.Zooming:
                    state.Zoom.DeltaZoomLevel = scrollDelta * (config.invertZoom ? -1.0f : 1.0f);
                    state.Zoom.CurrentZoomLevel += state.Zoom.DeltaZoomLevel;
                    if (isScrolling) {
                        zoomLastScrollTime = Time.time;
                        break;
                    }
                    if (Time.time - zoomLastScrollTime > config.zoomDelaySeconds) {
                        UpdateState(MouseGestureState.Up);
                    }
                    break;
                case MouseGestureState.Twisting:
                    state.Twist.CurrentScreenDirection = ((Vector2)camera.ScreenToViewportPoint(Input.mousePosition) - HalfPort).normalized;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}