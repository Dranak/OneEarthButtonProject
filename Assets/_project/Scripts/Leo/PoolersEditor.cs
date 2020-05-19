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
        if (GUILayout.Button("Create sub-Pools from Scriptable"))
        {
            // destroy all previous obj if any
            while (pC.transform.childCount > 0)
            {
                Undo.DestroyObjectImmediate(pC.transform.GetChild(0).gameObject);
            }

            foreach (GameObject spawnablePrefab in pC.selectedBlocsScriptable.obstaclesPrefabs)
            {
                var pool = new GameObject(spawnablePrefab.name + "_T");
                pool.transform.SetParent(pC.transform);
                for (int i = 0; i < pC.poolsSize; ++i)
                {
                    GameObject spawnableObj = PrefabUtility.InstantiatePrefab(spawnablePrefab, pool.transform) as GameObject;
                    spawnableObj.name += i;
                    spawnableObj.SetActive(false);
                }
            }
        }
    }
}