using UnityEngine;
using System.Collections;
using Game.Entities;
using Game.Entities.Controllers;
using Game.Entities.Util;
using Game.Entities.Projectiles;

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
      projectilePool;
    
    public Pool ProjectilePool {
      get {
        return projectilePool;
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
      if (ShouldFire()) {
        if (lastFire + fireRate < Time.unscaledTime) {
          Fire();
          lastFire = Time.unscaledTime;
        }
      }
    }

    public virtual bool ShouldFire() {
      return false;
    }

    public virtual void Fire() {
    }
  }
}