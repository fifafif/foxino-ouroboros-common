using System.Collections.Generic;
using UnityEngine;

namespace Ouroboros.Common.Pooling
{
    public struct PoolObjectRequest
    {
        public GameObject Prefab;
        public string Id;
        public int Count;

        public static List<PoolObjectRequest> CreateRequests()
        {
            return new List<PoolObjectRequest>();
        }
    }

    public static class PoolObjectRequestExtensions
    {
        public static List<PoolObjectRequest> AddRequest(this List<PoolObjectRequest> requests, Component component, int count)
        {
            return requests.AddRequest(component, null, count);
        }

        public static List<PoolObjectRequest> AddRequest(this List<PoolObjectRequest> requests, Component component, string id, int count)
        {
            if (component == null)
            {
                return requests;
            }

            return requests.AddRequest(component.gameObject, id, count);
        }

        public static List<PoolObjectRequest> AddRequest(this List<PoolObjectRequest> requests, GameObject gameObject, string id, int count)
        {
            if (gameObject == null)
            {
                return requests;
            }

            requests.Add(new PoolObjectRequest
            {
                Prefab = gameObject,
                Id = id,
                Count = count
            });

            return requests;
        }
    }
}