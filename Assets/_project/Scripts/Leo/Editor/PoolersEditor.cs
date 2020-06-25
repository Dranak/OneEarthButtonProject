using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

[CustomEditor(typeof(PoolersCreator))]
public class PoolersEditor : Editor
{
    PoolersCreator pC;
    private void OnEnable()
    {
        pC = target as PoolersCreator;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        GUI.color = Color.green;
        EditorGUI.BeginDisabledGroup(!pC.selectedBlocsScriptable);
        if (GUILayout.Button("Create sub-Pools from Scriptable, and generate array of pools"))
        {
            // destroy all previous obj if any
            while (pC.transform.childCount > 0)
            {
                Undo.DestroyObjectImmediate(pC.transform.GetChild(0).gameObject);
            }

            foreach (GameObject spawnablePrefab in pC.selectedBlocsScriptable.spawnablesPrefabs)
            {
                var pool = new GameObject(spawnablePrefab.name + "_T");
                pool.transform.SetParent(pC.transform);
                for (int i = 0; i < pC.poolsSize * (1 + System.Convert.ToInt32(spawnablePrefab.name.Contains("bread"))*(pC.breadPoolMult-1)); ++i)
                {
                    GameObject spawnableObj = PrefabUtility.InstantiatePrefab(spawnablePrefab, pool.transform) as GameObject;
                    spawnableObj.name += i;
                    spawnableObj.SetActive(false);
                }
            }
            pC.SetSpawnablesPools();
        }
        EditorGUI.EndDisabledGroup();
    }
}