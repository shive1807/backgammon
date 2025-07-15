using UnityEngine;
using MPLCore.DI;

namespace MPLCore.DI
{
    /// <summary>
    /// Example installer demonstrating automatic GameObject creation with components.
    /// This shows how to create MonoBehaviour services without manually placing them in the scene.
    /// </summary>
    public class GameObjectCreationInstaller : MonoInstaller
    {
        [Header("Parent Transform (Optional)")]
        [SerializeField] private Transform uiParent;
        
        public override void InstallBindings(DiContainer container)
        {
            Debug.Log("[GameObjectCreationInstaller] Installing bindings with GameObject creation...");
            
            // Example 1: Create GameObject with class name "UiRoot"
            // This will create a GameObject named "UiRoot" with UiRoot component
            // container.Bind<UiRoot>()
            //     .FromNewGameObject()
            //     .AsSingle()
            //     .NonLazy(); // Created immediately
            
            // Example 2: Create GameObject with custom name
            container.Bind<AudioController>()
                .FromNewGameObject("GameAudioController")
                .AsSingle()
                .NonLazy();
            
            // Example 3: Create GameObject as child of existing parent
            if (uiParent != null)
            {
                container.Bind<HudManager>()
                    .FromNewGameObject(uiParent)
                    .AsSingle();
            }
            
            // Example 4: Multiple services with automatic GameObject creation
            container.Bind<SceneTransitionManager>()
                .FromNewGameObject()
                .AsSingle()
                .NonLazy();
                
            container.Bind<NotificationManager>()
                .FromNewGameObject("NotificationSystem")
                .AsSingle();
            
            Debug.Log("[GameObjectCreationInstaller] GameObject creation bindings installed!");
        }
    }
    
    /// <summary>
    /// Example UI Root MonoBehaviour that gets automatically created
    /// </summary>
    
    /// <summary>
    /// Example Audio Controller that gets automatically created
    /// </summary>
    public class AudioController : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log($"[AudioController] Awake called - GameObject: {gameObject.name}");
        }
        
        public void PlaySound(string soundName)
        {
            Debug.Log($"[AudioController] Playing sound: {soundName}");
        }
    }
    
    /// <summary>
    /// Example HUD Manager that gets created as child of UI parent
    /// </summary>
    public class HudManager : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log($"[HudManager] Awake called - GameObject: {gameObject.name}, Parent: {transform.parent?.name}");
        }
        
        public void UpdateHealth(int health)
        {
            Debug.Log($"[HudManager] Health updated: {health}");
        }
    }
    
    /// <summary>
    /// Example Scene Transition Manager
    /// </summary>
    public class SceneTransitionManager : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log($"[SceneTransitionManager] Awake called - GameObject: {gameObject.name}");
        }
        
        public void TransitionToScene(string sceneName)
        {
            Debug.Log($"[SceneTransitionManager] Transitioning to: {sceneName}");
        }
    }
    
    /// <summary>
    /// Example Notification Manager
    /// </summary>
    public class NotificationManager : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log($"[NotificationManager] Awake called - GameObject: {gameObject.name}");
        }
        
        public void ShowNotification(string message)
        {
            Debug.Log($"[NotificationManager] Notification: {message}");
        }
    }
    
    /// <summary>
    /// Example of using the automatically created services
    /// </summary>
    public class ServiceUsageExample : MonoBehaviour
    {
        [Inject] private AudioController audioController;
        [Inject] private HudManager hudManager;
        [Inject] private SceneTransitionManager sceneTransition;
        [Inject] private NotificationManager notificationManager;
        
        private void Start()
        {
            // Inject dependencies
            MonoInjectHelper.InjectIntoObject(this);
            
            // Use the automatically created services
            TestServices();
        }
        
        private void TestServices()
        {
            // Test all injected services
            audioController?.PlaySound("ButtonClick");
            hudManager?.UpdateHealth(100);
            sceneTransition?.TransitionToScene("GameScene");
            notificationManager?.ShowNotification("Services ready!");
        }
    }
} 