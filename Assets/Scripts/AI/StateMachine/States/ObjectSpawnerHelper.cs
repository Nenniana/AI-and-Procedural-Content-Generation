using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectSpawnerHelper
{
    static private Color color;
    static private Vector3 centerPosition;
    static private Vector2 spawnSizeCube;
    static private float spawnRadiusSphere;

    public List<GameObject> SpawnObjects(GameObject objectToSpawn, float spawnAmount, Vector3 spawnCenter, float spawnRadius, Quaternion quaternion, SpawnContainerType type)
    {
        centerPosition = spawnCenter;
        spawnRadiusSphere = spawnRadius;
        color = Color.white;

        return Construct(objectToSpawn, spawnAmount, spawnCenter, new Vector3 (spawnRadius, spawnRadius, spawnRadius), quaternion, type);
    }

    public List<GameObject> SpawnObjects(GameObject objectToSpawn, float spawnAmount, Vector3 spawnCenter, Vector2 spawnSizeVector, Quaternion quaternion, SpawnContainerType type)
    {
        centerPosition = spawnCenter;
        spawnSizeCube = spawnSizeVector;
        color = Color.white;

        return Construct(objectToSpawn, spawnAmount, spawnCenter, spawnSizeVector, quaternion, type);
    }

    private List<GameObject> Construct(GameObject objectToSpawn, float spawnAmount, Vector3 spawnCenter, Vector3 spawnSizeVector, Quaternion quaternion, SpawnContainerType type)
    {
        List<GameObject> objects = new List<GameObject>();

        for (int i = 0; i < spawnAmount; i++)
        {
            objects.Add(InstantiateObject(objectToSpawn, quaternion, GetPosition(type, spawnCenter, spawnSizeVector)));
        }

        return objects;
    }

    private Vector3 GetPosition (SpawnContainerType type, Vector3 spawnCenter, Vector3 spawnSize)
    {
        switch (type)
        {
            case SpawnContainerType.Circle:
                Vector3 circlePosition = Random.insideUnitCircle;
                return spawnCenter + new Vector3(circlePosition.x * spawnSize.x, circlePosition.y * spawnSize.y, circlePosition.z * spawnSize.z);
            case SpawnContainerType.Sphere:
                Vector3 spherePosition = Random.insideUnitSphere;
                return spawnCenter + new Vector3(spherePosition.x * spawnSize.x, spherePosition.y * spawnSize.y, spherePosition.z * spawnSize.z);
            case SpawnContainerType.Cube:
                return new Vector3(Random.Range(spawnCenter.x - spawnSize.x, spawnCenter.x + spawnSize.x),
                    Random.Range(spawnCenter.y - spawnSize.y, spawnCenter.y + spawnSize.y),
                    Random.Range(spawnCenter.z - spawnSize.z, spawnCenter.z + spawnSize.z));
            default:
                return spawnCenter;
        }
    }

    private static GameObject InstantiateObject(GameObject objectToSpawn, Quaternion quaternion, Vector3 position)
    {
        return GameObject.Instantiate(objectToSpawn, position, quaternion);
    }

    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Active)]
    static void DrawCubeSelected(MonoBehaviour scr, GizmoType gizmoType)
    {
        DrawGizmo();
    }

    static void DrawGizmo()
    {
        //Gizmos.DrawIcon(position, "MyScript Gizmo.tiff");
        Gizmos.color = new Color(color.r, color.g, color.b, 0.01f);
        if (spawnSizeCube != null && spawnSizeCube != Vector2.zero)
            Gizmos.DrawCube(centerPosition, spawnSizeCube*2);
        else
            Gizmos.DrawSphere(centerPosition, spawnRadiusSphere);
    }
}

public enum SpawnContainerType
{
    Circle,
    Sphere,
    Cube
}