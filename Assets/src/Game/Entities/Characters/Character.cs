using System;
using UnityEngine;
using Z.Util;
using Game.Entities.Controllers;

namespace Game.Entities.Characters {
  public class Character : Entity {
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

    private ObservableValue<bool> isMoving = new ObservableValue<bool>(false);
    
    public bool IsMoving {
      get {
        return isMoving.Value;
      }
      set {
        isMoving.Value = value;
      }
    }
    
    public ObservableValue<bool> IsMovingObservable {
      get {
        return isMoving;
      }
    }
  }
}

