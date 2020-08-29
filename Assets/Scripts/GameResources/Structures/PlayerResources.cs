using UnityEngine;

namespace Entities
{
    public struct PlayerResources
    {
        public Material CurrentPlayerMaterial { get; set; }
        public Material SidePlayerMaterial { get; }
        public Material FrontPlayerMaterial { get; }
        public Mesh QuadMesh { get; }

        public PlayerResources(Material sidePlayerMaterial, Material frontPlayerMaterial, Mesh quadMesh)
        {
            SidePlayerMaterial = sidePlayerMaterial;
            FrontPlayerMaterial = frontPlayerMaterial;
            CurrentPlayerMaterial = sidePlayerMaterial;
            
            QuadMesh = quadMesh;
        }
    }
}