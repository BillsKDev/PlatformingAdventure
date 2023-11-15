using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] Transform ItemPoint;

    Key EquippedKey => _items.Count >= _currentItemIndex ? _items[_currentItemIndex] : null;
    List<Key> _items = new List<Key>();
    PlayerInput _playerInput;

    int _currentItemIndex = 0;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions["Fire"].performed += UseEquippedItem;
        _playerInput.actions["EquipNext"].performed += EquipNext;
    }

    void EquipNext(InputAction.CallbackContext context)
    {
        _currentItemIndex++;
        if (_currentItemIndex >= _items.Count)
            _currentItemIndex = 0;
        ToggleEquippedItem();
    }

    void ToggleEquippedItem()
    {
        for (int i = 0; i < _items.Count; i++)
            _items[i].gameObject.SetActive(i == _currentItemIndex);
    }

    void UseEquippedItem(InputAction.CallbackContext context)
    {
        if (EquippedKey)
            EquippedKey.Use();
    }

    public void Pickup(Key key)
    {
        key.transform.SetParent(ItemPoint);
        key.transform.localPosition = Vector3.zero;
        _items.Add(key);
        _currentItemIndex = _items.Count - 1;
        ToggleEquippedItem();
    }
}
