// Audio/Core/AudioCollection.cs

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MPLCore.GameAudio
{
    [Serializable]
    public class AudioCollection
    {
        [Header("Collection Info")]
        public string collectionName;
        public string description;
        public bool enabled = true;
        
        [Header("Audio Clips")]
        public List<AudioClipDefinition> audioClips = new List<AudioClipDefinition>();
        
        [Header("Playback Settings")]
        public PlaybackMode playbackMode = PlaybackMode.Random;
        public bool shuffleOnRepeat = true;
        public float collectionVolume = 1f;
        
        [Header("Sequence Settings")]
        public float timeBetweenClips = 0f;
        public bool waitForClipToFinish = true;
        
        // Runtime data
        [NonSerialized]
        private int currentIndex = 0;
        [NonSerialized]
        private List<int> shuffledIndices = new List<int>();
        [NonSerialized]
        private bool isShuffled = false;

        public enum PlaybackMode
        {
            Random,
            Sequential,
            RandomNoRepeat,
            WeightedRandom
        }

        public AudioClipDefinition GetNextClip()
        {
            if (audioClips.Count == 0) return null;
            
            switch (playbackMode)
            {
                case PlaybackMode.Random:
                    return audioClips[UnityEngine.Random.Range(0, audioClips.Count)];
                    
                case PlaybackMode.Sequential:
                    return GetSequentialClip();
                    
                case PlaybackMode.RandomNoRepeat:
                    return GetRandomNoRepeatClip();
                    
                case PlaybackMode.WeightedRandom:
                    return GetWeightedRandomClip();
                    
                default:
                    return audioClips[0];
            }
        }

        private AudioClipDefinition GetSequentialClip()
        {
            var clip = audioClips[currentIndex];
            currentIndex = (currentIndex + 1) % audioClips.Count;
            return clip;
        }

        private AudioClipDefinition GetRandomNoRepeatClip()
        {
            if (!isShuffled || shuffledIndices.Count == 0)
            {
                CreateShuffledList();
            }
            
            int index = shuffledIndices[0];
            shuffledIndices.RemoveAt(0);
            
            if (shuffledIndices.Count == 0 && shuffleOnRepeat)
            {
                CreateShuffledList();
            }
            
            return audioClips[index];
        }

        private AudioClipDefinition GetWeightedRandomClip()
        {
            // Simple weighted random - you can enhance this
            var validClips = audioClips.Where(c => c.CanPlay()).ToList();
            if (validClips.Count == 0) return audioClips[0];
            
            return validClips[UnityEngine.Random.Range(0, validClips.Count)];
        }

        private void CreateShuffledList()
        {
            shuffledIndices.Clear();
            for (int i = 0; i < audioClips.Count; i++)
            {
                shuffledIndices.Add(i);
            }
            
            // Fisher-Yates shuffle
            for (int i = shuffledIndices.Count - 1; i > 0; i--)
            {
                int randomIndex = UnityEngine.Random.Range(0, i + 1);
                int temp = shuffledIndices[i];
                shuffledIndices[i] = shuffledIndices[randomIndex];
                shuffledIndices[randomIndex] = temp;
            }
            
            isShuffled = true;
        }

        public List<AudioClipDefinition> GetClipsByCategory(AudioCategory category)
        {
            return audioClips.Where(clip => clip.category == category).ToList();
        }

        public List<AudioClipDefinition> GetClipsByTag(string tag)
        {
            return audioClips.Where(clip => clip.HasTag(tag)).ToList();
        }

        public List<AudioClipDefinition> GetClipsByType(UnityEngine.AudioType type)
        {
            return audioClips.Where(clip => clip.audioType == (AudioType)type).ToList();
        }

        public AudioClipDefinition GetClipById(string id)
        {
            return audioClips.FirstOrDefault(clip => clip.id == id);
        }

        public void AddClip(AudioClipDefinition clip)
        {
            if (clip != null && !audioClips.Contains(clip))
            {
                audioClips.Add(clip);
            }
        }

        public void RemoveClip(AudioClipDefinition clip)
        {
            audioClips.Remove(clip);
        }

        public void RemoveClipById(string id)
        {
            audioClips.RemoveAll(clip => clip.id == id);
        }
    }
}