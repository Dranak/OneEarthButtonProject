using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlocsMerger : MonoBehaviour
{
    public List<BlocsStorageScriptableObject> blocsToMerge;
    public string mergedBlocPath = "Assets/_project/CreatorTool/LD/";
    [HideInInspector] [SerializeField] public BlocsStorageScriptableObject mergedBloc;
}
