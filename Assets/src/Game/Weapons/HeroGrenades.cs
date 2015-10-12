using UnityEngine;
using System.Collections;
using Game.Entities.Util;
using Game.Entities;
using Game.Entities.Projectiles;

namespace Game.Weapons {
  public class HeroGrenades : Weapon {
    [SerializeField]
    private Pool
      explosionPool;
    
    public Pool ExplosionPool {
      get {
        return explosionPool;
      }
    }

    public override bool ShouldFire() {
      return Controller.SecondaryWeapon;
    }

    public override void Fire() {
      Vector2 position = new Vector2(transform.position.x, transform.position.y);
      Vector2 localScale = new Vector2(Entity.IsHFlipped ? -1.0f : 1.0f, 1.0f);
      Vector2 velocity = new Vector2(Mathf.Abs(Entity.Body.velocity.x) + 5.0f, 10.0f);
      velocity.x = Entity.IsHFlipped ? -velocity.x : velocity.x;
      
      Projectile grenade = (Projectile) ProjectilePool.Spawn(position, localScale, velocity);
      grenade.WorldCollideFxPool = explosionPool;
    }
  }
}
