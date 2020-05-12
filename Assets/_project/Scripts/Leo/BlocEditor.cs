using UnityEditor;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

[CustomEditor(typeof(BlocCreator))]
//[CanEditMultipleObjects]
public class BlocEditor : Editor
{
    PhysicsScene2D editorPS;
    // painter
    Vector3 worldCursor;
    Texture2D[] palleteImages;
    GameObject stamp;
    BlocCreator bc;
    List<GameObject> erase = new List<GameObject>();
    List<Bounds> overlaps = new List<Bounds>();
    List<GameObject> overlappedGameObjects = new List<GameObject>();

    private void OnEnable()
    {
        stamp = new GameObject("Stamp");
        stamp.hideFlags = HideFlags.HideAndDontSave;
        bc = target as BlocCreator;
        if (bc.SelectedPrefab != null)
            CreateNewStamp();

        var scriptObject = bc.gameObject;
        editorPS = PhysicsSceneExtensions2D.GetPhysicsScene2D(scriptObject.scene);
    }

    private void OnDisable()
    {
        if (stamp != null)
            DestroyImmediate(stamp);
    }

    void RefreshPaletteImages(BlocCreator bc)
    {
        if (palleteImages == null || palleteImages.Length != bc.prefabPallete.Length)
        {
            palleteImages = new Texture2D[bc.prefabPallete.Length];
            for (var i = 0; i < bc.prefabPallete.Length; i++)
            {
                palleteImages[i] = AssetPreview.GetAssetPreview(bc.prefabPallete[i]);
            }
        }
    }

    string blocName = "Enter Bloc Name"; // bloc name to store
    Bloc.BlocArea blocArea = Bloc.BlocArea.COUNTRY; // bloc area to store
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (bc.rootTransform == null)
        {
            EditorGUILayout.HelpBox("You must assign the root transform for new painted instances.", MessageType.Error);
            bc.rootTransform = (Transform)EditorGUILayout.ObjectField("Root Transform", bc.rootTransform, typeof(Transform), true);
            return;
        }
        EditorGUILayout.HelpBox("Stamp: Left Click\nErase: Ctrl + Left Click\nRotate: Shift + Scroll", MessageType.Info);
        base.OnInspectorGUI();

        GUILayout.Space(16);
        if (bc.prefabPallete != null && bc.prefabPallete.Length > 0)
        {
            RefreshPaletteImages(bc);
            var tileSize = 96;
            var xCount = Mathf.FloorToInt((EditorGUIUtility.currentViewWidth / tileSize + 1) / 2);
            var gridHeight = GUILayout.Height(palleteImages.Length / xCount * tileSize);
            var buttonStyle = EditorStyles.miniButton;
            buttonStyle.fixedHeight = palleteImages.Length / xCount * tileSize;
            var newIndex = GUILayout.SelectionGrid(bc.selectedPrefabIndex, palleteImages, xCount, buttonStyle, gridHeight);
            if (newIndex != bc.selectedPrefabIndex)
            {
                bc.selectedPrefabIndex = newIndex;
                CreateNewStamp();
            }
            GUILayout.Space(16);
        }

