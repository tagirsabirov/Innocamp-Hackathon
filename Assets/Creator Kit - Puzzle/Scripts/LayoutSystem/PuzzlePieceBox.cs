using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Puzzle Game/Puzzle Piece Box", fileName = "PuzzlePieceBox")]
public class PuzzlePieceBox : ScriptableObject
{
    public PuzzlePiece[] Pieces;
}
