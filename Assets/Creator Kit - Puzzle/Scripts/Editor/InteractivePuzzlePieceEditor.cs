using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InteractivePuzzlePieceEditor<TComponent> : Editor
where TComponent : Component
{
    void OnEnable ()
    {
        InteractivePuzzlePiece<TComponent> puzzlePiece = (InteractivePuzzlePiece<TComponent>)target;
        puzzlePiece.rb.hideFlags = HideFlags.NotEditable;
        puzzlePiece.physicsComponent.hideFlags = HideFlags.NotEditable;
    }

    void OnDisable ()
    {
        InteractivePuzzlePiece<TComponent> puzzlePiece = (InteractivePuzzlePiece<TComponent>)target;
        if(puzzlePiece.rb != null)
            puzzlePiece.rb.hideFlags = HideFlags.None;
        if(puzzlePiece.physicsComponent != null)
            puzzlePiece.physicsComponent.hideFlags = HideFlags.None;
    }
}

[CustomEditor(typeof(Flipper))]
public class FlipperEditor : InteractivePuzzlePieceEditor<HingeJoint>
{}

[CustomEditor(typeof(SwingHammer))]
public class SwingHammerEditor : InteractivePuzzlePieceEditor<HingeJoint>
{}

[CustomEditor(typeof(Trapdoor))]
public class TrapdoorEditor : InteractivePuzzlePieceEditor<ConstantForce>
{}
