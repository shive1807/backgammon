using UnityEngine;
using Core.DI;

namespace Core.GameAudio
{
    /// <summary>
    /// Specialized context installer for audio services.
    /// Registers audio-related services with the DI container.
    /// </summary>
    public class AudioContextInstaller : MonoInstaller
    {
        [Header("Audio Services")]
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private AudioDatabase audioDatabase;
        
        [Header("Audio Settings")]
        [SerializeField] private bool enableAudioDebug = false;
        [SerializeField] private float defaultVolume = 1f;
        [SerializeField] private float defaultFadeTime = 1f;
        
        public override void InstallBindings(DiContainer container)
        {
            Debug.Log("[AudioContextInstaller] Installing audio bindings...");
            
            // Auto-find audio services if not assigned
            if (audioManager == null)
                audioManager = FindObjectOfType<AudioManager>();
            
            if (audioDatabase == null)
                audioDatabase = FindObjectOfType<AudioDatabase>();
            
            // Install audio services
            InstallAudioServices(container);
            
            // Install audio settings
            InstallAudioSettings(container);
            
            Debug.Log("[AudioContextInstaller] Audio bindings installed successfully!");
        }
        
        private void InstallAudioServices(DiContainer container)
        {
            // Audio Manager - Core audio system
            if (audioManager != null)
            {
                container.Bind<AudioManager>().FromInstance(audioManager);
                Debug.Log("[AudioContextInstaller] Registered AudioManager");
            }
            else
            {
                Debug.LogWarning("[AudioContextInstaller] AudioManager not found!");
            }
            
            // Audio Database - Audio clip definitions
            if (audioDatabase != null)
            {
                container.Bind<AudioDatabase>().FromInstance(audioDatabase);
                Debug.Log("[AudioContextInstaller] Registered AudioDatabase");
            }
            else
            {
                Debug.LogWarning("[AudioContextInstaller] AudioDatabase not found!");
            }
        }
        
        private void InstallAudioSettings(DiContainer container)
        {
            // Audio Settings - Configuration values
            var audioSettings = new AudioSettings
            {
                EnableDebug = enableAudioDebug,
                DefaultVolume = defaultVolume,
                DefaultFadeTime = defaultFadeTime
            };
            
            container.Bind<AudioSettings>().FromInstance(audioSettings);
            Debug.Log("[AudioContextInstaller] Registered AudioSettings");
        }
    }
    
    /// <summary>
    /// Audio configuration settings for dependency injection
    /// </summary>
    [System.Serializable]
    public class AudioSettings
    {
        public bool EnableDebug { get; set; }
        public float DefaultVolume { get; set; }
        public float DefaultFadeTime { get; set; }
    }
} 