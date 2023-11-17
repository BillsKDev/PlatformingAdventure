using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject _open1;
    [SerializeField] GameObject _open2;
    [SerializeField] GameObject _closed1;
    [SerializeField] GameObject _closed2;
    public void Open()
    {
        _open1.SetActive(true);
        _open2.SetActive(true);
        _closed1.SetActive(false);
        _closed2.SetActive(false);
    }

    public void Close()
    {
        _open1.SetActive(false);
        _open2.SetActive(false);
        _closed1.SetActive(true);
        _closed2.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var playerInteractionController = collision.GetComponent<PlayerInteractionController>();
        if (playerInteractionController)
            playerInteractionController.Add(this);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        var playerInteractionController = collision.GetComponent<PlayerInteractionController>();
        if (playerInteractionController)
            playerInteractionController.Remove(this);
    }

    public void Interact(PlayerInteractionController playerInteractionController)
    {
        var destination = Vector2.Distance(playerInteractionController.transform.position, _open1.transform.position) >
            Vector2.Distance(playerInteractionController.transform.position, _open2.transform.position) 
            ? _open1.transform.position
            : _open2.transform.position;

        playerInteractionController.transform.position = destination;
    }
}
