using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WFCSpawner))]
public class WFCSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Collapse grid"))
        {
            (target as WFCSpawner).CollapseGrid();
        }
        if (GUILayout.Button("Stop collapsing"))
        {
            (target as WFCSpawner).StopCollapsing();
        }
        if (GUILayout.Button("Clear blocks"))
        {
            (target as WFCSpawner).ClearBlocks();
        }
        if (GUILayout.Button("Load from XML"))
        {
            (target as WFCSpawner).LoadSamplesFromXML(EditorUtility.OpenFilePanel("XML file that contains samples", "", "xml"));
        }
    }
}
