using Interface;
using UnityEngine;

/// <summary>
/// Scriptable Object for defining button themes and visual styles
/// This allows designers to create consistent button styling across the game
/// </summary>
[CreateAssetMenu(fileName = "New Button Theme", menuName = "UI/Button Theme")]
public class ButtonTheme : ScriptableObject
{
    [Header("Theme Information")]
    public string themeName = "Default";
    public string description = "Default button theme";
    
    [Header("Sprites")]
    public Sprite primaryBackground;
    public Sprite secondaryBackground;
    public Sprite destructiveBackground;
    public Sprite successBackground;
    public Sprite iconBackground;
    
    [Header("Primary Button")]
    public GenericButton.ColorScheme primaryColors = new GenericButton.ColorScheme();
    
    [Header("Secondary Button")]
    public GenericButton.ColorScheme secondaryColors = new GenericButton.ColorScheme();
    
    [Header("Destructive Button")]
    public GenericButton.ColorScheme destructiveColors = new GenericButton.ColorScheme();
    
    [Header("Success Button")]
    public GenericButton.ColorScheme successColors = new GenericButton.ColorScheme();
    
    [Header("Icon Button")]
    public GenericButton.ColorScheme iconColors = new GenericButton.ColorScheme();
    
    [Header("Text Button")]
    public GenericButton.ColorScheme textColors = new GenericButton.ColorScheme();
    
    [Header("Animation Settings")]
    public float defaultAnimationDuration = 0.2f;
    public AnimationCurve defaultAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Size Settings")]
    public Vector2 smallButtonSize = new Vector2(80f, 32f);
    public Vector2 mediumButtonSize = new Vector2(120f, 48f);
    public Vector2 largeButtonSize = new Vector2(200f, 64f);
    public Vector2 extraLargeButtonSize = new Vector2(280f, 80f);
    
    [Header("Font Settings")]
    public float smallFontSize = 12f;
    public float mediumFontSize = 16f;
    public float largeFontSize = 20f;
    public float extraLargeFontSize = 24f;
    
    [Header("Audio")]
    public string defaultHoverSound = "button_hover";
    public string defaultClickSound = "button_click";
    
    /// <summary>
    /// Apply this theme to a GenericButton
    /// </summary>
    public void ApplyToButton(GenericButton button)
    {
        if (button == null) return;
        
        // Apply colors based on button type
        switch (button.buttonType)
        {
            case GenericButton.ButtonType.Primary:
                button.colorScheme = primaryColors;
                if (button.backgroundImage && primaryBackground)
                    button.backgroundImage.sprite = primaryBackground;
                break;
                
            case GenericButton.ButtonType.Secondary:
                button.colorScheme = secondaryColors;
                if (button.backgroundImage && secondaryBackground)
                    button.backgroundImage.sprite = secondaryBackground;
                break;
                
            case GenericButton.ButtonType.Destructive:
                button.colorScheme = destructiveColors;
                if (button.backgroundImage && destructiveBackground)
                    button.backgroundImage.sprite = destructiveBackground;
                break;
                
            case GenericButton.ButtonType.Success:
                button.colorScheme = successColors;
                if (button.backgroundImage && successBackground)
                    button.backgroundImage.sprite = successBackground;
                break;
                
            case GenericButton.ButtonType.Icon:
                button.colorScheme = iconColors;
                if (button.backgroundImage && iconBackground)
                    button.backgroundImage.sprite = iconBackground;
                break;
                
            case GenericButton.ButtonType.Text:
                button.colorScheme = textColors;
                break;
        }
        
        // Apply animation settings
        button.animationDuration = defaultAnimationDuration;
        button.animationCurve = defaultAnimationCurve;
        
        // Apply audio settings
        button.hoverSoundId = defaultHoverSound;
        button.clickSoundId = defaultClickSound;
        
        // Apply size and font settings
        ApplySizeSettings(button);
    }
    
    private void ApplySizeSettings(GenericButton button)
    {
        Vector2 targetSize = button.buttonSize switch
        {
            GenericButton.ButtonSize.Small => smallButtonSize,
            GenericButton.ButtonSize.Medium => mediumButtonSize,
            GenericButton.ButtonSize.Large => largeButtonSize,
            GenericButton.ButtonSize.ExtraLarge => extraLargeButtonSize,
            _ => mediumButtonSize
        };
        
        if (button.GetComponent<RectTransform>())
        {
            button.GetComponent<RectTransform>().sizeDelta = targetSize;
        }
        
        if (button.buttonText != null)
        {
            button.buttonText.fontSize = button.buttonSize switch
            {
                GenericButton.ButtonSize.Small => smallFontSize,
                GenericButton.ButtonSize.Medium => mediumFontSize,
                GenericButton.ButtonSize.Large => largeFontSize,
                GenericButton.ButtonSize.ExtraLarge => extraLargeFontSize,
                _ => mediumFontSize
            };
        }
    }
    
