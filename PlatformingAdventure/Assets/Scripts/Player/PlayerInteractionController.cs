using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionController : MonoBehaviour
{
    TMP_Text _interactText;
    PlayerInput _playerInput;
    List<Door> _doors = new List<Door>();

    

    void Awake()
    {
        _interactText = GetComponentInChildren<TMP_Text>();
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions["Interact"].performed += Interact;
        _interactText.gameObject.SetActive(false);
    }

    void Interact(InputAction.CallbackContext context)
    {
        foreach (var door in _doors)
        {
            door.Interact(this);
        }
    }

    public void Add(Door door)
    {
        _doors.Add(door);
        _interactText.gameObject.SetActive(true);
    }

    public void Remove(Door door)
    {
        _doors.Remove(door);
        if (_doors.Count == 0)
            _interactText.gameObject.SetActive(false);
    }
}
