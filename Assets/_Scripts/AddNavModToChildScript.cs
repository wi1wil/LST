using UnityEngine;
using UnityEditor;
using NavMeshPlus.Components;

public class AddNavModToChildScript : MonoBehaviour
{
    [MenuItem("Tools/Add NavMeshModifier to Children")]
    public static void AddModifiersToEnvironment()
    {
        GameObject environment = GameObject.Find("Environment");
        if (environment == null) return;

        foreach (Transform child in environment.transform)
        {
            var modifier = child.GetComponent<NavMeshModifier>();
            if (modifier == null)
            {
                modifier = child.gameObject.AddComponent<NavMeshModifier>();
            }

            modifier.overrideArea = true;
            modifier.area = 1;
        }

        var surface = GameObject.FindObjectOfType<NavMeshSurface>();
        if (surface != null)
        {
            surface.BuildNavMesh();
        }
    }

    [MenuItem("Tools/Add Build NavMesh")]
    public static void AddBuildNavMesh()
    {
        var surface = GameObject.FindObjectOfType<NavMeshSurface>();
        if (surface != null)
        {
            surface.BuildNavMesh();
        }
    }
}