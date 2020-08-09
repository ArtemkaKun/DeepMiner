using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PlayerSpritesComponent : IComponentData
{
    public Material SideSprite;
    public Material FrontSprite;
}
