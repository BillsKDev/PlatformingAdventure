﻿using UnityEngine;

public abstract class Item : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var playerInventory = collision.GetComponent<PlayerInventory>();
            if (playerInventory != null)
                playerInventory.Pickup(this, true);
        }
    }

    public abstract void Use();

}