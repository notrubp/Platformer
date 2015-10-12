using UnityEngine;
using System.Collections;
using Game.Entities;
using Game.Entities.Util;

namespace Game.Entities.Projectiles {
  public class Projectile : Entity {
    [SerializeField]
    private Pool
      worldCollideFxPool;
    
    public Pool WorldCollideFxPool {
      get {
        return worldCollideFxPool;
      }
      set {
        worldCollideFxPool = value;
      }
    }

    protected override void Awake() {
      IsHFlippedObservable.ValueChanged += (bool value, bool oldValue) => {
        Vector3 localScale = transform.localScale;
        localScale.x = IsHFlipped ? -1.0f : 1.0f;
        transform.localScale = localScale;
      };
    }

    protected override void OnCollisionEnter2D(Collision2D collision) {
      if (collision.gameObject.CompareTag("World")) {
        ExplodeAt(collision.contacts[0].point);
        Kill();
      }
    }

    protected virtual void ExplodeAt(Vector2 position) {
      if (worldCollideFxPool != null) {
        worldCollideFxPool.Spawn(position, Vector2.one, Vector2.zero);
      }
    }
  }
}