    /// <summary>
    /// Create a default backgammon theme
    /// </summary>
    public static ButtonTheme CreateBackgammonTheme()
    {
        var theme = CreateInstance<ButtonTheme>();
        theme.themeName = "Backgammon Classic";
        theme.description = "Classic backgammon styling with warm colors";
        
        // Primary button (Play, Roll Dice)
        theme.primaryColors.normalBackground = new Color(0.8f, 0.4f, 0.2f); // Rich brown
        theme.primaryColors.normalText = Color.white;
        theme.primaryColors.hoverBackground = new Color(0.9f, 0.5f, 0.3f);
        theme.primaryColors.pressedBackground = new Color(0.7f, 0.3f, 0.1f);
        
        // Secondary button (Settings, Back)
        theme.secondaryColors.normalBackground = new Color(0.9f, 0.9f, 0.8f); // Cream
        theme.secondaryColors.normalText = new Color(0.3f, 0.2f, 0.1f);
        theme.secondaryColors.hoverBackground = new Color(1f, 1f, 0.9f);
        theme.secondaryColors.pressedBackground = new Color(0.8f, 0.8f, 0.7f);
        
        // Destructive button (Reset, Delete)
        theme.destructiveColors.normalBackground = new Color(0.8f, 0.2f, 0.2f); // Red
        theme.destructiveColors.normalText = Color.white;
        theme.destructiveColors.hoverBackground = new Color(0.9f, 0.3f, 0.3f);
        theme.destructiveColors.pressedBackground = new Color(0.7f, 0.1f, 0.1f);
        
        // Success button (Done, Confirm)
        theme.successColors.normalBackground = new Color(0.2f, 0.6f, 0.2f); // Green
        theme.successColors.normalText = Color.white;
        theme.successColors.hoverBackground = new Color(0.3f, 0.7f, 0.3f);
        theme.successColors.pressedBackground = new Color(0.1f, 0.5f, 0.1f);
        
        return theme;
    }
}

/// <summary>
/// Component for applying button themes to all buttons in a hierarchy
/// </summary>
public class ButtonThemeApplier : MonoBehaviour
{
    [Header("Theme Settings")]
    public ButtonTheme theme;
    public bool applyOnStart = true;
    public bool applyToChildren = true;
    
    private void Start()
    {
        if (applyOnStart)
            ApplyTheme();
    }
    
    [ContextMenu("Apply Theme")]
    public void ApplyTheme()
    {
        if (theme == null)
        {
            Debug.LogWarning("No theme assigned to ButtonThemeApplier!");
            return;
        }
        
        GenericButton[] buttons;
        
        if (applyToChildren)
        {
            buttons = GetComponentsInChildren<GenericButton>(true);
        }
        else
        {
            buttons = GetComponents<GenericButton>();
        }
        
        foreach (var button in buttons)
        {
            theme.ApplyToButton(button);
        }
        
        Debug.Log($"Applied theme '{theme.themeName}' to {buttons.Length} buttons");
    }
    
    public void SetTheme(ButtonTheme newTheme)
    {
        theme = newTheme;
        if (Application.isPlaying)
            ApplyTheme();
    }
}

/// <summary>
/// Global button theme manager
/// </summary>
public class ButtonThemeManager : MonoBehaviour
{
    public static ButtonThemeManager Instance { get; private set; }
    
    [Header("Available Themes")]
    public ButtonTheme[] availableThemes;
    public ButtonTheme defaultTheme;
    
    [Header("Current Theme")]
    [SerializeField] private ButtonTheme _currentTheme;
    
    public ButtonTheme CurrentTheme => _currentTheme;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (_currentTheme == null)
                _currentTheme = defaultTheme;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SetTheme(ButtonTheme theme)
    {
        _currentTheme = theme;
        
        // Apply to all existing buttons
        var allButtons = FindObjectsOfType<GenericButton>();
        foreach (var button in allButtons)
        {
            theme.ApplyToButton(button);
        }
        
        // Notify theme change
        MessageBus.Instance.Publish(new UIMessage.ThemeChanged(theme));
    }
    
    public void SetTheme(string themeName)
    {
        var theme = System.Array.Find(availableThemes, t => t.themeName == themeName);
        if (theme != null)
        {
            SetTheme(theme);
        }
        else
        {
            Debug.LogWarning($"Theme '{themeName}' not found!");
        }
    }
    
    public ButtonTheme GetTheme(string themeName)
    {
        return System.Array.Find(availableThemes, t => t.themeName == themeName);
    }
}

/// <summary>
/// UI Messages for theme system
/// </summary>
public static partial class UIMessage
{
    public class ThemeChanged : IMessage
    {
        public readonly ButtonTheme NewTheme;
        
        public ThemeChanged(ButtonTheme theme)
        {
            NewTheme = theme;
        }
    }
} 