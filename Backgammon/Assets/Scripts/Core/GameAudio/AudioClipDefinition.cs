// Audio/Core/AudioClipDefinition.cs

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.GameAudio
{
    public enum AudioType
    {
        SFX,
        Music,
        Voice,
        Ambience,
        UI
    }

    public enum AudioCategory
    {
        // SFX Categories
        Gameplay,
        UI,
        Combat,
        Environment,
        Notification,
        
        // Music Categories
        Menu,
        GameplayMusic,
        Victory,
        Defeat,
        
        // Voice Categories
        Narrator,
        Character,
        Announcer
    }

    [Serializable]
    public class AudioClipDefinition
    {
        [Header("Basic Info")]
        public string id;
        public string displayName;
        public AudioClip clip;
        
        [Header("Audio Settings")]
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0f, 3f)]
        public float pitch = 1f;
        [Range(0f, 1f)]
        public float spatialBlend = 0f; // 0 = 2D, 1 = 3D
        
        [Header("Classification")]
        public AudioType audioType = AudioType.SFX;
        public AudioCategory category = AudioCategory.Gameplay;
        public List<string> tags = new List<string>();
        
        [Header("Playback Settings")]
        public bool loop = false;
        public float fadeInTime = 0f;
        public float fadeOutTime = 0f;
        public int priority = 128; // AudioSource priority (0-256)
        
        [Header("Randomization")]
        public bool randomizeVolume = false;
        [Range(0f, 1f)]
        public float volumeVariation = 0.1f;
        
        public bool randomizePitch = false;
        [Range(0f, 1f)]
        public float pitchVariation = 0.1f;
        
        [Header("Cooldown")]
        public float cooldownTime = 0f;
        public bool preventOverlap = false;
        
        [Header("Conditions")]
        public float minDistance = 1f;
        public float maxDistance = 500f;
        public AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;
        
        [Header("Advanced")]
        public bool bypassEffects = false;
        public bool bypassListenerEffects = false;
        public bool bypassReverbZones = false;
        public int outputAudioMixerGroup = 0; // Index reference
        
        // Runtime data
        [NonSerialized]
        public float lastPlayedTime = -1f;
        [NonSerialized]
        public bool isPlaying = false;
        [NonSerialized]
        public List<AudioSource> activeAudioSources = new List<AudioSource>();

        public AudioClipDefinition()
        {
            id = System.Guid.NewGuid().ToString();
        }

        public AudioClipDefinition(string displayName, AudioClip clip)
        {
            this.id = System.Guid.NewGuid().ToString();
            this.displayName = displayName;
            this.clip = clip;
        }

        public float GetRandomizedVolume()
        {
            if (!randomizeVolume) return volume;
            
            float variation = UnityEngine.Random.Range(-volumeVariation, volumeVariation);
            return Mathf.Clamp01(volume + variation);
        }

        public float GetRandomizedPitch()
        {
            if (!randomizePitch) return pitch;
            
            float variation = UnityEngine.Random.Range(-pitchVariation, pitchVariation);
            return Mathf.Clamp(pitch + variation, 0.1f, 3f);
        }

        public bool CanPlay()
        {
            if (cooldownTime <= 0f) return true;
            if (lastPlayedTime < 0f) return true;
            
            return Time.time - lastPlayedTime >= cooldownTime;
        }

        public bool HasTag(string tag)
        {
            return tags.Contains(tag);
        }

        public bool IsValidClip()
        {
            return clip != null && !string.IsNullOrEmpty(displayName);
        }
    }
}