// Audio/Components/AudioTrigger.cs

using Core.GameAudio;
using UnityEngine;
using Core.GameAudio;

namespace GameAudio.Components
{
    public class AudioTrigger : MonoBehaviour
    {
        [Header("Audio Events")]
        public AudioEvent audioEvent;
        
        [Header("Trigger Settings")]
        public TriggerType triggerType = TriggerType.OnStart;
        public bool triggerOnce = false;
        public float delay = 0f;
        
        [Header("Collision Settings")]
        public string requiredTag = "";
        public LayerMask triggerLayers = -1;
        
        private bool hasTriggered = false;

        public enum TriggerType
        {
            OnStart,
            OnEnable,
            OnDisable,
            OnDestroy,
            OnTriggerEnter,
            OnTriggerExit,
            OnCollisionEnter,
            OnCollisionExit,
            Manual
        }

        private void Start()
        {
            if (triggerType == TriggerType.OnStart)
            {
                TriggerAudio();
            }
        }

        private void OnEnable()
        {
            if (triggerType == TriggerType.OnEnable)
            {
                TriggerAudio();
            }
        }

        private void OnDisable()
        {
            if (triggerType == TriggerType.OnDisable)
            {
                TriggerAudio();
            }
        }

        private void OnDestroy()
        {
            if (triggerType == TriggerType.OnDestroy)
            {
                TriggerAudio();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggerType == TriggerType.OnTriggerEnter && ShouldTrigger(other.gameObject))
            {
                TriggerAudio();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (triggerType == TriggerType.OnTriggerExit && ShouldTrigger(other.gameObject))
            {
                TriggerAudio();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (triggerType == TriggerType.OnCollisionEnter && ShouldTrigger(collision.gameObject))
            {
                TriggerAudio();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (triggerType == TriggerType.OnCollisionExit && ShouldTrigger(collision.gameObject))
            {
                TriggerAudio();
            }
        }

        public void TriggerAudio()
        {
            if (audioEvent == null) return;
            if (triggerOnce && hasTriggered) return;

            if (delay > 0f)
            {
                Invoke(nameof(PlayAudio), delay);
            }
            else
            {
                PlayAudio();
            }

            hasTriggered = true;
        }

        private void PlayAudio()
        {
            audioEvent.Play(gameObject);
        }

        private bool ShouldTrigger(GameObject other)
        {
            // Check layer
            if (triggerLayers != -1 && (triggerLayers & (1 << other.layer)) == 0)
            {
                return false;
            }

            // Check tag
            if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag))
            {
                return false;
            }

            return true;
        }
    }
}