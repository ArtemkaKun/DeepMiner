using System.Collections.Generic;
using Unity.Entities;

namespace Entities
{
    public struct GroundEntities
    {
        public Dictionary<string, Entity> GameEntities { get; }
        public BlobAssetStore BlobAssetStore { get; }

        public GroundEntities(Dictionary<string, Entity> gameEntities, BlobAssetStore blobAssetStore)
        {
            GameEntities = gameEntities;
            BlobAssetStore = blobAssetStore;
        }
    }
}