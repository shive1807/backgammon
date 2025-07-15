using UnityEngine;
using Core.DI;
using Core.GameAudio;

namespace Core.DI
{
    /// <summary>
    /// Example installer demonstrating non-lazy (eager) initialization.
    /// Non-lazy services are created immediately when the container is set up.
    /// </summary>
    public class NonLazyExampleInstaller : MonoInstaller
    {
        [Header("Services")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private AudioManager audioManager;
        
        public override void InstallBindings(DiContainer container)
        {
            Debug.Log("[NonLazyExampleInstaller] Installing bindings with non-lazy examples...");
            
            // Example 1: Lazy binding (default behavior)
            // Service is created when first requested
            if (gameManager != null)
            {
                container.Bind<GameManager>()
                    .FromInstance(gameManager)
                    .AsSingle(); // Lazy by default
            }
            
            // Example 2: Non-lazy binding
            // Service is created immediately during container setup
            if (audioManager != null)
            {
                container.Bind<AudioManager>()
                    .FromInstance(audioManager)
                    .AsSingle()
                    .NonLazy(); // Will be created immediately
            }
            
            // Example 3: Factory with non-lazy
            // Factory result is created immediately
            container.BindFactory<MyEagerService>(c => new MyEagerService())
                .NonLazy();
            
            // Example 4: Multiple chained options
            container.Bind<IMyInterface>()
                .To<MyImplementation>()
                .AsSingle()
                .NonLazy(); // Eager singleton
                
            Debug.Log("[NonLazyExampleInstaller] Non-lazy bindings configured.");
        }
    }
    
    /// <summary>
    /// Example interface for demonstration
    /// </summary>
    public interface IMyInterface
    {
        void DoSomething();
    }
    
    /// <summary>
    /// Example implementation that demonstrates eager initialization
    /// </summary>
    public class MyImplementation : IMyInterface
    {
        public MyImplementation()
        {
            Debug.Log("[MyImplementation] Constructor called - eagerly initialized!");
        }
        
        public void DoSomething()
        {
            Debug.Log("[MyImplementation] DoSomething called");
        }
    }
    
    /// <summary>
    /// Example service that demonstrates factory with eager initialization
    /// </summary>
    public class MyEagerService
    {
        public MyEagerService()
        {
            Debug.Log("[MyEagerService] Constructor called - created via factory and eagerly initialized!");
            Initialize();
        }
        
        private void Initialize()
        {
            Debug.Log("[MyEagerService] Performing eager initialization logic...");
            // Perform expensive setup operations here
            // This happens immediately when the container is created
        }
    }
} 