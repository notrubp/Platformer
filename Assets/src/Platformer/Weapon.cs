using UnityEngine;
using System.Collections;

namespace Platformer {
  public class Weapon : MonoBehaviour {
    [SerializeField]
    private float
      fireRate = 0.1f;

    public float FireRate {
      get {
        return fireRate;
      }
      set {
        fireRate = value;
      }
    }
  
    [SerializeField]
    private float
      lastFire = 0.0f;

    [SerializeField]
    private Character
      character;

    public Character Character {
      get {
        return character;
      }
    }

    [SerializeField]
    private ProjectilePool
      pool;

    public ProjectilePool Pool {
      get {
        return pool;
      }
    }
  
    private Animator[] animators;

    protected virtual void Awake() {
      animators = character.GetComponentsInChildren<Animator>();
    }

    protected virtual void Update() {
      bool fire1 = Input.GetButton("Fire1");
    
      foreach (Animator animator in animators) {
        animator.SetBool("fire1", fire1);
      }
    
      if (fire1) {
        if (lastFire + fireRate < Time.unscaledTime) {
          pool.Fire("machinegun_bullet", 
            transform.position + new Vector3(0.0f, UnityEngine.Random.Range(-0.2f, 0.2f)), 
            new Vector2(character.IsHFlipped ? -1.0f : 1.0f, 1.0f), 
            new Vector2(character.IsHFlipped ? -30.0f : 30.0f, 0.0f));
        
          lastFire = Time.unscaledTime;
        }
      }
    
      bool fire2 = Input.GetButton("Fire2");
    
      foreach (Animator animator in animators) {
        animator.SetBool("fire2", fire2);
      }
    }
  }
}