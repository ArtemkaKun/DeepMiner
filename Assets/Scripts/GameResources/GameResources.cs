using System.Collections.Generic;
using Entities.Enums;
using Unity.Entities;
using UnityEngine;

namespace Entities
{
    public class GameResources : MonoBehaviour
    {
        private static GroundEntities _groundEntities;
        private static PlayerResources _playerResources;
        private static ShopResources _shopResources;

        public static void SwitchCurrentPlayerMaterial(PlayerViewPosition viewPosition)
        {
            switch (viewPosition)
            {
                case PlayerViewPosition.Front:
                    _playerResources.CurrentPlayerMaterial = _playerResources.FrontPlayerMaterial;
                    break;
                case PlayerViewPosition.Side:
                    _playerResources.CurrentPlayerMaterial = _playerResources.SidePlayerMaterial;
                    break;
            }
        }

        public static Mesh GetQuadMesh()
        {
            return _playerResources.QuadMesh;
        }

        public static Material GetCurrentPlayerMaterial()
        {
            return _playerResources.CurrentPlayerMaterial;
        }
        
        public static Material GetShopButtonMaterial()
        {
            return _shopResources.ShopButtonMaterial;
        }
        
        private void Awake()
        {
            InitializePlayerResources();
            InitializeShopResources();

            _groundEntities = new GroundEntities(new Dictionary<string, Entity>(), new BlobAssetStore());
            ConvertGroundCellsToEntity();
        }

        private void InitializePlayerResources()
        {
            var simpleMesh = CreateQuadGameObject();

            _playerResources = new PlayerResources(
                Resources.Load<Material>("Materials/PlayerMaterial"),
                Resources.Load<Material>("Materials/PlayerMaterialFront"),
                simpleMesh.GetComponent<MeshFilter>().sharedMesh
            );
            
            DestroyImmediate(simpleMesh);
        }

        private GameObject CreateQuadGameObject()
        {
            return GameObject.CreatePrimitive(PrimitiveType.Quad);
        }

        private void InitializeShopResources()
        {
            _shopResources = new ShopResources(Resources.Load<Material>("Materials/ShopButton"));
        }

        private void ConvertGroundCellsToEntity()
        {
            var groundCellsPrefabs = Resources.LoadAll<GameObject>("Prefabs/GroundCells/");
            
            foreach (var cellPrefab in groundCellsPrefabs)
            {
                var groundCellEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(cellPrefab,
                    GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _groundEntities.BlobAssetStore));
            
                _groundEntities.GameEntities[cellPrefab.name] = groundCellEntity;
            }
        }

        public static Entity GetGroundCell(string entityName)
        {
            return _groundEntities.GameEntities[entityName];
        }

        private void OnDestroy()
        {
            _groundEntities.BlobAssetStore.Dispose();
        }
    }
}
