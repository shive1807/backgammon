using System.Collections.Generic;
using Core.DI;
using UnityEngine;

namespace Core.Context
{
    public class SceneContext : MonoBehaviour
    {
        [SerializeField] private List<MonoInstaller> installers = new();
        
        private DIContainer sceneContainer;

        public DIContainer Container => sceneContainer;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            // Create scene container with project container as parent
            sceneContainer = new DIContainer();
            
            // Copy project bindings to scene container
            // (In a full implementation, you'd want parent-child container relationship)
            
            foreach (MonoInstaller installer in installers)
            {
                installer.InstallBindings(sceneContainer);
            }

            Debug.Log("SceneContext initialized");
        }

        private void OnDestroy()
        {
            sceneContainer?.Dispose();
        }
    }
}