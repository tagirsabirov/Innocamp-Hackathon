using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class PuzzleLayout : MonoBehaviour
{
    public PuzzlePiece[] pieces = new PuzzlePiece[0];

    //Each piece when they are destroyed try to update the pieces array, problem is if the system is destroyed, that
    //spawned an assert. So we use that bool to know if the system is getting destroyed.
    public bool Destroyed { get; private set; }
    
    void OnDestroy()
    {
        Destroyed = true;
    }

    [ContextMenu("Debug Reveal Children")]
    void RevealChildren()
    {
        foreach (var p in pieces)
        {
            if(p != null)
                p.gameObject.hideFlags = HideFlags.None;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PuzzleLayout))]
public class PuzzleLayoutEditor : Editor
{
    PuzzleLayout m_PuzzleLayout;
    
    bool m_EditingLayout = false;
    
    List<PuzzlePieceBox> m_AvailablePieces = new List<PuzzlePieceBox>();

    PuzzlePieceBox m_CurrentBox = null;

    Vector2 m_PuzzleBoxSelectionScroll;
    Vector2 m_ObjectSelectScroll;

    PuzzlePiece m_SelectedPiece = null;   
    PuzzlePiece m_CurrentInstance = null;
    int m_CurrentUsedExit = 0;

    int m_EditingMode = 0;

    Vector3 m_CurrentScale = Vector3.one;

    SerializedProperty m_PieceProperty;

    Material m_HighlightMaterial;
    
