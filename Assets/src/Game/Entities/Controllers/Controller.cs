using UnityEngine;
using System.Collections;
using Z.Util;

namespace Game.Entities.Controllers {
  public class Controller : MonoBehaviour {
    private ObservableValue<float> horizontal = new ObservableValue<float>(0.0f);
    
    public float Horizontal {
      get {
        return horizontal.Value;
      }
      set {
        horizontal.Value = value;
      }
    }
    
    public ObservableValue<float> HorizontalObservable {
      get {
        return horizontal;
      }
    }

    private ObservableValue<float> vertical = new ObservableValue<float>(0.0f);
    
    public float Vertical {
      get {
        return vertical.Value;
      }
      set {
        vertical.Value = value;
      }
    }
    
    public ObservableValue<float> VerticalObservable {
      get {
        return vertical;
      }
    }

    private ObservableValue<bool> primaryWeapon = new ObservableValue<bool>(false);
    
    public bool PrimaryWeapon {
      get {
        return primaryWeapon.Value;
      }
      set {
        primaryWeapon.Value = value;
      }
    }
    
    public ObservableValue<bool> PrimaryWeaponObservable {
      get {
        return primaryWeapon;
      }
    }

    private ObservableValue<bool> secondaryWeapon = new ObservableValue<bool>(false);
    
    public bool SecondaryWeapon {
      get {
        return secondaryWeapon.Value;
      }
      set {
        secondaryWeapon.Value = value;
      }
    }
    
    public ObservableValue<bool> SecondaryWeaponObservable {
      get {
        return secondaryWeapon;
      }
    }

    private ObservableValue<bool> jump = new ObservableValue<bool>(false);
    
    public bool Jump {
      get {
        return jump.Value;
      }
      set {
        jump.Value = value;
      }
    }
    
    public ObservableValue<bool> JumpObservable {
      get {
        return jump;
      }
    }

    protected virtual void Awake() {
    }

    protected virtual void Update() {
    }
  }
}