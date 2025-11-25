using System;
using System.Collections.Generic;
using Ouroboros.Common.Logging;
using UnityEngine;

namespace Ouroboros.Common.Services
{
    public class ServiceLocator
    {
        private static Dictionary<Type, object> serviceMap = new Dictionary<Type, object>();

        // TODO: This needs some unregistration.    
        private static Dictionary<Type, Action> serviceRegisterdActionMap = new Dictionary<Type, Action>();

        public static T Register<T>(T service) where T : class
        {
            serviceMap[typeof(T)] = service;

            if (serviceRegisterdActionMap.TryGetValue(typeof(T), out var action))
            {
                action?.Invoke();
            }

            Logs.Debug<ServiceLocator>($"Register service of type={typeof(T)}.");

            return service;
        }

        public static void Unregister<T>(T service) where T : class
        {
            serviceMap.Remove(typeof(T));

            Logs.Debug<ServiceLocator>($"Unregister service of type={typeof(T)}.");
        }

        public static T Get<T>() where T : class
        {
            if (!serviceMap.TryGetValue(typeof(T), out var service))
            {
                Debug.LogError($"[ServiceLocator] Error getting service of type={typeof(T)}. Service not registered!");
            }

            return service as T;
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            if (!serviceMap.TryGetValue(typeof(T), out var genericService))
            {
                Logs.Warning<ServiceLocator>($"Warning while getting service of type={typeof(T)}. Service not registered!");

                service = default;
                return false;
            }

            service = genericService as T;

            return true;
        }

        public static void WaitForService<T>(Action onServiceRegistered) where T : class
        {
            if (serviceRegisterdActionMap.TryGetValue(typeof(T), out var action))
            {
                serviceRegisterdActionMap[typeof(T)] += onServiceRegistered;
            }
            else
            {
                serviceRegisterdActionMap.Add(typeof(T), onServiceRegistered);
            }

            if (TryGet<T>(out T service)
                && service != null)
            {
                onServiceRegistered?.Invoke();
            }
        }
    }
}