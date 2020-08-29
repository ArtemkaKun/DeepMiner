using UnityEngine;

namespace Entities
{
    public struct ShopResources
    {
        public Material ShopButtonMaterial { get; set; }

        public ShopResources(Material shopButtonMaterial)
        {
            ShopButtonMaterial = shopButtonMaterial;
        }
    }
}