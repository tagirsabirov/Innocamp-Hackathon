using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper : InteractivePuzzlePiece<HingeJoint>
{
    [Range(500f, 2000f)]
    public float power = 700f;

    void Awake ()
    {
        JointMotor flipperMotor = physicsComponent.motor;
        flipperMotor.targetVelocity = power;
        physicsComponent.motor = flipperMotor;
    }
    
    protected override void ApplyActiveState ()
    {
        physicsComponent.useMotor = true;
    }

    protected override void ApplyInactiveState ()
    {
        physicsComponent.useMotor = false;
    }
}
