// Audio/Core/AudioDatabase.cs

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.GameAudio
{
    [CreateAssetMenu(fileName = "AudioDatabase", menuName = "Game Audio/Audio Database")]
    public class AudioDatabase : ScriptableObject
    {
        [Header("Database Info")]
        public string databaseName = "Game Audio Database";
        public string version = "1.0";
        
        [Header("Audio Collections")]
        public List<AudioCollection> audioCollections = new List<AudioCollection>();
        
        [Header("Audio Mixer Groups")]
        public List<string> mixerGroupNames = new List<string> { "Master", "SFX", "Music", "Voice", "Ambience" };
        
        [Header("Global Settings")]
        public float globalVolume = 1f;
        public bool enableAudioLOD = true;
        public int maxConcurrentSounds = 32;
        
        // Runtime caching
        [NonSerialized]
        private Dictionary<string, AudioClipDefinition> audioClipCache;
        [NonSerialized]
        private Dictionary<AudioType, List<AudioClipDefinition>> typeCache;
        [NonSerialized]
        private Dictionary<AudioCategory, List<AudioClipDefinition>> categoryCache;
        [NonSerialized]
        private Dictionary<string, List<AudioClipDefinition>> tagCache;

        private void OnEnable()
        {
            BuildCache();
        }

        private void BuildCache()
        {
            audioClipCache = new Dictionary<string, AudioClipDefinition>();
            typeCache = new Dictionary<AudioType, List<AudioClipDefinition>>();
            categoryCache = new Dictionary<AudioCategory, List<AudioClipDefinition>>();
            tagCache = new Dictionary<string, List<AudioClipDefinition>>();

            foreach (var collection in audioCollections)
            {
                foreach (var clip in collection.audioClips)
                {
                    if (clip.IsValidClip())
                    {
                        // Cache by ID
                        audioClipCache[clip.id] = clip;
                        
                        // Cache by type
                        if (!typeCache.ContainsKey(clip.audioType))
                            typeCache[clip.audioType] = new List<AudioClipDefinition>();
                        typeCache[clip.audioType].Add(clip);
                        
                        // Cache by category
                        if (!categoryCache.ContainsKey(clip.category))
                            categoryCache[clip.category] = new List<AudioClipDefinition>();
                        categoryCache[clip.category].Add(clip);
                        
                        // Cache by tags
                        foreach (var tag in clip.tags)
                        {
                            if (!tagCache.ContainsKey(tag))
                                tagCache[tag] = new List<AudioClipDefinition>();
                            tagCache[tag].Add(clip);
                        }
                    }
                }
            }
        }

        public AudioClipDefinition GetClipById(string id)
        {
            if (audioClipCache == null) BuildCache();
            return audioClipCache.TryGetValue(id, out var clip) ? clip : null;
        }

        public AudioClipDefinition GetClipByName(string name)
        {
            if (audioClipCache == null) BuildCache();
            return audioClipCache.Values.FirstOrDefault(c => c.displayName == name);
        }

        public List<AudioClipDefinition> GetClipsByType(AudioType type)
        {
            if (typeCache == null) BuildCache();
            return typeCache.TryGetValue(type, out var clips) ? clips : new List<AudioClipDefinition>();
        }

        public List<AudioClipDefinition> GetClipsByCategory(AudioCategory category)
        {
            if (categoryCache == null) BuildCache();
            return categoryCache.TryGetValue(category, out var clips) ? clips : new List<AudioClipDefinition>();
        }

        public List<AudioClipDefinition> GetClipsByTag(string tag)
        {
            if (tagCache == null) BuildCache();
            return tagCache.TryGetValue(tag, out var clips) ? clips : new List<AudioClipDefinition>();
        }

        public AudioCollection GetCollection(string name)
        {
            return audioCollections.FirstOrDefault(c => c.collectionName == name);
        }

        public List<AudioClipDefinition> GetAllClips()
        {
            if (audioClipCache == null) BuildCache();
            return audioClipCache.Values.ToList();
        }

        public List<string> GetAllTags()
        {
            if (tagCache == null) BuildCache();
            return tagCache.Keys.ToList();
        }

        // Validation methods
        public List<string> ValidateDatabase()
        {
            var issues = new List<string>();
            
            foreach (var collection in audioCollections)
            {
                if (string.IsNullOrEmpty(collection.collectionName))
                {
                    issues.Add($"Collection has no name");
                }
                
                foreach (var clip in collection.audioClips)
                {
                    if (clip.clip == null)
                    {
                        issues.Add($"Clip '{clip.displayName}' in collection '{collection.collectionName}' has no AudioClip assigned");
                    }
                    
                    if (string.IsNullOrEmpty(clip.displayName))
                    {
                        issues.Add($"Clip in collection '{collection.collectionName}' has no display name");
                    }
                }
            }
            
            return issues;
        }

        #if UNITY_EDITOR
        [ContextMenu("Rebuild Cache")]
        public void RebuildCache()
        {
            BuildCache();
            Debug.Log($"Audio Database cache rebuilt. Found {audioClipCache.Count} clips.");
        }
        
        [ContextMenu("Validate Database")]
        public void ValidateDatabaseEditor()
        {
            var issues = ValidateDatabase();
            if (issues.Count == 0)
            {
                Debug.Log("Audio Database validation passed!");
            }
            else
            {
                Debug.LogWarning($"Audio Database validation found {issues.Count} issues:");
                foreach (var issue in issues)
                {
                    Debug.LogWarning($"- {issue}");
                }
            }
        }
        #endif
    }
}