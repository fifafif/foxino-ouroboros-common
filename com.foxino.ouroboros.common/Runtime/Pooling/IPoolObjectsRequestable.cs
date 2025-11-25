using System.Collections.Generic;

namespace Ouroboros.Common.Pooling
{
    public interface IPoolObjectsRequestable
    {
        List<PoolObjectRequest> GetPoolableObjects();
    }
}
