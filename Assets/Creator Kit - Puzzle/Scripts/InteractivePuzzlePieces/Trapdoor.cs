using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trapdoor : InteractivePuzzlePiece<ConstantForce>
{
    [Range(10f, 50f)]
    public float openSpeed = 12f;

    void Awake ()
    {
        physicsComponent.force = new Vector3(0f, openSpeed, 0f);
    }
    
    protected override void ApplyActiveState ()
    {
        physicsComponent.enabled = true;
    }

    protected override void ApplyInactiveState ()
    {
        physicsComponent.enabled = false;
    }
}
