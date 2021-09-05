using UnityEngine;

namespace RDG.UnityInput {
    
    public class GestureDragState {
        public Vector3 StartScreenPoint;
        public Vector3 CurrentScreenPoint;
        public Ray StartWorldRay;
        public Ray CurrentWorldRay;
    }

    public class GestureZoomState {
        public float StartZoomLevel;
        public float CurrentZoomLevel;
        public float DeltaZoomLevel;
    }

    public class GestureTapState {
        public Vector3 TapScreenPoint;
        public Ray TapWorldRay;
    }

    public class GestureTwistState {
        public Vector2 StartScreenDirection;
        public Vector2 CurrentScreenDirection;
    }

    public enum GestureAction {
        Idle, Dragging, Zooming,
        Twisting, Tap
    }

    public class GestureState {
        public string PlatformName;
        public GestureAction Action;
        public GestureDragState Drag;
        public GestureZoomState Zoom;
        public GestureTwistState Twist;
        public GestureTapState Tap;
    }
}