        GUILayout.Label("Bloc Saving:");
        GUILayout.Space(8);
        blocName = EditorGUILayout.TextField("Bloc Name", blocName);
        blocArea = (Bloc.BlocArea)EditorGUILayout.EnumPopup("Bloc Area : ", blocArea);
        if (GUILayout.Button("Save Bloc to Scriptable"))
        {
            var obstacles = bc.rootTransform.GetComponentsInChildren<Obstacle>().Select(o => o.obstacleParameters).ToArray();
            Bloc newBloc = new Bloc(blocArea, bc.rootTransform.childCount, 0, blocName, obstacles);
            var scriptableStoredBlocs = bc.blocScriptable.storedBlocs;
            var presavedBloc = bc.blocScriptable.storedBlocs.FirstOrDefault(b => b.blocName == blocName);
            if (presavedBloc != null)
            {
                scriptableStoredBlocs[scriptableStoredBlocs.IndexOf(presavedBloc)] = newBloc;
            }
            else
                bc.blocScriptable.storedBlocs.Add(newBloc);
        }
    }

    void CreateNewStamp()
    {
        while (stamp.transform.childCount > 0)
            DestroyImmediate(stamp.transform.GetChild(0).gameObject);

        GameObject dummy;
        dummy = PrefabUtility.InstantiatePrefab(bc.SelectedPrefab as GameObject) as GameObject;
        foreach (var c in dummy.GetComponentsInChildren<Collider>())
            c.enabled = false;
        dummy.transform.parent = stamp.transform;
        dummy.transform.localPosition = Vector3.zero;
        dummy.transform.localRotation = Quaternion.identity;
    }

    void PerformErase()
    {
        foreach (var g in erase)
            Undo.DestroyObjectImmediate(g);
        erase.Clear();
    }

    void PerformStamp()
    {
        var dummy = stamp.transform.GetChild(0);
        if (dummy.gameObject.activeSelf)
        {
            var stampObject = dummy;
            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(stampObject.gameObject);
            var g = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            Undo.RegisterCreatedObjectUndo(g, "Stamp");

            g.transform.position = stampObject.position;
            if (bc.rootTransform != null)
            {
                g.transform.parent = bc.rootTransform;
                g.isStatic = bc.rootTransform.gameObject.isStatic;
            }

            var rotation = dummy.transform.GetChild(0).rotation;
            var offset = dummy.transform.GetChild(0).localPosition;

            g.transform.GetChild(0).rotation = rotation;
            g.transform.GetChild(0).localPosition = offset;

            var obstacle = g.GetComponent<Obstacle>();
            var obstacleIndex = GetIndexFromPrefabList(bc.blocScriptable.obstaclesPrefabs, g);
            obstacle.SetObstacle(obstacle.obstacleParameters.Size, new Vector2Int((int)g.transform.localPosition.x, (int)g.transform.localPosition.y), rotation, offset, obstacleIndex);
        }
    }

    uint GetIndexFromPrefabList(in List<GameObject> prefabList, in GameObject chosenObject)
    {
        var chosenPrefabParent = PrefabUtility.GetCorrespondingObjectFromSource(chosenObject);
        GameObject correspondingP = prefabList.FirstOrDefault(w => w == chosenPrefabParent);
        if (!correspondingP) throw new System.Exception("Couldn't find prefab correspondance, this is not a prefab");
        var indexFound = prefabList.IndexOf(correspondingP);
        if (indexFound < 0) throw new System.Exception("Couldn't find prefab in prefab list");
        else return (uint)indexFound;
    }

    void RotateStamp(Vector2 delta)
    {
        var obsBody = stamp.transform.GetChild(0).GetChild(0);
        obsBody.Rotate(Vector3.back , delta.y);

        var dummyBounds = obsBody.gameObject.GetBoxColliderFixedBounds();
        Vector2 dummyOffset = stamp.transform.position - dummyBounds.min;
        obsBody.localPosition += (Vector3)dummyOffset;
    }

    private void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseMove) SceneView.RepaintAll();

        var isErasing = Event.current.control;
        var controlId = GUIUtility.GetControlID(FocusType.Passive);
        Vector3 mousePosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        mousePosition = ray.origin;

        // Handling raycast
        int creatorMask = bc.layerMask << 0;
        RaycastHit2D hit2d = editorPS.Raycast(ray.origin, ray.direction, Mathf.Infinity, creatorMask);

        if (hit2d.collider)
        {
            //worldCursor = hit2d.point;
            var snappedMousePos = new Vector3(Mathf.Floor(mousePosition.x), Mathf.Floor(mousePosition.y));
            var visibleMousePos = snappedMousePos + new Vector3(.5f, .5f);
            Handles.DrawWireDisc(visibleMousePos, -ray.direction, .5f); // white disc is always visible
            OverlapCapsule(visibleMousePos, 1, bc.layerMask);
            if (isErasing)
                DrawEraser(snappedMousePos);
            else
                DrawStamp(snappedMousePos);
        }

        switch (Event.current.type)
        {
            case EventType.ScrollWheel:
                if (Event.current.shift)
                {
                    RotateStamp(Event.current.delta.normalized * 11.25f);
                    Event.current.Use();
                }
                break;
            case EventType.MouseDown:
                //If not using the default orbit mode...
                if (Event.current.button == 0 && !Event.current.alt)
                {
                    if (isErasing)
                        PerformErase();
                    else
                        PerformStamp();
                    
                    GUIUtility.hotControl = controlId;
                    Event.current.Use();
                }
                break;
        }
    }

    private void OverlapCapsule(Vector2 center, float brushRadius, LayerMask layerMask)
    {
        overlaps.Clear();
        overlappedGameObjects.Clear();
        if (bc.collisionTest == BlocCreator.CollisionTest.ColliderBounds)
        {
            foreach (var c in Physics2D.OverlapCapsuleAll(center, Vector2.one, CapsuleDirection2D.Vertical, 0f))
            {
                if (c.transform.parent == bc.rootTransform)
                {
                    overlaps.Add(c.bounds);
                    overlappedGameObjects.Add(c.gameObject);
                }
            }
        }
        if (bc.collisionTest == BlocCreator.CollisionTest.RendererBounds)
        {
            var capsule = new Bounds(center, new Vector3(brushRadius, brushRadius, brushRadius));
            for (var i = 0; i < bc.rootTransform.childCount; i++)
            {
                var child = bc.rootTransform.GetChild(i);
                var bounds = child.gameObject.GetRendererBounds();
                if (capsule.Intersects(bounds))
                {
                    overlaps.Add(bounds);
                    overlappedGameObjects.Add(child.gameObject);
                }
            }
        }
    }

    void DrawStamp(Vector3 center)
    {
        stamp.transform.parent = bc.transform;

        stamp.transform.position = center;

        stamp.transform.rotation = Quaternion.identity;

        var dummy = stamp.transform.GetChild(0).gameObject;
        dummy.SetActive(true);// .gameObject.SetActive(true);

        var bounds = stamp.GetRendererBounds();
        var childVolume = bounds.size.x * bounds.size.y * bounds.size.z;
        foreach (var b in overlaps)
        {
            if (b.Intersects(bounds))
            {
                var overlapVolume = b.size.x * b.size.y * b.size.z;
                var intersection = Intersection(b, bounds);
                var intersectionVolume = intersection.size.x * intersection.size.y * intersection.size.z;
                var maxIntersection = Mathf.Max(intersectionVolume / overlapVolume, intersectionVolume / childVolume);
                if (maxIntersection > bc.maxIntersectionVolume)
                {
                    dummy.SetActive(false);
                }
            }
        }
    }

    Bounds Intersection(Bounds A, Bounds B)
    {
        var min = new Vector3(Mathf.Max(A.min.x, B.min.x), Mathf.Max(A.min.y, B.min.y), Mathf.Max(A.min.z, B.min.z));
        var max = new Vector3(Mathf.Min(A.max.x, B.max.x), Mathf.Min(A.max.y, B.max.y), Mathf.Min(A.max.z, B.max.z));
        return new Bounds(Vector3.Lerp(min, max, 0.5f), max - min);
    }

    void DrawEraser(Vector3 center)
    {
        erase.Clear();
        stamp.transform.GetChild(0).gameObject.SetActive(false);

        stamp.transform.position = center;
        stamp.transform.rotation = Quaternion.identity;

        for (var i = 0; i < overlaps.Count; i++)
        {
            var h = overlaps[i];
            Handles.color = Color.red;
            Handles.DrawWireDisc(h.center, Vector3.back, h.extents.magnitude);
            erase.Add(overlappedGameObjects[i]);
        }
    }

    public override bool RequiresConstantRepaint() { return true; }
}
