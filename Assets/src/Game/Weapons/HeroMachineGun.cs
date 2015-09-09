using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Weapons {
  public class HeroMachineGun : Weapon {
    [SerializeField]
    private String
      projectileTypeName;

    public String ProjectileTypeName {
      get {
        return projectileTypeName;
      } 
      set {
        projectileTypeName = value;
      }
    }

    [SerializeField]
    private float
      spread = 0.2f;

    public float Spread {
      get {
        return spread;
      }
      set {
        spread = value;
      }
    }

    [SerializeField]
    private float
      velocity = 30.0f;

    public float Velocity {
      get {
        return velocity;
      }
      set {
        velocity = value;
      }
    }

    public override void Fire() {
      Vector2 random = new Vector2(0.0f, Random.Range(-spread, spread));
      Vector2 position = new Vector2(transform.position.x, transform.position.y) + random;
      Vector2 localScale = new Vector2(Entity.IsHFlipped ? -1.0f : 1.0f, 1.0f);
      Vector2 velocity = new Vector2(Entity.IsHFlipped ? -this.velocity : this.velocity, 0.0f);

      Pool.Fire(projectileTypeName, position, localScale, velocity); 
    }
  }
}

