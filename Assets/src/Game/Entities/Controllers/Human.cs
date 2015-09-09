using UnityEngine;
using System.Collections;

namespace Game.Entities.Controllers {
  public class Human : Controller {
    protected override void Update() {
      Horizontal = Input.GetAxisRaw("Horizontal");
      Vertical = Input.GetAxisRaw("Horizontal"); 
      PrimaryWeapon = Input.GetButton("Fire1");
      SecondaryWeapon = Input.GetButton("Fire2");
      Jump = Input.GetButtonDown("Jump");
    }
  }
}
