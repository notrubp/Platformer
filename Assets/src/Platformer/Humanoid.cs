using UnityEngine;
using Z.Util;

namespace Platformer {
  public class Humanoid : Character {
    private Collider2D feet;

    public Collider2D Feet {
      get {
        return feet;
      }
    }

    private ObservableValue<bool> isMoving = new ObservableValue<bool>(false);
    
    public bool IsMoving {
      get {
        return isMoving.Value;
      }
      set {
        isMoving.Value = value;
      }
    }
    
    public ObservableValue<bool> IsMovingObservable {
      get {
        return isMoving;
      }
    }

    private ObservableValue<bool> isJumping = new ObservableValue<bool>(false);
    
    public bool IsJumping {
      get {
        return isJumping.Value;
      }
      set {
        isJumping.Value = value;
      }
    }
    
    public ObservableValue<bool> IsJumpingObservable {
      get {
        return isJumping;
      }
    }

    protected override void Awake() {
      base.Awake();

      Transform feet = transform.FindChild("feet");

      if (feet == null) {
      }

      this.feet = feet.GetComponent<Collider2D>();

      Animator[] animators = GetComponentsInChildren<Animator>();

      IsOnGroundObservable.ValueChanged += (bool value, bool oldValue) => {
        foreach (Animator animator in animators) {
          animator.SetBool("grounded", value);
        }
      };

      IsMovingObservable.ValueChanged += (bool value, bool oldValue) => {
        foreach (Animator animator in animators) {
          animator.SetBool("moving", value);
        }
      };

      IsJumpingObservable.ValueChanged += (bool value, bool oldValue) => {
        foreach (Animator animator in animators) {
          if (value) {
            animator.SetTrigger("jump");
          } else {
            animator.ResetTrigger("jump");
          }
        }
      };
    }

    private float horizontalMovement;

    protected override void Update() {
      horizontalMovement = Input.GetAxisRaw("Horizontal");

      if (horizontalMovement < 0) {
        IsHFlipped = true;
      } else if (horizontalMovement > 0) {
        IsHFlipped = false;
      }

      IsOnGround = Physics2D.IsTouchingLayers(feet, 1 << LayerMask.NameToLayer("World"));

      IsJumping = IsOnGround && Input.GetButtonDown("Jump");

      if (IsJumping) {
        IsOnGround = false;
      }

      Vector3 localScale = transform.localScale;
      localScale.x = IsHFlipped ? -1.0f : 1.0f;
      transform.localScale = localScale;
    }

    protected override void PreFixedUpdate() {
      base.PreFixedUpdate();

      IsMoving = IsOnGround && Mathf.Abs(LastVelocityScaled.x) >= 0.01f;
    }

    protected override void ApplyFixedUpdate() {
      base.ApplyFixedUpdate();

      Vector2 velocity = Body.velocity;

      if (horizontalMovement < 0.0f && FlyingIntoWorldCollider >= 0.0f) {
        velocity += Vector2.left;
      } else if (horizontalMovement > 0.0f && FlyingIntoWorldCollider <= 0.0f) {
        velocity += Vector2.right;
      } else {
        velocity.x *= 0.9f;
      }

      float max = 10.0f;

      velocity.x = velocity.x <= -max ? -max : velocity.x >= max ? max : velocity.x;

      if (IsJumping) {
        velocity += Vector2.up * 10.0f;
        IsJumping = false;
      }

      Body.velocity = velocity;
    }
  }
}
