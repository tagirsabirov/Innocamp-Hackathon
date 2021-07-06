using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class PuzzlePiece : MonoBehaviour
{
   public Transform[] Connectors;

   public PuzzlePiece[] ConnectorConnections;

   [HideInInspector]
   public PuzzleLayout Owner;

   /// <summary>
   /// Called by the editor script that place piece to initialize the ExitUsed array to match Exits
   /// </summary>
   public void Placed(PuzzleLayout layoutOwner)
   {
      Owner = layoutOwner;
      ConnectorConnections = new PuzzlePiece[Connectors.Length];
   }
#if UNITY_EDITOR
   public void  Removed()
   {   
      if (ConnectorConnections != null)
      {
         for (int i = 0; i < ConnectorConnections.Length; ++i)
         {
            if (ConnectorConnections[i] != null)
            {
               SerializedObject otherObj = new SerializedObject(ConnectorConnections[i]);
               var connectorProp = otherObj.FindProperty(nameof(ConnectorConnections));

               for (int k = 0; k < connectorProp.arraySize; ++k)
               {
                  var prop = connectorProp.GetArrayElementAtIndex(k);

                  if (prop.objectReferenceValue == this)
                  {
                     prop.objectReferenceValue = null;
                     prop.serializedObject.ApplyModifiedProperties();
                  }
               }
            }
         }
      }

      if (Owner != null && !Owner.Destroyed)
      {
            SerializedObject ownerObject = new SerializedObject(Owner);
            
            var piecesProp = ownerObject.FindProperty(nameof(Owner.pieces));

            for (int i = 0; i < piecesProp.arraySize; ++i)
            {
               var prop = piecesProp.GetArrayElementAtIndex(i);

               if (prop.objectReferenceValue == this)
               {
                  piecesProp.DeleteArrayElementAtIndex(i);
                  piecesProp.DeleteArrayElementAtIndex(i);
                  break;
               }
            }
            
            ownerObject.ApplyModifiedProperties();
      }
   }
#endif
}