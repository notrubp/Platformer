using UnityEngine;
using System.Collections;
using Z.Util;
using Game.Entities.Util;
using System;

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

    // Quick utility for toggling.
    public void HFlip() {
      IsHFlipped = !IsHFlipped;
    }
    
    public ObservableValue<bool> IsHFlippedObservable {
      get {
        return isHFlipped;
      }
    }

    private ObservableValue<float> flyingIntoGeometryLocalPlane = new ObservableValue<float>(0.0f);
    
    public float FlyingIntoGeometryLocalPlane {
      get {
        return flyingIntoGeometryLocalPlane.Value;
      }
      set {
        flyingIntoGeometryLocalPlane.Value = value;
      }
    }
    
    public ObservableValue<float> IsFlyingIntoWorldColliderObservable {
      get {
        return flyingIntoGeometryLocalPlane;
      }
    }

    public event Action OnUpdate;

    public event Action OnPreFixedUpdate;

    public event Action OnPostFixedUpdate;

    public event Action OnRevive;

    public event Action OnKill;

    protected virtual void Awake() {
      lastKnownPosition = body != null ? body.position : Vector2.zero;
      lastVelocityScaled = Vector2.zero;
    }

    protected virtual void Update() {
      if (IsOnGround) {
        FlyingIntoGeometryLocalPlane = 0.0f;
      }

      if (OnUpdate != null) {
        OnUpdate();
      }
    }

    protected virtual void FixedUpdate() {
      PreFixedUpdate();
      ApplyFixedUpdate();
      PostFixedUpdate();
    }

    protected virtual void PreFixedUpdate() {
      lastVelocityScaled = body != null ? body.position - lastKnownPosition : Vector2.zero;

      if (OnPreFixedUpdate != null) {
        OnPreFixedUpdate();
      }
    }

    protected virtual void ApplyFixedUpdate() {
    }

    protected virtual void PostFixedUpdate() {
      lastKnownPosition = body != null ? body.position : Vector2.zero;

      if (OnPostFixedUpdate != null) {
        OnPostFixedUpdate();
      }
    }

    public virtual void Revive() {
      gameObject.SetActive(true);

      if (OnRevive != null) {
        OnRevive();
      }
    }

    public virtual void Kill() {
      if (OnKill != null) {
        OnKill();
      }

      gameObject.SetActive(false);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision) {
      DoFlyingIntoGeometryCheck(collision);
    }

    protected virtual void OnCollisionStay2D(Collision2D collision) {
      DoFlyingIntoGeometryCheck(collision);
    }

    protected void DoFlyingIntoGeometryCheck(Collision2D collision) {
      if (!IsOnGround && collision.gameObject.layer == LayerMask.NameToLayer("World")) {
        float closest = float.MaxValue;

        foreach (ContactPoint2D contactPoint in collision.contacts) {
          float x = contactPoint.point.x - transform.position.x;

          if (Mathf.Abs(x) < closest) {
            closest = x;
          } 
        }

        FlyingIntoGeometryLocalPlane = closest != float.MaxValue ? closest : 0.0f;
      }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision) {
      FlyingIntoGeometryLocalPlane = 0;
    }
  }
}
