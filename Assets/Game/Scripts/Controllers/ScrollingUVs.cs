using UnityEngine;
using System.Collections;

namespace Aponi
{
    public class ScrollingUVs : MonoBehaviour
    {
        public int materialIndex = 0;
        public Vector2 uvAnimationRate = new Vector2(1.0f, 0.0f);
        public string textureName = "_MainTex";

        Vector2 uvOffset = Vector2.zero;

        Renderer ren;

        Material mat;

        private void Start()
        {
            ren = GetComponent<Renderer>();
            mat = ren.materials[materialIndex];
        }

        void LateUpdate()
        {
            uvOffset += (uvAnimationRate * Time.deltaTime);
            if (ren.enabled)
            {
                mat.SetTextureOffset(textureName, uvOffset);
            }
        }
    }
}