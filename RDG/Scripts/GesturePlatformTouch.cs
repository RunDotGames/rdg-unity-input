using System;
using UnityEngine;

namespace RDG.UnityInput {
    
    internal class TouchGesture {
        public Vector2 StartScreenPoint;
        public Vector2 CurrentScreenPoint;
        public bool IsTouching;
    }

    internal class GesturePlatformTouch : GesturePlatform {

        private readonly GestureState state;
        private readonly TouchGesture[] touches ={
            new TouchGesture(), new TouchGesture()
        };
        private readonly GesturePlatformTouchConfig config;
        
        private Camera camera;

        public GesturePlatformTouch(GestureState state, GesturePlatformTouchConfig config) {
            this.state = state;
            this.config = config;
        }

        private Ray TouchToCameraRay(Touch touch) {
            return camera.ScreenPointToRay(touch.position);
        }


        private void UpdateSingleTouch() {
            if (Input.touchCount != 1) {
                state.Action = GestureAction.Idle;
                return;
            }

            var touch = Input.GetTouch(0);
            switch (touch.phase) {
                case TouchPhase.Began:
                    state.Action = GestureAction.Idle;
                    break;
                case TouchPhase.Moved:
                    if (state.Action == GestureAction.Idle) {
                        state.Drag.StartWorldRay = TouchToCameraRay(touch);
                        state.Drag.StartScreenPoint = touch.position;
                    }
                    state.Action = GestureAction.Dragging;
                    state.Drag.CurrentWorldRay = TouchToCameraRay(touch);
                    state.Drag.CurrentScreenPoint = touch.position;
                    break;
                case TouchPhase.Stationary:
                    break;
                case TouchPhase.Ended:
                    if (state.Action == GestureAction.Idle) {
                        state.Action = GestureAction.Tap;
                        state.Tap.TapWorldRay = TouchToCameraRay(touch);
                        state.Tap.TapScreenPoint = touch.position;
                    }
                    else {
                        state.Action = GestureAction.Idle;
                    }
                    break;
                case TouchPhase.Canceled:
                    state.Action = GestureAction.Idle;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void PopulateDrag(Touch touch, TouchGesture gesture) {
            if (!gesture.IsTouching) {
                gesture.StartScreenPoint = touch.position;
            }
            gesture.CurrentScreenPoint = touch.position;
            gesture.IsTouching = true;
        }

        private bool UpdateTwist(Vector2 startDirection, Vector2 currentDirection) {
            var wasTwisting = state.Action == GestureAction.Twisting;
            if (!wasTwisting && 1.0f - Mathf.Abs(Vector3.Dot(startDirection.normalized, currentDirection.normalized)) <
                config.twistTolerance) {
                return false;
            }

            if (!wasTwisting) {
                state.Twist.StartScreenDirection = startDirection.normalized;
            }
            state.Twist.CurrentScreenDirection = currentDirection.normalized;
            state.Action = GestureAction.Twisting;
            return true;
        }

        private bool UpdateZoom(Vector2 startDirection, Vector2 currentDirection) {
            var wasZooming = state.Action == GestureAction.Zooming;
            var startDistance = (startDirection).magnitude;
            var currentDistance = (currentDirection).magnitude;
            var delta = (currentDistance - startDistance) * (config.invertZoom ? -1.0f : 1.0f);
            if (!wasZooming && Mathf.Abs(delta) < config.zoomTolerance) {
                return false;
            }

            delta *= config.zoomScale;

            if (!wasZooming) {
                state.Zoom.StartZoomLevel = state.Zoom.CurrentZoomLevel;
            }

            var oldCurrent = state.Zoom.CurrentZoomLevel;
            var current = state.Zoom.StartZoomLevel + delta;

            state.Action = GestureAction.Zooming;
            state.Zoom.CurrentZoomLevel = current;
            state.Zoom.DeltaZoomLevel = current - oldCurrent;
            return true;
        }

        private void UpdateMultiTouch() {
            if (Input.touchCount < 2) {
                touches[1].IsTouching = false;
                touches[0].IsTouching = false;
                return;
            }

            PopulateDrag(Input.GetTouch(1), touches[1]);
            PopulateDrag(Input.GetTouch(0), touches[0]);
            var startDirection = touches[0].StartScreenPoint - touches[1].StartScreenPoint;
            var currentDirection = touches[0].CurrentScreenPoint - touches[1].CurrentScreenPoint;
            if (UpdateTwist(startDirection, currentDirection)) {
                return;
            }

            if (UpdateZoom(startDirection, currentDirection)) {
                return;
            }
            state.Action = GestureAction.Idle;

        }

        public void Update() {
            camera = Camera.main;
            if (Input.touchCount == 0) {
                state.Action = GestureAction.Idle;
                return;
            }
            UpdateSingleTouch();
            UpdateMultiTouch();
        }

        public void Release() { }
    }
}