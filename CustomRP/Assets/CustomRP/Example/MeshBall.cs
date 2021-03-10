using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.CustomRP.Example
{
    public class MeshBall: MonoBehaviour
    {
        static int baseColorId = Shader.PropertyToID("_BaseColor");

        [SerializeField]
        Mesh mesh = default;

        [SerializeField]
        Material material = default;

        [SerializeField]
        bool useGpuInstancing = true;

        Matrix4x4[] matrices = new Matrix4x4[1023];
        Vector4[] baseColors = new Vector4[1023];

        MaterialPropertyBlock block;

        void Awake()
        {
            for (int i = 0; i < matrices.Length; i++)
            {
                matrices[i] = Matrix4x4.TRS(
                    Random.insideUnitSphere * 10f, Quaternion.identity, Vector3.one
                );
                baseColors[i] =
                    new Vector4(Random.value, Random.value, Random.value, 1f);
            }

            block = new MaterialPropertyBlock();
            if (useGpuInstancing)
            {
                block.SetVectorArray(baseColorId , baseColors);
            }
        }

        void Update()
        {
            if (useGpuInstancing)
            {
                Graphics.DrawMeshInstanced(mesh, 0, material, matrices, 1023, block);
            }
            else
            {
                for (int i = 0; i < matrices.Length; i++)
                {
                    block.SetVector(baseColorId , baseColors[i]);
                    Graphics.DrawMesh(mesh, matrices[i], material, LayerMask.NameToLayer("Default") , Camera.main , 0 , block);
                }

            }
        }
    }

}
