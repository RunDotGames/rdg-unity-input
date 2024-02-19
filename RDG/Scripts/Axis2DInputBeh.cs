using UnityEngine;
using UnityEngine.Events;

namespace RDG.Chop_Chop.Scripts.Input {

  // Converts 2 axis inputs into a Vector2 direction (oriented away from the camera)
  
  public class Axis2DInputBeh : MonoBehaviour {
    
    [Header("Config")]
    [Tooltip("Axis forward is relative to the provided transform when set")]
    [SerializeField] private Transform axisForward;
    
    
    [Header("Events")]
    [SerializeField] private UnityEvent<Vector2> onChange;

    private Vector2 rawValue = Vector2.zero;
    private Vector3 oldForward;
    private bool hasForward;
    

    
    public void OnDestroy() {
      onChange?.RemoveAllListeners();
    }

    public void Awake() {
      hasForward = axisForward != null;
    }

    public void Update() {
      var forward = hasForward ? axisForward.forward : Vector3.forward;
      if (forward != oldForward) {
        onChange?.Invoke(Value);
      }
      oldForward = forward; 
    }

    public Vector2 Value {
      get {
        if (!isActiveAndEnabled) {
          return Vector2.zero;
        }
        if (!hasForward) {
          return new Vector2(rawValue.x, rawValue.y);
        }
        
        var forwardSpace = axisForward.TransformVector(new Vector3(rawValue.x, 0, rawValue.y));
        return new Vector2(forwardSpace.x, forwardSpace.z).normalized;
      }
    }

    public void SetX(float value) {
      rawValue.x = Mathf.Max(-1, Mathf.Min(value, 1));
      onChange?.Invoke(Value);
    }

    public void SetY(float value) {
      rawValue.y = Mathf.Max(-1, Mathf.Min(value, 1));
      onChange?.Invoke(Value);
    }
  }
}
