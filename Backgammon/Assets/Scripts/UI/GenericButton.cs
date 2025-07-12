using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

/// <summary>
/// Generic button component that can be used for all buttons in the game
/// Provides consistent behavior, animations, audio, and visual feedback
/// </summary>
public class GenericButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Button Configuration")]
    public ButtonType buttonType = ButtonType.Primary;
    public ButtonSize buttonSize = ButtonSize.Medium;
    public bool isInteractable = true;
    
    [Header("Visual Elements")]
    public Image backgroundImage;
    public Image iconImage;
    public TextMeshProUGUI buttonText;
    public GameObject glowEffect;
    public GameObject[] additionalElements;
    
    [Header("Colors")]
    public ColorScheme colorScheme;
    
    [Header("Animation Settings")]
    public bool enableAnimations = true;
    public AnimationType animationType = AnimationType.Scale;
    public float animationDuration = 0.2f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Audio")]
    public bool playAudio = true;
    public string hoverSoundId = "button_hover";
    public string clickSoundId = "button_click";
    
    [Header("Haptics")]
    public bool enableHaptics = true;
    public HapticType hapticType = HapticType.Light;
    
    [Header("Events")]
    public UnityEvent OnButtonClicked;
    public UnityEvent OnButtonHoverEnter;
    public UnityEvent OnButtonHoverExit;
    
    // Private variables
    private Button _button;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Vector3 _originalScale;
    private Color _originalColor;
    private bool _isHovered;
    private bool _isPressed;
    private Coroutine _currentAnimation;
    
    public enum ButtonType
    {
        Primary,        // Main action buttons (Play, Start Game)
        Secondary,      // Secondary actions (Settings, Back)
        Destructive,    // Warning actions (Delete, Reset)
        Success,        // Positive actions (Confirm, Save)
        Icon,          // Icon-only buttons
        Text,          // Text-only buttons
        Toggle         // Toggle buttons
    }
    
    public enum ButtonSize
    {
        Small,      // 32px height
        Medium,     // 48px height  
        Large,      // 64px height
        ExtraLarge  // 80px height
    }
    
    public enum AnimationType
    {
        None,
        Scale,
        Bounce,
        Pulse,
        Shake,
        Glow
    }
    
    public enum HapticType
    {
        None,
        Light,
        Medium,
        Heavy
    }
    
    [System.Serializable]
    public class ColorScheme
    {
        [Header("Normal State")]
        public Color normalBackground = Color.white;
        public Color normalText = Color.black;
        public Color normalIcon = Color.black;
        
        [Header("Hover State")]
        public Color hoverBackground = Color.gray;
        public Color hoverText = Color.black;
        public Color hoverIcon = Color.black;
        
        [Header("Pressed State")]
        public Color pressedBackground = Color.gray;
        public Color pressedText = Color.white;
        public Color pressedIcon = Color.white;
        
        [Header("Disabled State")]
        public Color disabledBackground = Color.gray;
        public Color disabledText = Color.gray;
        public Color disabledIcon = Color.gray;
    }
    
    private void Awake()
    {
        InitializeComponents();
        SetupButton();
        ApplyButtonStyle();
    }
    
    private void Start()
    {
        _originalScale = _rectTransform.localScale;
        _originalColor = backgroundImage ? backgroundImage.color : Color.white;
        
        // Apply initial state
        SetVisualState(ButtonState.Normal);
    }
    
    private void InitializeComponents()
    {
        _button = GetComponent<Button>();
        if (_button == null)
        {
            _button = gameObject.AddComponent<Button>();
        }
        
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        _rectTransform = GetComponent<RectTransform>();
        
        // Auto-find components if not assigned
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
        
        if (buttonText == null)
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
        
        if (iconImage == null)
        {
            var images = GetComponentsInChildren<Image>();
            if (images.Length > 1)
                iconImage = images[1]; // Assume first is background, second is icon
        }
    }
    
    private void SetupButton()
    {
        // Remove default button click to use our custom handling
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClick);
        
        // Configure button properties
        _button.interactable = isInteractable;
        _button.transition = Selectable.Transition.None; // We handle transitions manually
    }
    
    private void ApplyButtonStyle()
    {
        // Apply size
        ApplySize();
        
        // Apply colors based on button type
        ApplyTypeColors();
        
        // Setup navigation
        SetupNavigation();
    }
    
    private void ApplySize()
    {
        float height = buttonSize switch
        {
            ButtonSize.Small => 32f,
            ButtonSize.Medium => 48f,
            ButtonSize.Large => 64f,
            ButtonSize.ExtraLarge => 80f,
            _ => 48f
        };
        
        _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, height);
        
        // Adjust font size
        if (buttonText != null)
        {
            buttonText.fontSize = buttonSize switch
            {
                ButtonSize.Small => 12f,
                ButtonSize.Medium => 16f,
                ButtonSize.Large => 20f,
                ButtonSize.ExtraLarge => 24f,
                _ => 16f
            };
        }
    }
    
    private void ApplyTypeColors()
    {
        // Set default colors based on button type
        switch (buttonType)
        {
            case ButtonType.Primary:
                colorScheme.normalBackground = new Color(0.2f, 0.4f, 0.8f, 1f);
                colorScheme.normalText = Color.white;
                break;
                
            case ButtonType.Secondary:
                colorScheme.normalBackground = new Color(0.9f, 0.9f, 0.9f, 1f);
                colorScheme.normalText = Color.black;
                break;
                
            case ButtonType.Destructive:
                colorScheme.normalBackground = new Color(0.8f, 0.2f, 0.2f, 1f);
                colorScheme.normalText = Color.white;
                break;
                
            case ButtonType.Success:
                colorScheme.normalBackground = new Color(0.2f, 0.8f, 0.2f, 1f);
                colorScheme.normalText = Color.white;
                break;
        }
    }
    
    private void SetupNavigation()
    {
        var navigation = _button.navigation;
        navigation.mode = Navigation.Mode.Automatic;
        _button.navigation = navigation;
    }
    
    #region Event Handlers
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        _isHovered = true;
        SetVisualState(ButtonState.Hover);
        
        if (playAudio)
            PlayAudio(hoverSoundId);
            
        OnButtonHoverEnter?.Invoke();
        
        if (enableAnimations)
            StartAnimation(AnimationType.Scale, true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        _isHovered = false;
        SetVisualState(ButtonState.Normal);
        
        OnButtonHoverExit?.Invoke();
        
        if (enableAnimations)
            StartAnimation(AnimationType.Scale, false);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        _isPressed = true;
        SetVisualState(ButtonState.Pressed);
        
        if (enableAnimations)
            StartAnimation(AnimationType.Bounce, true);
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isInteractable) return;
        
        _isPressed = false;
        SetVisualState(_isHovered ? ButtonState.Hover : ButtonState.Normal);
        
        if (enableAnimations)
            StartAnimation(AnimationType.Bounce, false);
    }
    
    private void OnClick()
    {
        if (!isInteractable) return;
        
        if (playAudio)
            PlayAudio(clickSoundId);
            
        if (enableHaptics)
            TriggerHaptic();
        
        OnButtonClicked?.Invoke();
        
        if (enableAnimations && animationType == AnimationType.Pulse)
            StartAnimation(AnimationType.Pulse, true);
    }
    
    #endregion
    
    #region Visual States
    
    private enum ButtonState
    {
        Normal,
        Hover,
        Pressed,
        Disabled
    }
    
    private void SetVisualState(ButtonState state)
    {
        Color bgColor, textColor, iconColor;
        
        switch (state)
        {
            case ButtonState.Normal:
                bgColor = colorScheme.normalBackground;
                textColor = colorScheme.normalText;
                iconColor = colorScheme.normalIcon;
                break;
                
            case ButtonState.Hover:
                bgColor = colorScheme.hoverBackground;
                textColor = colorScheme.hoverText;
                iconColor = colorScheme.hoverIcon;
                break;
                
            case ButtonState.Pressed:
                bgColor = colorScheme.pressedBackground;
                textColor = colorScheme.pressedText;
                iconColor = colorScheme.pressedIcon;
                break;
                
            case ButtonState.Disabled:
                bgColor = colorScheme.disabledBackground;
                textColor = colorScheme.disabledText;
                iconColor = colorScheme.disabledIcon;
                break;
                
            default:
                bgColor = colorScheme.normalBackground;
                textColor = colorScheme.normalText;
                iconColor = colorScheme.normalIcon;
                break;
        }
        
        // Apply colors with smooth transition
        if (backgroundImage != null)
            StartCoroutine(AnimateColor(backgroundImage, bgColor));
            
        if (buttonText != null)
            StartCoroutine(AnimateTextColor(buttonText, textColor));
            
        if (iconImage != null)
            StartCoroutine(AnimateColor(iconImage, iconColor));
    }
    
    private IEnumerator AnimateColor(Image image, Color targetColor)
    {
        Color startColor = image.color;
        float elapsed = 0f;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            image.color = Color.Lerp(startColor, targetColor, animationCurve.Evaluate(t));
            yield return null;
        }
        
        image.color = targetColor;
    }
    
    private IEnumerator AnimateTextColor(TextMeshProUGUI text, Color targetColor)
    {
        Color startColor = text.color;
        float elapsed = 0f;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            text.color = Color.Lerp(startColor, targetColor, animationCurve.Evaluate(t));
            yield return null;
        }
        
        text.color = targetColor;
    }
    
    #endregion
    
    #region Animations
    
    private void StartAnimation(AnimationType type, bool forward)
    {
        if (_currentAnimation != null)
            StopCoroutine(_currentAnimation);
            
        switch (type)
        {
            case AnimationType.Scale:
                _currentAnimation = StartCoroutine(ScaleAnimation(forward));
                break;
                
            case AnimationType.Bounce:
                _currentAnimation = StartCoroutine(BounceAnimation());
                break;
                
            case AnimationType.Pulse:
                _currentAnimation = StartCoroutine(PulseAnimation());
                break;
                
            case AnimationType.Shake:
                _currentAnimation = StartCoroutine(ShakeAnimation());
                break;
                
            case AnimationType.Glow:
                _currentAnimation = StartCoroutine(GlowAnimation(forward));
                break;
        }
    }
    
    private IEnumerator ScaleAnimation(bool scaleUp)
    {
        Vector3 startScale = _rectTransform.localScale;
        Vector3 targetScale = scaleUp ? _originalScale * 1.1f : _originalScale;
        
        float elapsed = 0f;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            _rectTransform.localScale = Vector3.Lerp(startScale, targetScale, animationCurve.Evaluate(t));
            yield return null;
        }
        
        _rectTransform.localScale = targetScale;
    }
    
    private IEnumerator BounceAnimation()
    {
        Vector3 startScale = _rectTransform.localScale;
        Vector3 bounceScale = _originalScale * 0.9f;
        
        // Scale down
        float elapsed = 0f;
        while (elapsed < animationDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (animationDuration * 0.5f);
            _rectTransform.localScale = Vector3.Lerp(startScale, bounceScale, t);
            yield return null;
        }
        
        // Scale back up
        elapsed = 0f;
        while (elapsed < animationDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (animationDuration * 0.5f);
            _rectTransform.localScale = Vector3.Lerp(bounceScale, _originalScale, t);
            yield return null;
        }
        
        _rectTransform.localScale = _originalScale;
    }
    
    private IEnumerator PulseAnimation()
    {
        Vector3 startScale = _originalScale;
        Vector3 pulseScale = _originalScale * 1.2f;
        
        // Pulse out
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            _rectTransform.localScale = Vector3.Lerp(startScale, pulseScale, animationCurve.Evaluate(t));
            yield return null;
        }
        
        // Pulse back
        elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            _rectTransform.localScale = Vector3.Lerp(pulseScale, startScale, animationCurve.Evaluate(t));
            yield return null;
        }
        
        _rectTransform.localScale = startScale;
    }
    
    private IEnumerator ShakeAnimation()
    {
        Vector3 startPosition = _rectTransform.localPosition;
        float shakeIntensity = 5f;
        
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-shakeIntensity, shakeIntensity);
            float y = Random.Range(-shakeIntensity, shakeIntensity);
            _rectTransform.localPosition = startPosition + new Vector3(x, y, 0);
            yield return null;
        }
        
        _rectTransform.localPosition = startPosition;
    }
    
    private IEnumerator GlowAnimation(bool glowUp)
    {
        if (glowEffect == null) yield break;
        
        CanvasGroup glowGroup = glowEffect.GetComponent<CanvasGroup>();
        if (glowGroup == null)
            glowGroup = glowEffect.AddComponent<CanvasGroup>();
            
        float startAlpha = glowGroup.alpha;
        float targetAlpha = glowUp ? 1f : 0f;
        
        glowEffect.SetActive(true);
        
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            glowGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }
        
        glowGroup.alpha = targetAlpha;
        
        if (!glowUp)
            glowEffect.SetActive(false);
    }
    
    #endregion
    
    #region Audio & Haptics
    
    private void PlayAudio(string soundId)
    {
        // Integrate with your audio system
        // if (GameServices.Instance?.AudioManager != null)
        // {
        //     GameServices.Instance.AudioManager.PlaySFX(soundId);
        // }
    }
    
    private void TriggerHaptic()
    {
        #if UNITY_ANDROID || UNITY_IOS
        switch (hapticType)
        {
            case HapticType.Light:
                Handheld.Vibrate();
                break;
            case HapticType.Medium:
                Handheld.Vibrate();
                break;
            case HapticType.Heavy:
                Handheld.Vibrate();
                break;
        }
        #endif
    }
    
    #endregion
    
    #region Public API
    
    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
        _button.interactable = interactable;
        _canvasGroup.alpha = interactable ? 1f : 0.6f;
        
        SetVisualState(interactable ? ButtonState.Normal : ButtonState.Disabled);
    }
    
    public void SetText(string text)
    {
        if (buttonText != null)
            buttonText.text = text;
    }
    
    public void SetIcon(Sprite icon)
    {
        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.gameObject.SetActive(icon != null);
        }
    }
    
    public void SetButtonType(ButtonType type)
    {
        buttonType = type;
        ApplyTypeColors();
        SetVisualState(ButtonState.Normal);
    }
    
    public void SetButtonSize(ButtonSize size)
    {
        buttonSize = size;
        ApplySize();
    }
    
    public void PlayClickAnimation()
    {
        if (enableAnimations)
            StartAnimation(AnimationType.Pulse, true);
    }
    
    public void AddClickListener(UnityAction action)
    {
        OnButtonClicked.AddListener(action);
    }
    
    public void RemoveClickListener(UnityAction action)
    {
        OnButtonClicked.RemoveListener(action);
    }
    
    public void SimulateClick()
    {
        OnClick();
    }
    
    #endregion
    
    #region Editor Helpers
    
    #if UNITY_EDITOR
    [ContextMenu("Preview Hover")]
    private void PreviewHover()
    {
        SetVisualState(ButtonState.Hover);
    }
    
    [ContextMenu("Preview Pressed")]
    private void PreviewPressed()
    {
        SetVisualState(ButtonState.Pressed);
    }
    
    [ContextMenu("Preview Normal")]
    private void PreviewNormal()
    {
        SetVisualState(ButtonState.Normal);
    }
    
    [ContextMenu("Test Animation")]
    private void TestAnimation()
    {
        StartAnimation(animationType, true);
    }
    #endif
    
    #endregion
} 