using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Game : MonoBehaviour
{
 public InputActionReference _controllerPosition;


 private void OnEnable()
 {
  _controllerPosition.action.Enable();
 }


 private void Update()
 {
   Debug.Log(_controllerPosition.action.ReadValue<Vector3>());
 }
}