    void OnEnable()
    {
        EditorApplication.playModeStateChanged += EditorApplicationOnPlayModeStateChanged;
    
        m_PuzzleLayout = target as PuzzleLayout;
 
        var assets = AssetDatabase.FindAssets("t:PuzzlePieceBox");
        foreach (var a in assets)
        {
            string path = AssetDatabase.GUIDToAssetPath(a);
            PuzzlePieceBox palette = AssetDatabase.LoadAssetAtPath<PuzzlePieceBox>(path);
            
            m_AvailablePieces.Add(palette);
        }

        m_PieceProperty = serializedObject.FindProperty("pieces");

        m_HighlightMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));

        m_HighlightMaterial.color = new Color32(255,238,0, 255);
        m_HighlightMaterial.SetInt("ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);

        foreach (var p in m_PuzzleLayout.pieces)
        {
            if(p != null)
                p.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }
    }

    void EditorApplicationOnPlayModeStateChanged(PlayModeStateChange obj)
    {
        Clean();
    }

    void OnDisable()
    {
        EditorApplication.playModeStateChanged -= EditorApplicationOnPlayModeStateChanged;
        Clean();
    }

    void Clean()
    {
        if (m_CurrentInstance != null)
        {
            DestroyImmediate(m_CurrentInstance.gameObject);
            m_CurrentInstance = null;
        }
    }

    public override void OnInspectorGUI()
    {       
        bool editing = GUILayout.Toggle(m_EditingLayout, "Editing Layout", "Button");

        if (editing != m_EditingLayout)
        {
            if (!editing)
            {//disabled editing, cleanup
                if(m_CurrentInstance != null)
                    DestroyImmediate(m_CurrentInstance.gameObject);
                m_CurrentBox = null;
                m_CurrentInstance = null;
                m_SelectedPiece = null;
            }
            else
            {
                if (!SceneView.lastActiveSceneView.drawGizmos)
                {
                    if (EditorUtility.DisplayDialog("Warning", "Gizmos are globally disabled, which prevents the layout editing tools from working. Do you want to re-enable Gizmos?", "Yes", "No"))
                    {
                        SceneView.lastActiveSceneView.drawGizmos = true;
                    }
                    else
                    {
                        editing = false;
                    }
                }
            }

            m_EditingLayout = editing;
        }

        if (m_EditingLayout)
        {   
            EditorGUILayout.HelpBox("Press R to change which connector the piece use to connect to other piece", MessageType.Info);

            EditorGUILayout.BeginHorizontal();

            int editingMode = GUILayout.Toolbar(m_EditingMode, new[] { "Add", "Remove" }, GUILayout.Width(120));
            if (editingMode != m_EditingMode)
            {
                if (editingMode == 1)
                {
                    if(m_CurrentInstance != null)
                        DestroyImmediate(m_CurrentInstance.gameObject);

                    m_SelectedPiece = null;
                }

                m_EditingMode = editingMode;
            }

            if (m_CurrentInstance != null)
            {
                EditorGUILayout.LabelField("Flip : ", GUILayout.Width(32));
                EditorGUI.BeginChangeCheck();

                bool flipX = GUILayout.Toggle(m_CurrentScale.x < 0, "X", "button", GUILayout.Width(32));
                bool flipY = GUILayout.Toggle(m_CurrentScale.y < 0, "Y", "button", GUILayout.Width(32));
                bool flipZ = GUILayout.Toggle(m_CurrentScale.z < 0, "Z", "button", GUILayout.Width(32));

                GUILayout.FlexibleSpace();

                if (EditorGUI.EndChangeCheck())
                {
                    m_CurrentScale = new Vector3(flipX ? -1 : 1, flipY ? -1 : 1, flipZ ? -1 : 1);
                    m_CurrentInstance.transform.localScale = m_CurrentScale;
                }   
            }
            
            EditorGUILayout.EndHorizontal();
            
            //we repaint all scene view to be sure they get a notification so they can "steal" focus in edit mode
            SceneView.RepaintAll();
        }
        
        GUILayout.BeginHorizontal();
        
        GUILayout.BeginVertical();
        GUILayout.Label("Boxes");
        m_PuzzleBoxSelectionScroll = GUILayout.BeginScrollView(m_PuzzleBoxSelectionScroll);
        
        foreach (var p in m_AvailablePieces)
        {
            GUI.enabled = m_CurrentBox != p;
            if (GUILayout.Button(p.name))
            {
                if (!m_EditingLayout)
                {
                    if (!SceneView.lastActiveSceneView.drawGizmos)
                    {
                        if (EditorUtility.DisplayDialog("Warning", "Gizmos are globally disabled, which prevent the layout editing tool to work. Do you want to re-enable Gizmos?", "Yes", "No"))
                        {
                            SceneView.lastActiveSceneView.drawGizmos = true;
                            m_EditingLayout = true;
                        }
                    }
                    else
                    {
                        m_EditingLayout = true;
                    }
                }

                if (m_EditingLayout)
                    m_CurrentBox = p;
            }
        }

        GUI.enabled = true;

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        
        GUILayout.BeginScrollView(m_PuzzleBoxSelectionScroll, GUILayout.Width(72*3));
        GUILayout.BeginVertical();
        
        if (m_CurrentBox != null)
        {
            bool horizontalOpen = false;

            for (int i = 0; i < m_CurrentBox.Pieces.Length; ++i)
            {
                PuzzlePiece part = m_CurrentBox.Pieces[i];
                
                if (i % 3 == 0 && i != 0)
                {
                    GUILayout.EndHorizontal();
                    horizontalOpen = false;
                }
                
                if (!horizontalOpen)
                {
                    GUILayout.BeginHorizontal();
                    horizontalOpen = true;
                }

                bool wrapValue = GUI.skin.button.wordWrap;
                
                var renderers = part.gameObject.GetComponentInChildren<Renderer>();
                GUIContent content = new GUIContent();

                if (renderers != null)
                    content.image = AssetPreview.GetAssetPreview(part.gameObject);
                else
                {
                    content.text = part.gameObject.name;
                    GUI.skin.button.wordWrap = true;
                }

                GUI.enabled = part != m_SelectedPiece;
                if (GUILayout.Button(content,  GUILayout.Width(64), GUILayout.Height(64)))
                {
                    if (m_EditingMode == 1)
                        m_EditingMode = 0;
                    
                    m_SelectedPiece = part;
                    
                    if(m_CurrentInstance != null)
                        DestroyImmediate(m_CurrentInstance.gameObject);

                    m_CurrentInstance = Instantiate(m_SelectedPiece, m_PuzzleLayout.transform);
                    m_CurrentInstance.gameObject.isStatic = false;
                    m_CurrentInstance.gameObject.tag = "EditorOnly";
                    m_CurrentInstance.name = "TempInstance";
                    m_CurrentUsedExit = 0;

                    m_CurrentInstance.transform.localScale = m_CurrentScale;
                }
                
                GUI.skin.button.wordWrap = wrapValue;
            }
            
            if(horizontalOpen)
                GUILayout.EndHorizontal();
        }
        
        GUI.enabled = true;
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        
        GUILayout.EndHorizontal();
    }

    void OnSceneGUI()
    {
        if (m_EditingLayout)
        {
            if (m_EditingMode == 0)
            {
                AddPiece();
            }
            else
            {
                RemovePiece();
            }
        }
    }

    void RemovePiece()
    {
        int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
        
        if(GUIUtility.hotControl == 0)
            HandleUtility.AddDefaultControl(controlID);
        
        if (m_CurrentInstance != null)
        {
            m_CurrentInstance.gameObject.SetActive(false);
        }
        
        var mousePos = Event.current.mousePosition;
        PuzzlePiece closestPiece = null;
        Bounds closestBound = new Bounds();
               
        float closestSqrDist = float.MaxValue;
        for (int i = 0; i < m_PuzzleLayout.pieces.Length; ++i)
        {
            PuzzlePiece r = m_PuzzleLayout.pieces[i];           
                    
            if(r == null)
                continue;
            
            //This bit is inneficient, but should be enough for our purpose here in the kit. In very big scene
            //it could slow down the editing process. Bound should probably be stored in local space or better should
            //find a way to use the built-in picking but that require more complexity than necessary for those small kit
            Bounds b = new Bounds();

            Renderer[] renderers = r.GetComponentsInChildren<Renderer>();

            if (renderers.Length > 0)
            {
                for (int k = 0; k < renderers.Length; ++k)
                {
                    if (k == 0)
                    {
                        b = renderers[k].bounds;
                    }
                    else
                    {
                        b.Encapsulate(renderers[k].bounds);
                    }
                }
            }
            else
            {
                //if the piece got no collider, it may be an "empty" piece used to introduce gap, so instead look for
                //a collider to find it's size.
                Collider[] colliders = r.GetComponentsInChildren<Collider>();
                for (int k = 0; k < colliders.Length; ++k)
                {
                    if (k == 0)
                    {
                        b = colliders[k].bounds;
                    }
                    else
                    {
                        b.Encapsulate(colliders[k].bounds);
                    }
                }
            }

            var guiPts = HandleUtility.WorldToGUIPoint(b.center);

            float dist = (guiPts - mousePos).sqrMagnitude;

            if (dist < closestSqrDist)
            {
                closestSqrDist = dist;
                closestPiece = r;
                closestBound = b;
            }
        }

        if (closestPiece != null)
        {
            if (Event.current.type == EventType.Repaint)
            {
                MeshFilter filter = closestPiece.GetComponentInChildren<MeshFilter>();

                if (filter != null)
                {
                    m_HighlightMaterial.SetPass(0);
                    Graphics.DrawMeshNow(filter.sharedMesh, closestPiece.transform.localToWorldMatrix);
                }
               
                Handles.DrawWireCube(closestBound.center, closestBound.size);
            }
            else if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && GUIUtility.hotControl == 0)
            {
                closestPiece.Removed();
                Undo.DestroyObjectImmediate(closestPiece.gameObject);
            }
            else
            {
                SceneView.currentDrawingSceneView.Repaint();
            }
        }
    }

    void AddPiece()
    {
        if (m_CurrentInstance != null)
        {
            m_CurrentInstance.gameObject.SetActive(true);
            
            //Since the scene view is not having focus after we choose a new room, pressing R won't rotate it until
            //we click on the scene view. So we force focus on windows. But we only do it if the cursor is above the
            //scene view otherwise we mess focus on OSX with the scene view always stealing focus from other app
            if(SceneView.currentDrawingSceneView.position.Contains(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)))
                SceneView.currentDrawingSceneView.Focus();
            
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            
            if(GUIUtility.hotControl == 0)
                HandleUtility.AddDefaultControl(controlID);

            if (Event.current.GetTypeForControl(controlID) == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.R)
                {
                    m_CurrentUsedExit = m_CurrentUsedExit + 1 >= m_CurrentInstance.Connectors.Length ? 0 : m_CurrentUsedExit + 1;
                }
            }


            PuzzlePiece currentCLosestPiece = null;
            int currentClosestExit = -1;
            
            if (m_PuzzleLayout.pieces.Length == 0)
            {//if we have no piece, we force the instance in 0,0,0, as it's the seed piece
                m_CurrentInstance.transform.position = m_PuzzleLayout.transform.TransformPoint(Vector3.zero);
            }
            else
            {
                var mousePos = Event.current.mousePosition;
               
                float closestSqrDist = float.MaxValue;
                for (int i = 0; i < m_PuzzleLayout.pieces.Length; ++i)
                {
                    PuzzlePiece r = m_PuzzleLayout.pieces[i];
                    
                    if(r == null)
                        continue;
                    
                    for (int k = 0; k < r.Connectors.Length; ++k)
                    {
                        if (r.ConnectorConnections[k] != null)
                            continue;
                        
                        var guiPts = HandleUtility.WorldToGUIPoint(r.Connectors[k].transform.position);

                        float dist = (guiPts - mousePos).sqrMagnitude;

                        if (dist < closestSqrDist)
                        {
                            closestSqrDist = dist;
                            currentCLosestPiece = r;
                            currentClosestExit = k;
                        }
                    }
                }

                if (currentCLosestPiece != null)
                {
                    m_CurrentInstance.transform.rotation = Quaternion.identity;
                    
                    Transform closest = currentCLosestPiece.Connectors[currentClosestExit];
                    Transform usedExit = m_CurrentInstance.Connectors[m_CurrentUsedExit];

                    Quaternion targetRotation = Quaternion.LookRotation(-closest.forward, closest.up);
                    Quaternion difference = targetRotation * Quaternion.Inverse(usedExit.rotation);
                    
                    Quaternion rotation = m_CurrentInstance.transform.rotation * difference;
                    m_CurrentInstance.transform.rotation = rotation;

                    m_CurrentInstance.transform.position = closest.position + m_CurrentInstance.transform.TransformVector(-usedExit.transform.localPosition);
                }
            }
            
            
            //if hot control is not 0, that mean we clicked a gizmo and we don't want that.
            if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && GUIUtility.hotControl == 0)
            {
                var c = PrefabUtility.InstantiatePrefab(m_SelectedPiece) as PuzzlePiece;
                c.gameObject.hideFlags = HideFlags.HideInHierarchy;
                c.transform.SetParent(m_PuzzleLayout.transform, false);
                
                c.transform.position = m_CurrentInstance.transform.position;
                c.transform.rotation = m_CurrentInstance.transform.rotation;
                c.transform.localScale = m_CurrentInstance.transform.localScale;
                
                c.name = m_SelectedPiece.gameObject.name;
                c.gameObject.isStatic = true;
                
                c.Placed(m_PuzzleLayout);
                   
                int i = Undo.GetCurrentGroup();
                Undo.SetCurrentGroupName("Added new piece");

                Undo.RegisterCreatedObjectUndo(c.gameObject, "Added new piece");

                m_PieceProperty.serializedObject.Update();
                
                m_PieceProperty.InsertArrayElementAtIndex(m_PieceProperty.arraySize);
                m_PieceProperty.GetArrayElementAtIndex(m_PieceProperty.arraySize - 1).objectReferenceValue = c;

                if (currentCLosestPiece != null)
                {
                    SerializedObject newObj = new SerializedObject(c);
                    SerializedObject currentObj = new SerializedObject(currentCLosestPiece);
                    
                    var propNew = newObj.FindProperty("ConnectorConnections");
                    var propCurrent = currentObj.FindProperty("ConnectorConnections");
                    
                    newObj.Update();
                    currentObj.Update();

                    propCurrent.GetArrayElementAtIndex(currentClosestExit).objectReferenceValue = c;
                    propNew.GetArrayElementAtIndex(m_CurrentUsedExit).objectReferenceValue = currentCLosestPiece;

                    newObj.ApplyModifiedProperties();
                    currentObj.ApplyModifiedProperties();
                }

                Undo.CollapseUndoOperations(i);

                m_PieceProperty.serializedObject.ApplyModifiedProperties();
            }
        }
    }

    void ObjectPrefabUndo(Object obj, string name)
    {
        bool prefab = PrefabUtility.IsPartOfAnyPrefab(obj);
        if (prefab)
            PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
        else
            Undo.RecordObject(obj, name);
    }
}
#endif