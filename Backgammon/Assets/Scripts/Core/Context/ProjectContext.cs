using System.Collections.Generic;
using Core.DI;
using UnityEngine;

namespace Core.Context
{
    public class ProjectContext : MonoBehaviour
    {
        [SerializeField] private List<MonoInstaller> installers = new();
        
        private static ProjectContext instance;
        private static DiContainer projectContainer;

        public static DiContainer Container
        {
            get
            {
                if (projectContainer == null)
                {
                    EnsureInstance();
                }
                return projectContainer;
            }
        }

        private static void EnsureInstance()
        {
            if (instance == null)
            {
                GameObject contextGO = new GameObject("[ProjectContext]");
                DontDestroyOnLoad(contextGO);
                instance = contextGO.AddComponent<ProjectContext>();
                instance.Initialize();
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            projectContainer = new DiContainer();
            
            // Install default bindings
            InstallDefaultBindings();
            
            // Install custom bindings
            foreach (MonoInstaller installer in installers)
            {
                installer.InstallBindings(projectContainer);
            }

            Debug.Log("ProjectContext initialized with DI Container");
        }

        private void InstallDefaultBindings()
        {
            // Bind the container itself
            projectContainer.BindInstance<DiContainer>(projectContainer);
        }

        private void OnDestroy()
        {
            projectContainer?.Dispose();
        }
    }
}