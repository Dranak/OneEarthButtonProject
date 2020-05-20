using UnityEngine;

public static class GameObjectExtensions
{
    public static GameObject Dummy(this GameObject gameObject)
    {
        if (gameObject == null)
            return null;
        var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        var dummy = new GameObject(gameObject.name);
        dummy.transform.position = gameObject.transform.position;
        dummy.transform.rotation = gameObject.transform.rotation;
        foreach (var i in renderers)
        {
            var child = new GameObject(i.gameObject.name);
            child.transform.parent = dummy.transform;
            child.transform.position = i.transform.position;
            child.transform.rotation = i.transform.rotation;
            child.transform.localScale = i.transform.lossyScale;
            child.AddComponent<MeshFilter>().sharedMesh = i.GetComponent<MeshFilter>().sharedMesh;
            child.AddComponent<MeshRenderer>().sharedMaterial = i.sharedMaterial;
        }
        return dummy;
    }

    public static Bounds GetRendererBounds(this GameObject gameObject)
    {
        var bounds = new Bounds();
        var firstBounds = true;
        foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
        {
            if (firstBounds)
            {
                bounds = r.bounds;
                firstBounds = false;
            }
            bounds.Encapsulate(r.bounds);
        }
        return bounds;
    }

    public static Bounds GetBoxColliderFixedBounds(this GameObject gameObject)
    {
        var bounds = new Bounds();

        Vector2[] vertices = new Vector2[0];
        var collider = gameObject.GetComponent<Collider2D>();
        var colliderType = collider.GetType();

        if (colliderType == typeof(PolygonCollider2D))
            vertices = (collider as PolygonCollider2D).points;
        else if (colliderType == typeof(EdgeCollider2D))
            vertices = (collider as EdgeCollider2D).points;
        else if (colliderType == typeof(BoxCollider2D))
        {
            float top = collider.offset.y + (collider as BoxCollider2D).size.y / 2;
            float btm = collider.offset.y - (collider as BoxCollider2D).size.y / 2;
            float left = collider.offset.x - (collider as BoxCollider2D).size.x / 2;
            float right = collider.offset.x + (collider as BoxCollider2D).size.x / 2;
            vertices = new Vector2[4] { new Vector2(left, top), new Vector2(right, top), new Vector2(right, btm), new Vector2(left, btm) };
        }
        else if (colliderType == typeof(CircleCollider2D))
        {
            return collider.bounds;
        }

        if (vertices.Length <= 0) return bounds;

        // TransformPoint converts the local mesh vertice dependent on the transform
        // position, scale and orientation into a global position
        var min = gameObject.transform.TransformPoint(vertices[0]);
        var max = min;

        // Iterate through all vertices
        // except first one
        for (var i = 1; i < vertices.Length; ++i)
        {
            var V = gameObject.transform.TransformPoint(vertices[i]);

            // Go through X,Y of the Vector2
            for (var n = 0; n < 3; ++n)
            {
                max[n] = Mathf.Max(V[n], max[n]);
                min[n] = Mathf.Min(V[n], min[n]);
            }
        }

        bounds.SetMinMax(min, max);
        return bounds;
    }

    public static Bounds GetLocalBounds(this GameObject gameObject)
    {
        var rotation = gameObject.transform.rotation;
        gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        var bounds = new Bounds(gameObject.transform.position, Vector3.zero);
        foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>())
            bounds.Encapsulate(renderer.bounds);
        bounds.center = bounds.center - gameObject.transform.position;
        gameObject.transform.rotation = rotation;
        return bounds;
    }
}
