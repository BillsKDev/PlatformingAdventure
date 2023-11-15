using UnityEngine;

public interface IItem
{
    GameObject gameObject { get; }
    Transform transform { get; }
    void Use();
}