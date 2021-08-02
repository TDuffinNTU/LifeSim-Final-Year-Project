using UnityEngine;
using UnityEditor;

/// <summary>
/// In-editor button to reset the map generator
/// </summary>
[CustomEditor (typeof(GroundGenerator))]
public class GroundGeneratorInspector : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GroundGenerator script = (GroundGenerator)target;
        if (GUILayout.Button("Regenerate")) {
            script.Reset();
        } ;
    }
}
