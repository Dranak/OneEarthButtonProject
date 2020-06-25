using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlocsMerger))]
public class BlocsMergerEditor : Editor
{
    BlocsMerger bM;
    private void OnEnable()
    {
        bM = target as BlocsMerger;
        bM.mergedBloc = CreateInstance<BlocsStorageScriptableObject>();
        bM.mergedBloc.storedBlocs = new List<Bloc>();
    }
    string mergedBlocName = "new bloc name";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        mergedBlocName = EditorGUILayout.TextField("Blocs Scriptable Name : ", mergedBlocName);
        GUI.color = Color.green;
        EditorGUI.BeginDisabledGroup(bM.blocsToMerge.Count < 2 || mergedBlocName == "new bloc name" || mergedBlocName == "");
        if (GUILayout.Button("Create Merged Scriptable"))
        {
            bM.mergedBloc.spawnablesPrefabs = bM.blocsToMerge[0].spawnablesPrefabs; // always the same prefabs ! warning
            foreach (BlocsStorageScriptableObject blocsStorage in bM.blocsToMerge)
                bM.mergedBloc.storedBlocs.AddRange(blocsStorage.storedBlocs);
            AssetDatabase.CreateAsset(bM.mergedBloc, bM.mergedBlocPath + mergedBlocName + ".asset");
        }
        EditorGUI.EndDisabledGroup();
    }
}
