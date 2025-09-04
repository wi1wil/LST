using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireballScript : MonoBehaviour
{
    public void OnFireball(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Fireball activated");
        }   
    }
}
