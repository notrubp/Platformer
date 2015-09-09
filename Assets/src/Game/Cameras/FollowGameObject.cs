using UnityEngine;
using System.Collections;

namespace Game.Cameras {
  public class FollowGameObject : MonoBehaviour {
    [SerializeField]
    private GameObject
      target;

    public GameObject Target {
      get {
        return target;
      }
      set {
        target = value;
      }
    }

    void Awake() {
    }

    void Update() {
      if (target != null) {
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z);
      }
    }
  }
}