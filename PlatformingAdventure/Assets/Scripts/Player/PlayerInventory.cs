using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] Transform ItemPoint;

    IItem EquippedItem => _items.Count >= _currentItemIndex ? _items[_currentItemIndex] : null;
    List<IItem> _items = new List<IItem>();
    PlayerInput _playerInput;

    int _currentItemIndex = 0;

    void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _playerInput.actions["Fire"].performed += UseEquippedItem;
        _playerInput.actions["EquipNext"].performed += EquipNext;

        foreach (var item in GetComponentsInChildren<IItem>())
            Pickup(item);
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
        if (EquippedItem != null)
            EquippedItem.Use();
    }

    public void Pickup(IItem item)
    {
        item.transform.SetParent(ItemPoint);
        item.transform.localPosition = Vector3.zero;
        _items.Add(item);
        _currentItemIndex = _items.Count - 1;
        ToggleEquippedItem();

        var collider = item.gameObject.GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;
    }
}
