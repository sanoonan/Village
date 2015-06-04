using UnityEngine;
using System.Collections;
public class ItemCue : MemoryCue
{
    public ItemStatus Status;
    public CharacterCue Owner;
    internal Rigidbody RigidBody;

    public float Durability = 0.25f;

    void Awake()
    {
        if (CachedTransform == null)
            CachedTransform = transform.parent;

        RigidBody = CachedTransform.GetComponent<Rigidbody>();
    }
}
