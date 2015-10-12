using UnityEngine;
using Z.Util;

namespace Game.Entities.Characters {
  public class Humanoid : Character {
    [SerializeField]
    private Collider2D
      feet;

    public Collider2D Feet {
      get {
        return feet;
      }
    }

    [SerializeField]
    private float groundFriction = 0.9f;

    public float GroundFriction {
      get {
        return groundFriction;
      }
    }

    [SerializeField]
    private float groundAcceleration = 1.0f;

    public float GroundAcceleration {
      get {
        return groundAcceleration;
      }
    }

    [SerializeField]
    private float maxGroundVelocity = 10.0f;

    public float MaxGroundVelocity {
      get {
        return maxGroundVelocity;
      }
    }

    [SerializeField]
    private float jumpForce = 10.0f;

    public float JumpForce {
      get {
        return jumpForce;
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

    private Animator[] animators;

    protected override void Awake() {
      base.Awake();

      animators = GetComponentsInChildren<Animator>();

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

    protected override void Update() {
      bool fire1 = Controller.PrimaryWeapon;
      bool fire2 = Controller.SecondaryWeapon;

      foreach (Animator animator in animators) {
        animator.SetBool("fire1", fire1);
      }

      foreach (Animator animator in animators) {
        animator.SetBool("fire2", fire2);
      }

      if (Controller.Horizontal < 0) {
        IsHFlipped = true;
      } else if (Controller.Horizontal > 0) {
        IsHFlipped = false;
      }

      IsOnGround = Physics2D.IsTouchingLayers(feet, 1 << LayerMask.NameToLayer("World"));

      IsJumping = IsOnGround && Controller.Jump;

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

      if (Controller.Horizontal < 0.0f && FlyingIntoGeometryLocalPlane >= 0.0f) {
        velocity += Vector2.left * groundAcceleration;
      } else if (Controller.Horizontal > 0.0f && FlyingIntoGeometryLocalPlane <= 0.0f) {
        velocity += Vector2.right * groundAcceleration;
      } else {
        velocity.x *= groundFriction;
      }

      // Clamp to maxGroundVelocity.
      velocity.x = velocity.x <= -maxGroundVelocity ? -maxGroundVelocity : velocity.x >= maxGroundVelocity ? maxGroundVelocity : velocity.x;

      if (IsJumping) {
        velocity += Vector2.up * jumpForce;
        IsJumping = false;
      }

      Body.velocity = velocity;
    }
  }
}
