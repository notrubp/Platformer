using UnityEngine;
using System.Collections;

namespace Platformer {
  public class CameraFollow : MonoBehaviour {
    [SerializeField]
    private GameObject target;

    void Awake() {
    }

    void Update() {
      if (target != null) {
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, -10.0f);
      }
    }
  }
}