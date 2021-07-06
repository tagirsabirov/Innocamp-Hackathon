using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingHammer : InteractivePuzzlePiece<HingeJoint>
{
    [Range(1f, 10f)]
    public float power = 5f;
    
    void Awake ()
    {
        rb.mass = power;
    }
    
    protected override void ApplyActiveState ()
    {
        physicsComponent.useMotor = false;
    }

    protected override void ApplyInactiveState ()
    {
        physicsComponent.useMotor = true;
    }
}