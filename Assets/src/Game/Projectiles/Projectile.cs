using UnityEngine;
using System.Collections;

namespace Game.Projectiles {
  public class Projectile : MonoBehaviour {
    [SerializeField]
    private string
      typeName;

    public string TypeName {
      get {
        return typeName;
      }
    }
    
    private Pool pool;

    public Pool Pool {
      get {
        return pool;
      }
      set {
        pool = value;
      }
    }

    void OnCollisionEnter2D(Collision2D collision) {
      if (collision.gameObject.CompareTag("World")) {
        pool.Free(this);
      }
    }

    void OnTriggerEnter2D(Collider2D collider) {
      if (collider.CompareTag("World")) {
        pool.Free(this);
      }
    }
  }
}
