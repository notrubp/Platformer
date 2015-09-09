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
      Animator[] animators = GetComponentsInChildren<Animator>();

      bool fire1 = Controller.PrimaryWeapon;

      foreach (Animator animator in animators) {
        animator.SetBool("fire1", fire1);
      }

      if (fire1) {
        bool fire2 = Controller.SecondaryWeapon;
      
        foreach (Animator animator in animators) {
          animator.SetBool("fire2", fire2);
        }
      }

      horizontalMovement = Controller.Horizontal;

      if (horizontalMovement < 0) {
        IsHFlipped = true;
      } else if (horizontalMovement > 0) {
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
