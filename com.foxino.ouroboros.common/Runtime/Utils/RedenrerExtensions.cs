using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public static class RedenrerExtensions
    {
        public static Material GetMaterialSafe(this Renderer renderer)
        {
            if (Application.isPlaying)
            {
                return renderer.material;
            }

            return renderer.sharedMaterial;
        }

        public static Material[] GetMaterialsSafe(this Renderer renderer)
        {
            if (Application.isPlaying)
            {
                return renderer.materials;
            }

            return renderer.sharedMaterials;
        }
    }
}