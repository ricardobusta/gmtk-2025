using Unity.Mathematics;
using UnityEngine;
using UnityEditor;
using UnityEngine.Splines;

public class CreateParallelSplines : MonoBehaviour
{
    [MenuItem("Tools/Splines/Create Parallel Splines")]
    private static void CreateParallelTracks()
    {
        // Get selected GameObject
        var selected = Selection.activeGameObject;
        if (selected == null || !selected.TryGetComponent<SplineContainer>(out var splineContainer))
        {
            Debug.LogError("Please select a GameObject with a SplineContainer.");
            return;
        }

        var numberOfTracks = 3; // Total tracks including the original
        var spacing = 1.5f;     // Distance between tracks (in world units)

        var parent = selected.transform.parent;
        var originalSpline = splineContainer.Spline;

        for (var i = 1; i < numberOfTracks; i++)
        {
            var offset = spacing * i;

            // Create new GameObject
            var newTrack = new GameObject($"{selected.name}_Track_{i + 1}");
            newTrack.transform.SetParent(parent);
            newTrack.transform.position = selected.transform.position;
            newTrack.transform.rotation = selected.transform.rotation;

            var newContainer = newTrack.AddComponent<SplineContainer>();
            var newSpline = new Spline(originalSpline);

            foreach (var knot in originalSpline)
            {
                // World-space tangent direction
                float3 tangentDir = math.normalize(math.rotate(knot.Rotation, knot.TangentOut));
                float3 right = math.normalize(math.cross(new float3(0, 1, 0), tangentDir));

                // Offset knot position
                float3 offsetPosition = knot.Position + right * offset;

                // Offset tangents in world space
                float3 tangentInWorld = math.rotate(knot.Rotation, knot.TangentIn) + right * offset;
                float3 tangentOutWorld = math.rotate(knot.Rotation, knot.TangentOut) + right * offset;

                // Convert world tangents back to local space (relative to new position and rotation)
                float3 newTangentIn = math.mul(math.inverse(knot.Rotation), tangentInWorld - offsetPosition);
                float3 newTangentOut = math.mul(math.inverse(knot.Rotation), tangentOutWorld - offsetPosition);

                var newKnot = new BezierKnot(
                    offsetPosition,
                    newTangentIn,
                    newTangentOut
                )
                {
                    Rotation = knot.Rotation
                };

                newSpline.Add(newKnot);
            }

            newContainer.Spline = newSpline;
        }

        Debug.Log($"{numberOfTracks - 1} parallel spline(s) created.");
    }
}
