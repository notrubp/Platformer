using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Z.Util.Bind;

namespace Game.Entities.Util {
  public class Pool : MonoBehaviour {
    [SerializeField]
    private Entity
      prefab;

    [SerializeField]
    private int
      initialSize = 0;

    private LinkedList<Entity> free = new LinkedList<Entity>();

    private LinkedList<Entity> active = new LinkedList<Entity>();

    public void Awake() {
      for (int i = 0; i < initialSize; ++i) {
        Entity entity = Instantiate();

        if (entity != null) {
          entity.Kill();
        }
      }
    }

    public Entity Spawn(Vector2 position, Vector2 localScale, Vector2 velocity) {
      Entity entity = null;

      if (free.Count > 0) {
        entity = free.First.Value;
        free.RemoveFirst();
      } else {
        entity = Instantiate();
      }

      if (entity != null) {
        entity.Revive();
        entity.transform.position = position;
        entity.transform.localScale = localScale;

        if (entity.Body != null) {
          entity.Body.velocity = velocity;
        }
      }

      return entity;
    }

    protected virtual Entity Instantiate() {
      Entity entity = null;

      if (prefab != null) {
        entity = (Entity) Instantiate(prefab, Vector3.zero, Quaternion.identity);
        entity.OnRevive += new Action<Entity>(this.OnRevive).Bind(entity);
        entity.OnKill += new Action<Entity>(this.OnKill).Bind(entity);
      }

      return entity;
    }

    protected void OnRevive(Entity o) {
      if (free.Contains(o)) {
        free.Remove(o);
      }

      active.AddLast(o);
    }

    protected void OnKill(Entity o) {
      active.Remove(o);

      if (!free.Contains(o)) {
        free.AddLast(o);
      }
    }
  }
}