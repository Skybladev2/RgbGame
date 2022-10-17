using UnityEditor;

[CustomEditor(typeof(PipeScript))]
public class PipeInspector : Editor {

    public override void OnInspectorGUI()
    {
        PipeScript pipeScript = (PipeScript)target;
        EditorGUILayout.ObjectField("End node", pipeScript.GetEndNodeScript() as NodeScript, typeof(NodeScript), true);
    }
}
