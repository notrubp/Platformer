using System;
using Z.Util;

namespace Game.Entities.Characters {
  public class Character : Entity {
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

