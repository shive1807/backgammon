// Audio/Events/AudioEvent.cs

using UnityEngine;

namespace Core.GameAudio
{
    [CreateAssetMenu(fileName = "AudioEvent", menuName = "Game Audio/Audio Event")]
    public class AudioEvent : ScriptableObject
    {
        [Header("Event Configuration")]
        public string eventName;
        public Core.GameAudio.AudioClipDefinition clipDefinition;
        
        [Header("Playback Settings")]
        public bool useRandomPosition = false;
        public float randomPositionRadius = 5f;
        public bool followTransform = false;
        
        [Header("Trigger Conditions")]
        public bool requiresGameObject = false;
        public bool requiresValidPosition = false;
        public float minimumTimeBetweenTriggers = 0f;
        
        private float lastTriggerTime = -1f;

        public void Play()
        {
            Play(null, Vector3.zero);
        }

        public void Play(Vector3 position)
        {
            Play(null, position);
        }

        public void Play(GameObject source)
        {
            Play(source, source ? source.transform.position : Vector3.zero);
        }

        public void Play(GameObject source, Vector3 position)
        {
            if (!CanTrigger()) return;
            
            Vector3 playPosition = position;
            Transform parent = null;
            
            if (useRandomPosition)
            {
                Vector2 randomOffset = Random.insideUnitCircle * randomPositionRadius;
                playPosition += new Vector3(randomOffset.x, 0, randomOffset.y);
            }
            
            if (followTransform && source != null)
            {
                parent = source.transform;
            }
            
            if (Core.GameAudio.AudioManager.Instance != null)
            {
                // Core.GameAudio.AudioManager.Instance.PlayClip(clipDefinition, playPosition, parent);
            }
            
            lastTriggerTime = Time.time;
        }

        private bool CanTrigger()
        {
            if (minimumTimeBetweenTriggers > 0f)
            {
                if (lastTriggerTime > 0f && Time.time - lastTriggerTime < minimumTimeBetweenTriggers)
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}