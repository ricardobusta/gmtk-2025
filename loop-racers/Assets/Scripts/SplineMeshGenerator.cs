using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Rendering;

namespace Busta.LoopRacers
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class SplineMeshGenerator : MonoBehaviour
    {
        [Header("Inputs")] public SplineContainer spline;
        public Mesh sourceMesh;
        public float meshScale = 1f;
        public float meshMinX = -1f;
        public float meshMaxX = 1f;
        [Min(1)] public int repetitions = 10;
        [Range(0, 1)] public float sectionBeginAt = 0f;
        [Range(0, 1)] public float sectionEndAt = 1f;

        private Mesh _generatedMesh;

        private void OnValidate()
        {
            CleanMesh();
            GenerateMesh();
        }

        private void CleanMesh()
        {
            if (_generatedMesh != null)
            {
                DestroyImmediate(_generatedMesh);
                _generatedMesh = null;
            }
        }

        private void GenerateMesh()
        {
            if (spline == null || sourceMesh == null || repetitions <= 0)
                return;

            var filter = GetComponent<MeshFilter>();

            var srcVertices = sourceMesh.vertices;
            var srcNormals = sourceMesh.normals;
            var srcUVs = sourceMesh.uv;
            var subMeshCount = sourceMesh.subMeshCount;

            var vertexCount = srcVertices.Length * repetitions;

            var vertices = new Vector3[vertexCount];
            var normals = new Vector3[vertexCount];
            var uvs = new Vector2[vertexCount];

            var subMeshTriangles = new List<int>[subMeshCount];
            for (var s = 0; s < subMeshCount; s++)
            {
                subMeshTriangles[s] = new List<int>(sourceMesh.GetTriangles(s).Length * repetitions);
            }

            var vOffset = 0;

            var splinePos = spline.transform.position;
            var splineTransform = Matrix4x4.Translate(-splinePos);

            for (var i = 0; i < repetitions; i++)
            {
                // get beginning and end of the section within the spline
                var t0 = Mathf.Lerp(sectionBeginAt, sectionEndAt, (float)i / repetitions);
                var t1 = Mathf.Lerp(sectionBeginAt, sectionEndAt, (float)(i + 1) / repetitions);

                for (var v = 0; v < srcVertices.Length; v++)
                {
                    // map x position of the source mesh into a parameter between t0 and t1
                    var tx = Mathf.InverseLerp(meshMinX, meshMaxX, srcVertices[v].x);
                    var t = Mathf.Lerp(t0, t1, tx);
                    var pos = (Vector3)spline.EvaluatePosition(t);
                    var tangent = ((Vector3)spline.EvaluateTangent(t)).normalized;

                    var up = Vector3.up;
                    var binormal = Vector3.Cross(up, tangent).normalized;
                    var normal = Vector3.Cross(tangent, binormal).normalized;

                    var localToSpline = Matrix4x4.identity;
                    localToSpline.SetColumn(0, new Vector4(tangent.x, tangent.y, tangent.z, 0));
                    localToSpline.SetColumn(1, new Vector4(normal.x, normal.y, normal.z, 0));
                    localToSpline.SetColumn(2, new Vector4(binormal.x, binormal.y, binormal.z, 0));
                    localToSpline.SetColumn(3, new Vector4(pos.x, pos.y, pos.z, 1));
                    localToSpline = splineTransform * localToSpline;

                    var dstIndex = vOffset + v;
                    var localPos =
                        new Vector3(0, // mapped along the spline
                            srcVertices[v].y * meshScale,
                            srcVertices[v].z * meshScale);
                    vertices[dstIndex] = localToSpline.MultiplyPoint3x4(localPos);
                    normals[dstIndex] = localToSpline.MultiplyVector(srcNormals[v]).normalized;
                    uvs[dstIndex] = srcUVs[v];
                }

                // copy triangles for each submesh
                for (var s = 0; s < subMeshCount; s++)
                {
                    var srcTriangles = sourceMesh.GetTriangles(s);
                    for (var tIndex = 0; tIndex < srcTriangles.Length; tIndex += 3)
                    {
                        subMeshTriangles[s].Add(srcTriangles[tIndex + 0] + vOffset);
                        subMeshTriangles[s].Add(srcTriangles[tIndex + 2] + vOffset);
                        subMeshTriangles[s].Add(srcTriangles[tIndex + 1] + vOffset);
                    }
                }

                vOffset += srcVertices.Length;
            }

            _generatedMesh = new Mesh
            {
                name = "GeneratedSplineMesh",
                vertices = vertices,
                uv = uvs,
                normals = normals,
                subMeshCount = subMeshCount
            };

            for (var s = 0; s < subMeshCount; s++)
            {
                _generatedMesh.SetTriangles(subMeshTriangles[s], s);
            }

            _generatedMesh.RecalculateBounds(MeshUpdateFlags.Default);
            _generatedMesh.RecalculateNormals(MeshUpdateFlags.Default);

            filter.sharedMesh = _generatedMesh;
        }

#if UNITY_EDITOR
        [ContextMenu("Save Mesh As Asset")]
        private void SaveMesh()
        {
            if (!_generatedMesh)
            {
                Debug.LogWarning("No mesh generated to save.");
                return;
            }

            var path = EditorUtility.SaveFilePanelInProject(
                "Save Generated Mesh",
                _generatedMesh.name + ".asset",
                "asset",
                "Choose a location to save the generated mesh asset."
            );

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(Object.Instantiate(_generatedMesh), path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("Mesh saved to: " + path);
            }
        }
#endif
    }
}