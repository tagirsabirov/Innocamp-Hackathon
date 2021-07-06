using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetReferencer : MonoBehaviour
{
    public ScriptableObject[] assets;

    void Awake ()
    {
        DontDestroyOnLoad (this);
    }
}
