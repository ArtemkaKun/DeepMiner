using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Entities
{
    public class EntitiesManager : MonoBehaviour
    {
        private EntityManager _entityManager;
        private static Dictionary<string, Entity> _gameEntities;
        private BlobAssetStore _blobAssetStore;
        
        public static Material CurrentPlayerMaterial;
        public static Material SidePlayerMaterial;
        public static Material FrontPlayerMaterial;
        
        private void Awake()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _blobAssetStore = new BlobAssetStore();
            _gameEntities = new Dictionary<string, Entity>();
            
            SidePlayerMaterial = Resources.Load<Material>("Materials/PlayerMaterial");
            FrontPlayerMaterial = Resources.Load<Material>("Materials/PlayerMaterialFront");
            CurrentPlayerMaterial = SidePlayerMaterial;
            
            CreateGroundCellEntity();
        }

        private void CreateGroundCellEntity()
        {
            var groundCellsPrefabs = Resources.LoadAll<GameObject>("Prefabs/GroundCells/");
            foreach (var cellPrefab in groundCellsPrefabs)
            {
                var groundCellEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(cellPrefab,
                    GameObjectConversionSettings.FromWorld(_entityManager.World, _blobAssetStore));
            
                _gameEntities[cellPrefab.name] = groundCellEntity;
            }
        }

        public static Entity GetEntity(string entityName)
        {
            return _gameEntities[entityName];
        }

        private void OnDestroy()
        {
            _blobAssetStore.Dispose();
        }
    }
}
