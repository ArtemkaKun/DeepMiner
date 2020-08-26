using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Entities
{
    public class EntitiesManager : MonoBehaviour
    {
        private static Dictionary<string, Entity> _gameEntities;
        private EntityManager _entityManager;
        private BlobAssetStore _blobAssetStore;
        
        public static Material CurrentPlayerMaterial;
        public static Material SidePlayerMaterial;
        public static Material FrontPlayerMaterial;
        public static Mesh quad;
        
        public static Material ShopButtonMaterial;

        private void Awake()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _blobAssetStore = new BlobAssetStore();
            _gameEntities = new Dictionary<string, Entity>();
            
            SidePlayerMaterial = Resources.Load<Material>("Materials/PlayerMaterial");
            FrontPlayerMaterial = Resources.Load<Material>("Materials/PlayerMaterialFront");
            CurrentPlayerMaterial = SidePlayerMaterial;

            var simpleMesh = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad = simpleMesh.GetComponent<MeshFilter>().sharedMesh;
            simpleMesh.transform.position = new Vector3(10, 10, 10);
            
            ShopButtonMaterial = Resources.Load<Material>("Materials/ShopButton");
            
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
