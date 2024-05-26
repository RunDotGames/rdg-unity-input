
using System;
using UnityEngine;

namespace RDG.UnityInput {
    
    [Serializable]
    public class GesturePlatformMouseConfig {
        public float minDragDistance = 10.0f;
        public float zoomDelaySeconds = 0.35f;
        public bool invertZoom = true;
    }

    [Serializable]
    public class GesturePlatformTouchConfig {
        public float zoomTolerance = 100.0f;
        public float twistTolerance = 0.01f;
        public float zoomScale = 0.01f;
        public bool invertZoom = true;
    }

    [Serializable]
    public class GesturesConfig {
        public GesturePlatformMouseConfig mouse;
        public GesturePlatformTouchConfig touch;
    }

    [CreateAssetMenu(menuName = "RDG/Input/Gestures")]
    public class GesturesSo : ScriptableObject {
        public GesturesConfig config;

        public event Action<GestureTwistState> OnTwistStart;
        internal void FireTwistStart(GestureTwistState state) {
            OnTwistStart?.Invoke(state);
        }
        
        public event Action<GestureTwistState> OnTwistContinue;
        internal void FireTwistContinue(GestureTwistState state) {
            OnTwistContinue?.Invoke(state);
        }
        
        public event Action<GestureTwistState> OnTwistStop;
        internal void FireTwistStop(GestureTwistState state) {
            OnTwistStop?.Invoke(state);
        }

        public event Action<GestureZoomState> OnZoomStart;
        internal void FireZoomStart(GestureZoomState state) {
            OnZoomStart?.Invoke(state);
        }
        
        public event Action<GestureZoomState> OnZoomContinue;
        internal void FireZoomContinue(GestureZoomState state) {
            OnZoomContinue?.Invoke(state);
        }
        
        public event Action<GestureZoomState> OnZoomStop;
        internal void FireZoomStop(GestureZoomState state) {
            OnZoomStop?.Invoke(state);
        }

        public event Action<GestureDragState> OnDragStart;
        internal void FireDragStart(GestureDragState state) {
            OnDragStart?.Invoke(state);
        }
        
        public event Action<GestureDragState> OnDragContinue;
        internal void FireDragContinue(GestureDragState state) {
            OnDragContinue?.Invoke(state);
        }
        
        public event Action<GestureDragState> OnDragStop;
        internal void FireDragStop(GestureDragState state) {
            OnDragStop?.Invoke(state);
        }

        public event Action<GestureTapState> OnTap;
        internal void FireTap(GestureTapState state) {
            OnTap?.Invoke(state);
        }

    }
}
