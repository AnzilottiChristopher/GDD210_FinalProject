using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI() {
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.getViewRadius());
        Vector3 viewAngleA = fow.DirFromAngle(-fow.getViewAngle() / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.getViewAngle() / 2, false);
        
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.getViewRadius());
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.getViewRadius());
        
        Handles.color = Color.red;
        foreach (Transform visibleTarget in fow.getVisibleTargets()) 
        {
            Handles.DrawLine(fow.transform.position, visibleTarget.position);
        }

    }
}
