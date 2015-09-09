using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.Projectiles {
  public class Pool : MonoBehaviour {
    [SerializeField]
    private GameObject[]
      prefabs;

    private List<Projectile> free = new List<Projectile>();

    private List<Projectile> active = new List<Projectile>();

    public void Fire(string typeName, Vector2 position, Vector2 localScale, Vector2 velocity) {
      Projectile available = null;

      foreach (Projectile projectile in free) {
        if (projectile.TypeName.Equals(typeName)) {
          available = projectile;
          break;
        }
      }

      if (available != null) {
        free.Remove(available);
      } else {
        GameObject prefab = null;

        foreach (GameObject gameObject in prefabs) {
          Projectile component = gameObject.GetComponent<Projectile>();

          if (component.TypeName.Equals(typeName)) {
            prefab = gameObject;
            break;
          }
        }

        if (prefab != null) {
          GameObject gameObject = (GameObject) Instantiate(prefab, position, Quaternion.identity);
          available = gameObject.GetComponent<Projectile>();
          available.Pool = this;
        }
      }

      if (available != null) {
        available.gameObject.SetActive(true);

        available.transform.position = position;
        available.transform.localScale = localScale;

        Rigidbody2D rigidBody = available.GetComponent<Rigidbody2D>();
        rigidBody.velocity = velocity;

        active.Add(available);
      }
    }

    public void Free(Projectile projectile) {
      active.Remove(projectile);

      if (!free.Contains(projectile)) {
        free.Add(projectile);

        projectile.gameObject.SetActive(false);
      }
    }
  }
}