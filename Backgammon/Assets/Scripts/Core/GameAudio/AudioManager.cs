// Audio/Core/AudioManager.cs

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.GameAudio
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Database")]
        public AudioDatabase audioDatabase;
        
        [Header("Audio Mixer")]
        public AudioMixer audioMixer;
        
        [Header("Audio Sources")]
        public int audioSourcePoolSize = 20;
        public GameObject audioSourcePrefab;
        
        [Header("Settings")]
        public bool enableDebugLogs = false;
        public float defaultFadeTime = 1f;
        
        // Singleton
        public static AudioManager Instance { get; private set; }
        
        // Audio source pools
        private Queue<AudioSource> availableAudioSources = new Queue<AudioSource>();
        private List<AudioSource> activeAudioSources = new List<AudioSource>();
        
        // Currently playing audio
        private Dictionary<string, AudioSource> currentlyPlayingById = new Dictionary<string, AudioSource>();
        private Dictionary<string, List<AudioSource>> currentlyPlayingByCollection = new Dictionary<string, List<AudioSource>>();
        
        // Audio mixer groups
        private Dictionary<string, AudioMixerGroup> mixerGroups = new Dictionary<string, AudioMixerGroup>();
        
        // Events
        public event Action<AudioClipDefinition> OnAudioStarted;
        public event Action<AudioClipDefinition> OnAudioFinished;
        public event Action<string> OnCollectionStarted;
        public event Action<string> OnCollectionFinished;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            // Initialize audio source pool
            InitializeAudioSourcePool();
            
            // Initialize mixer groups
            InitializeMixerGroups();
            
            if (enableDebugLogs)
            {
                Debug.Log($"AudioManager initialized with {audioSourcePoolSize} audio sources");
            }
        }

        private void InitializeAudioSourcePool()
        {
            for (int i = 0; i < audioSourcePoolSize; i++)
            {
                var audioSourceGO = audioSourcePrefab ? 
                    Instantiate(audioSourcePrefab, transform) : 
                    new GameObject($"AudioSource_{i}");
                
                audioSourceGO.transform.SetParent(transform);
                
                var audioSource = audioSourceGO.GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = audioSourceGO.AddComponent<AudioSource>();
                }
                
                audioSource.playOnAwake = false;
                audioSource.loop = false;
                
                availableAudioSources.Enqueue(audioSource);
            }
        }

        private void InitializeMixerGroups()
        {
            if (audioMixer == null) return;
            
            foreach (var groupName in audioDatabase.mixerGroupNames)
            {
                var groups = audioMixer.FindMatchingGroups(groupName);
                if (groups.Length > 0)
                {
                    mixerGroups[groupName] = groups[0];
                }
            }
        }

        // Main play methods
        public AudioSource PlayClip(string clipId, Vector3? position = null, Transform parent = null)
        {
            var clipDef = audioDatabase.GetClipById(clipId);
            if (clipDef == null)
            {
                Debug.LogWarning($"Audio clip with ID '{clipId}' not found");
                return null;
            }
            
            return PlayClip(clipDef, position, parent);
        }

        public AudioSource PlayClip(AudioClipDefinition clipDef, Vector3? position = null, Transform parent = null)
        {
            if (clipDef == null || clipDef.clip == null) return null;
            
            // Check cooldown
            if (!clipDef.CanPlay())
            {
                if (enableDebugLogs)
                    Debug.Log($"Audio clip '{clipDef.displayName}' is on cooldown");
                return null;
            }
            
            // Check overlap prevention
            if (clipDef.preventOverlap && clipDef.isPlaying)
            {
                if (enableDebugLogs)
                    Debug.Log($"Audio clip '{clipDef.displayName}' is already playing (overlap prevented)");
                return null;
            }
            
            var audioSource = GetAudioSource();
            if (audioSource == null)
            {
                Debug.LogWarning("No available audio sources");
                return null;
            }
            
            // Configure audio source
            ConfigureAudioSource(audioSource, clipDef, position, parent);
            
            // Play the clip
            audioSource.Play();
            
            // Update runtime data
            clipDef.lastPlayedTime = Time.time;
            clipDef.isPlaying = true;
            clipDef.activeAudioSources.Add(audioSource);
            
            // Track playing audio
            currentlyPlayingById[clipDef.id] = audioSource;
            
            // Start coroutine to handle cleanup
            StartCoroutine(HandleAudioClipLifetime(audioSource, clipDef));
            
            // Fire event
            OnAudioStarted?.Invoke(clipDef);
            
            if (enableDebugLogs)
                Debug.Log($"Playing audio clip: {clipDef.displayName}");
            
            return audioSource;
        }

        public void PlayClipByName(string clipName, Vector3? position = null, Transform parent = null)
        {
            var clipDef = audioDatabase.GetClipByName(clipName);
            PlayClip(clipDef, position, parent);
        }

        public void PlayRandomFromCategory(AudioCategory category, Vector3? position = null, Transform parent = null)
        {
            var clips = audioDatabase.GetClipsByCategory(category);
            if (clips.Count > 0)
            {
                var randomClip = clips[UnityEngine.Random.Range(0, clips.Count)];
                PlayClip(randomClip, position, parent);
            }
        }

        public void PlayRandomFromTag(string tag, Vector3? position = null, Transform parent = null)
        {
            var clips = audioDatabase.GetClipsByTag(tag);
            if (clips.Count > 0)
            {
                var randomClip = clips[UnityEngine.Random.Range(0, clips.Count)];
                PlayClip(randomClip, position, parent);
            }
        }

        public void PlayCollection(string collectionName, Vector3? position = null, Transform parent = null)
        {
            var collection = audioDatabase.GetCollection(collectionName);
            if (collection == null)
            {
                Debug.LogWarning($"Audio collection '{collectionName}' not found");
                return;
            }
            
            StartCoroutine(PlayCollectionCoroutine(collection, position, parent));
        }

        private IEnumerator PlayCollectionCoroutine(AudioCollection collection, Vector3? position, Transform parent)
        {
            OnCollectionStarted?.Invoke(collection.collectionName);
            
            var clipsToPlay = new List<AudioClipDefinition>();
            
            // Get all clips based on playback mode
            switch (collection.playbackMode)
            {
                case AudioCollection.PlaybackMode.Random:
                    clipsToPlay.Add(collection.GetNextClip());
                    break;
                    
                case AudioCollection.PlaybackMode.Sequential:
                case AudioCollection.PlaybackMode.RandomNoRepeat:
                    // Play all clips in the collection
                    for (int i = 0; i < collection.audioClips.Count; i++)
                    {
                        clipsToPlay.Add(collection.GetNextClip());
                    }
                    break;
            }
            
            foreach (var clip in clipsToPlay)
            {
                if (clip != null)
                {
                    var audioSource = PlayClip(clip, position, parent);
                    
                    if (collection.waitForClipToFinish && audioSource != null)
                    {
                        yield return new WaitWhile(() => audioSource.isPlaying);
                    }
                    
                    if (collection.timeBetweenClips > 0)
                    {
                        yield return new WaitForSeconds(collection.timeBetweenClips);
                    }
                }
            }
            
            OnCollectionFinished?.Invoke(collection.collectionName);
        }

        // Stop methods
        public void StopClip(string clipId)
        {
            if (currentlyPlayingById.TryGetValue(clipId, out var audioSource))
            {
                audioSource.Stop();
                ReturnAudioSource(audioSource);
                currentlyPlayingById.Remove(clipId);
            }
        }

        public void StopAllClips()
        {
            foreach (var audioSource in activeAudioSources.ToList())
            {
                audioSource.Stop();
                ReturnAudioSource(audioSource);
            }
            
            currentlyPlayingById.Clear();
            currentlyPlayingByCollection.Clear();
        }

        public void StopClipsByCategory(AudioCategory category)
        {
            var clipsToStop = currentlyPlayingById.Where(kvp => 
                audioDatabase.GetClipById(kvp.Key)?.category == category).ToList();
            
            foreach (var kvp in clipsToStop)
            {
                kvp.Value.Stop();
                ReturnAudioSource(kvp.Value);
                currentlyPlayingById.Remove(kvp.Key);
            }
        }

        public void StopClipsByTag(string tag)
        {
            var clipsToStop = currentlyPlayingById.Where(kvp => 
                audioDatabase.GetClipById(kvp.Key)?.HasTag(tag) == true).ToList();
            
            foreach (var kvp in clipsToStop)
            {
                kvp.Value.Stop();
                ReturnAudioSource(kvp.Value);
                currentlyPlayingById.Remove(kvp.Key);
            }
        }

        // Fade methods
        public void FadeInClip(string clipId, float fadeTime = -1f, Vector3? position = null, Transform parent = null)
        {
            var clipDef = audioDatabase.GetClipById(clipId);
            if (clipDef != null)
            {
                var audioSource = PlayClip(clipDef, position, parent);
                if (audioSource != null)
                {
                    StartCoroutine(FadeInCoroutine(audioSource, fadeTime > 0 ? fadeTime : defaultFadeTime, clipDef.GetRandomizedVolume()));
                }
            }
        }

        public void FadeOutClip(string clipId, float fadeTime = -1f)
        {
            if (currentlyPlayingById.TryGetValue(clipId, out var audioSource))
            {
                StartCoroutine(FadeOutCoroutine(audioSource, fadeTime > 0 ? fadeTime : defaultFadeTime));
            }
        }

        private IEnumerator FadeInCoroutine(AudioSource audioSource, float fadeTime, float targetVolume)
        {
            audioSource.volume = 0f;
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(0f, targetVolume, elapsedTime / fadeTime);
                yield return null;
            }
            
            audioSource.volume = targetVolume;
        }

        private IEnumerator FadeOutCoroutine(AudioSource audioSource, float fadeTime)
        {
            float startVolume = audioSource.volume;
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeTime);
                yield return null;
            }
            
            audioSource.volume = 0f;
            audioSource.Stop();
        }

        // Helper methods
        private void ConfigureAudioSource(AudioSource audioSource, AudioClipDefinition clipDef, Vector3? position, Transform parent)
        {
            audioSource.clip = clipDef.clip;
            audioSource.volume = clipDef.GetRandomizedVolume();
            audioSource.pitch = clipDef.GetRandomizedPitch();
            audioSource.loop = clipDef.loop;
            audioSource.spatialBlend = clipDef.spatialBlend;
            audioSource.priority = clipDef.priority;
            audioSource.minDistance = clipDef.minDistance;
            audioSource.maxDistance = clipDef.maxDistance;
            audioSource.rolloffMode = clipDef.rolloffMode;
            audioSource.bypassEffects = clipDef.bypassEffects;
            audioSource.bypassListenerEffects = clipDef.bypassListenerEffects;
            audioSource.bypassReverbZones = clipDef.bypassReverbZones;
            
            // Set mixer group
            if (clipDef.outputAudioMixerGroup < audioDatabase.mixerGroupNames.Count)
            {
                var groupName = audioDatabase.mixerGroupNames[clipDef.outputAudioMixerGroup];
                if (mixerGroups.TryGetValue(groupName, out var mixerGroup))
                {
                    audioSource.outputAudioMixerGroup = mixerGroup;
                }
            }
            
            // Set position
            if (position.HasValue)
            {
                audioSource.transform.position = position.Value;
            }
            
            // Set parent
            if (parent != null)
            {
                audioSource.transform.SetParent(parent);
            }
            else
            {
                audioSource.transform.SetParent(transform);
            }
        }

        private AudioSource GetAudioSource()
        {
            if (availableAudioSources.Count > 0)
            {
                var audioSource = availableAudioSources.Dequeue();
                activeAudioSources.Add(audioSource);
                return audioSource;
            }
            
            return null;
        }

        private void ReturnAudioSource(AudioSource audioSource)
        {
            if (audioSource == null) return;
            
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.transform.SetParent(transform);
            audioSource.transform.localPosition = Vector3.zero;
            
            activeAudioSources.Remove(audioSource);
            availableAudioSources.Enqueue(audioSource);
        }

        private IEnumerator HandleAudioClipLifetime(AudioSource audioSource, AudioClipDefinition clipDef)
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            
            // Cleanup
            clipDef.isPlaying = false;
            clipDef.activeAudioSources.Remove(audioSource);
            
            currentlyPlayingById.Remove(clipDef.id);
            ReturnAudioSource(audioSource);
            
            // Fire event
            OnAudioFinished?.Invoke(clipDef);
        }

        // Query methods
        public bool IsClipPlaying(string clipId)
        {
            return currentlyPlayingById.ContainsKey(clipId);
        }

        public List<AudioClipDefinition> GetPlayingClips()
        {
            return currentlyPlayingById.Keys.Select(id => audioDatabase.GetClipById(id)).ToList();
        }

        public int GetActiveAudioSourceCount()
        {
            return activeAudioSources.Count;
        }

        public int GetAvailableAudioSourceCount()
        {
            return availableAudioSources.Count;
        }
    }
}