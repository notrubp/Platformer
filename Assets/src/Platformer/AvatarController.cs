using UnityEngine;
using System.Collections;

namespace Platformer
{
	public class AvatarController : MonoBehaviour
	{
		Rigidbody2D rigidBody;
		CircleCollider2D feet;
		CircleCollider2D feetCast;
		Vector2 previousPosition;
		Animator[] animators;
		bool jump = false;
		bool fire1 = false;
		bool fire2 = false;
		bool grounded = false;
		bool direction = true;
		bool disableInput = false;

		void Awake ()
		{
			rigidBody = GetComponent<Rigidbody2D> ();
			feet = transform.FindChild ("feet").GetComponent<CircleCollider2D> ();
			feetCast = transform.FindChild ("feetCast").GetComponent<CircleCollider2D> ();
			animators = GetComponentsInChildren<Animator> ();
			previousPosition = rigidBody.transform.position;
		}

		void Update ()
		{
			fire1 = Input.GetButton ("Fire1");

			foreach (Animator animator in animators) {
				animator.SetBool ("fire1", fire1);
			}
		
			fire2 = Input.GetButton ("Fire2");

			foreach (Animator animator in animators) {
				animator.SetBool ("fire2", fire2);
			}
		}
		
		void FixedUpdate ()
		{
			Vector2 movement = rigidBody.position - previousPosition;

			bool moving = Mathf.Abs (movement.x) >= 0.01f;
		
			foreach (Animator animator in animators) {
				animator.SetBool ("moving", moving);
			}

			Vector3 feetDiff = feetCast.transform.position - feet.transform.position;
			grounded = Physics2D.CircleCast (feet.transform.position, feetCast.radius, feetDiff, feetDiff.magnitude, 1 << LayerMask.NameToLayer ("Ground"));

			Debug.DrawLine (feet.transform.position + new Vector3 (feet.offset.x, feet.offset.y), feet.transform.position + new Vector3 (feet.offset.x, feet.offset.y) + Vector3.down * 0.1f);
	
			foreach (Animator animator in animators) {
				animator.SetBool ("grounded", grounded);
			}

			if (grounded) {
				disableInput = false;

				foreach (Animator animator in animators) {
					animator.ResetTrigger ("jump");
				}
			}
	
			if (!disableInput) {
				if (Input.GetButtonDown ("Jump") && grounded) {
					jump = true;
				}

				float horizontal = Input.GetAxisRaw ("Horizontal");

				if (horizontal < 0) {
					rigidBody.velocity += Vector2.left * 1.0f;
					direction = false;
				} else if (horizontal > 0) {
					rigidBody.velocity += Vector2.right * 1.0f;
					direction = true;
				} else {
					Vector2 v = rigidBody.velocity;
					v.x *= 0.7f;
					rigidBody.velocity = v;
				}
			}

			float maxVelocity = 10.0f;

			rigidBody.velocity = new Vector2 (rigidBody.velocity.x <= -maxVelocity ? -maxVelocity : rigidBody.velocity.x >= maxVelocity ? maxVelocity : rigidBody.velocity.x, rigidBody.velocity.y);

			transform.localScale = new Vector3 (direction ? 1.0f : -1.0f, 1.0f);

			if (jump) {
				foreach (Animator animator in animators) {
					animator.SetTrigger ("jump");
				}

				rigidBody.velocity += Vector2.up * 10.0f;

				jump = false;
			}

			previousPosition = rigidBody.transform.position;
		}

		void OnCollisionEnter2D (Collision2D collision)
		{
			disableInput = !grounded && collision.gameObject.CompareTag ("Static World");
		}

		void OnCollisionStay2D (Collision2D collision)
		{
			disableInput = !grounded && collision.gameObject.CompareTag ("Static World");
		}

		void OnCollisionExit2D (Collision2D collision)
		{
			disableInput = false;
		}
	}
}
