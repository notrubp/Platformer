using UnityEngine;
using System.Collections;
using Game.Entities;
using Game.Projectiles;
using Game.Entities.Controllers;

namespace Game.Weapons {
  public class Weapon : MonoBehaviour {
    [SerializeField]
    private Entity
      entity;
    
    public Entity Entity {
      get {
        return entity;
      }
    }

    [SerializeField]
    private Pool
      pool;
    
    public Pool Pool {
      get {
        return pool;
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
  
    private float
      lastFire = 0.0f;

    protected virtual void Awake() {
    }

    protected virtual void Update() {
      if (Controller.PrimaryWeapon) {
        if (lastFire + fireRate < Time.unscaledTime) {
          Fire();
          lastFire = Time.unscaledTime;
        }
      }
    }

    public virtual void Fire() {
    }
  }
}