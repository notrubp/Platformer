using UnityEngine;
using System.Collections;
using Z.Util;
using Game.Entities.Controllers;

namespace Game.Entities {
  public class Entity : MonoBehaviour {
    [SerializeField]
    private Rigidbody2D
      body;

    public Rigidbody2D Body {
      get {
        return body;
      }
    }

    [SerializeField]
    private Controller
      controller;

    public Controller Controller {
      get {
        return controller;
      }
      set {
        controller = value;
      }
    }

    private Vector2 lastKnownPosition;

    public Vector2 LastKnownPosition {
      get {
        return lastKnownPosition;
      }
    }

    private Vector2 lastVelocityScaled;

    public Vector2 LastVelocityScaled {
      get {
        return lastVelocityScaled;
      }
    }

    private ObservableValue<bool> isOnGround = new ObservableValue<bool>(false);

    public bool IsOnGround {
      get {
        return isOnGround.Value;
      }
      set {
        isOnGround.Value = value;
      }
    }

    public ObservableValue<bool> IsOnGroundObservable {
      get {
        return isOnGround;
      }
    }

    private ObservableValue<bool> isHFlipped = new ObservableValue<bool>(false);
    
    public bool IsHFlipped {
      get {
        return isHFlipped.Value;
      }
      set {
        isHFlipped.Value = value;
      }
    }
    
    public ObservableValue<bool> IsHFlippedObservable {
      get {
        return isHFlipped;
      }
    }

    private ObservableValue<float> flyingIntoWorldCollider = new ObservableValue<float>(0.0f);
    
    public float FlyingIntoWorldCollider {
      get {
        return flyingIntoWorldCollider.Value;
      }
      set {
        flyingIntoWorldCollider.Value = value;
      }
    }
    
    public ObservableValue<float> IsFlyingIntoWorldColliderObservable {
      get {
        return flyingIntoWorldCollider;
      }
    }
   
    protected virtual void Awake() {
      lastKnownPosition = body.position;
      lastVelocityScaled = new Vector2();
    }

    protected virtual void Update() {
      if (IsOnGround) {
        FlyingIntoWorldCollider = 0.0f;
      }
    }

    protected virtual void FixedUpdate() {
      PreFixedUpdate();
      ApplyFixedUpdate();
      PostFixedUpdate();
    }

    protected virtual void PreFixedUpdate() {
      lastVelocityScaled = body.position - lastKnownPosition;
    }

    protected virtual void ApplyFixedUpdate() {
    }

    protected virtual void PostFixedUpdate() {
      lastKnownPosition = body.position;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision) {
      DoFlyingIntoWorldColliderCheck(collision);
    }

    protected virtual void OnCollisionStay2D(Collision2D collision) {
      DoFlyingIntoWorldColliderCheck(collision);
    }

    protected void DoFlyingIntoWorldColliderCheck(Collision2D collision) {
      if (!IsOnGround && collision.gameObject.layer == LayerMask.NameToLayer("World")) {
        float avgx = 0.0f;

        foreach (ContactPoint2D contactPoint in collision.contacts) {
          avgx += contactPoint.point.x;
        }

        avgx /= collision.contacts.Length;

        float x = transform.position.x;

        FlyingIntoWorldCollider = avgx - x;
      }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision) {
      FlyingIntoWorldCollider = 0;
    }
  }
}
