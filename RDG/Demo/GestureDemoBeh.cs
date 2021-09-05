using System;
using UnityEngine;
using UnityEngine.UI;


namespace RDG.UnityInput {
    [Serializable]
    public class GestureDemoBeh : MonoBehaviour {
        public KeyActionsSo keys;
        public GesturesSo gesture;
        public GestureProducerBeh producer;

        public Text actionText;
        public Text activeActionText;
        public float forwardDraw;
        public GameObject startIndicator;
        public GameObject endIndicator;
        public float twistOffset = 1.0f;


        public void Start() {
            gesture.OnTap += HandleTap;
            gesture.OnDragStart += HandleStart;
            gesture.OnDragContinue += HandleDrag;
            gesture.OnDragStop += HandleStop;
            gesture.OnTwistStart += HandleStart;
            gesture.OnTwistContinue += HandleTwist;
            gesture.OnTwistStop += HandleStop;
        }
        
        public void OnDestroy() {
            gesture.OnTap -= HandleTap;
            gesture.OnDragStart -= HandleStart;
            gesture.OnDragContinue -= HandleDrag;
            gesture.OnDragStop -= HandleStop;
            gesture.OnTwistStart -= HandleStart;
            gesture.OnTwistContinue -= HandleTwist;
            gesture.OnTwistStop -= HandleStop;
        }
        
        private void HandleStart(object _) {
            startIndicator.SetActive(true);
            endIndicator.SetActive(true);
        }
        
        private void HandleStop(object _) {
            startIndicator.SetActive(false);
            endIndicator.SetActive(false);
        }
        
        private void HandleDrag(GestureDragState drag) {
            startIndicator.transform.position = drag.StartWorldRay.GetPoint(forwardDraw);
            endIndicator.transform.position = drag.CurrentWorldRay.GetPoint(forwardDraw);
        }
        

        private void HandleTap(GestureTapState tap) {
            startIndicator.SetActive(true);
            endIndicator.SetActive(true);
            startIndicator.transform.position = tap.TapWorldRay.GetPoint(forwardDraw);
            endIndicator.transform.position = tap.TapWorldRay.GetPoint(forwardDraw);
        }

        public void HandleTwist(GestureTwistState twist) {
            if (Camera.main == null) {
                return;
            }

            var t = Camera.main.transform;
            var forward = t.forward;
            startIndicator.transform.position = t.position + forward * forwardDraw;
            var offset = t.rotation * (twist.CurrentScreenDirection) * twistOffset;
            endIndicator.transform.position = startIndicator.transform.position + offset;
        }
        
        
        public void Update() {
            if (producer.State == null) {
                return;
            }

            actionText.text = producer.State.PlatformName;
            if (producer.State.Action != GestureAction.Idle) {
                activeActionText.text = producer.State.Action.ToString();
            }
            actionText.text = producer.State.Action.ToString();
        }
    }
}
