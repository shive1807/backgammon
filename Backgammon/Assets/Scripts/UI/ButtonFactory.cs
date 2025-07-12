using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Factory for creating and configuring different types of buttons
/// </summary>
public static class ButtonFactory
{
    /// <summary>
    /// Creates a primary action button (Play, Start Game, etc.)
    /// </summary>
    public static GenericButton CreatePrimaryButton(Transform parent, string text, Vector2 position, Vector2 size)
    {
        var buttonGO = CreateButtonGameObject(parent, text, position, size);
        var genericButton = buttonGO.GetComponent<GenericButton>();
        
        genericButton.buttonType = GenericButton.ButtonType.Primary;
        genericButton.buttonSize = GenericButton.ButtonSize.Large;
        
        return genericButton;
    }
    
    /// <summary>
    /// Creates a secondary action button (Settings, Back, etc.)
    /// </summary>
    public static GenericButton CreateSecondaryButton(Transform parent, string text, Vector2 position, Vector2 size)
    {
        var buttonGO = CreateButtonGameObject(parent, text, position, size);
        var genericButton = buttonGO.GetComponent<GenericButton>();
        
        genericButton.buttonType = GenericButton.ButtonType.Secondary;
        genericButton.buttonSize = GenericButton.ButtonSize.Medium;
        
        return genericButton;
    }
    
    /// <summary>
    /// Creates an icon-only button
    /// </summary>
    public static GenericButton CreateIconButton(Transform parent, Sprite icon, Vector2 position, float size = 48f)
    {
        var buttonGO = CreateButtonGameObject(parent, "", position, new Vector2(size, size));
        var genericButton = buttonGO.GetComponent<GenericButton>();
        
        genericButton.buttonType = GenericButton.ButtonType.Icon;
        genericButton.buttonSize = GenericButton.ButtonSize.Medium;
        
        // Hide text and show icon
        if (genericButton.buttonText != null)
            genericButton.buttonText.gameObject.SetActive(false);
            
        genericButton.SetIcon(icon);
        
        return genericButton;
    }
    
    /// <summary>
    /// Creates a navigation button for menus
    /// </summary>
    public static GenericButton CreateNavButton(Transform parent, string text, Sprite icon = null)
    {
        var buttonGO = CreateButtonGameObject(parent, text, Vector2.zero, new Vector2(200f, 60f));
        var genericButton = buttonGO.GetComponent<GenericButton>();
        
        genericButton.buttonType = GenericButton.ButtonType.Secondary;
        genericButton.buttonSize = GenericButton.ButtonSize.Large;
        genericButton.animationType = GenericButton.AnimationType.Scale;
        
        if (icon != null)
            genericButton.SetIcon(icon);
        
        return genericButton;
    }
    
    /// <summary>
    /// Creates a warning/destructive button (Delete, Reset, etc.)
    /// </summary>
    public static GenericButton CreateWarningButton(Transform parent, string text, Vector2 position, Vector2 size)
    {
        var buttonGO = CreateButtonGameObject(parent, text, position, size);
        var genericButton = buttonGO.GetComponent<GenericButton>();
        
        genericButton.buttonType = GenericButton.ButtonType.Destructive;
        genericButton.buttonSize = GenericButton.ButtonSize.Medium;
        genericButton.animationType = GenericButton.AnimationType.Shake;
        
        return genericButton;
    }
    
    /// <summary>
    /// Creates a success/confirmation button
    /// </summary>
    public static GenericButton CreateSuccessButton(Transform parent, string text, Vector2 position, Vector2 size)
    {
        var buttonGO = CreateButtonGameObject(parent, text, position, size);
        var genericButton = buttonGO.GetComponent<GenericButton>();
        
        genericButton.buttonType = GenericButton.ButtonType.Success;
        genericButton.buttonSize = GenericButton.ButtonSize.Medium;
        genericButton.animationType = GenericButton.AnimationType.Pulse;
        
        return genericButton;
    }
    
    /// <summary>
    /// Creates the base button GameObject with all necessary components
    /// </summary>
    private static GameObject CreateButtonGameObject(Transform parent, string text, Vector2 position, Vector2 size)
    {
        // Create main button GameObject
        var buttonGO = new GameObject("GenericButton");
        buttonGO.transform.SetParent(parent, false);
        
        // Add RectTransform
        var rectTransform = buttonGO.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;
        
        // Add Image component for background
        var image = buttonGO.AddComponent<Image>();
        image.color = Color.white;
        image.type = Image.Type.Sliced;
        
        // Add Button component
        var button = buttonGO.AddComponent<Button>();
        button.targetGraphic = image;
        
        // Add GenericButton component
        var genericButton = buttonGO.AddComponent<GenericButton>();
        genericButton.backgroundImage = image;
        
        // Create text child if text is provided
        if (!string.IsNullOrEmpty(text))
        {
            var textGO = new GameObject("Text");
            textGO.transform.SetParent(buttonGO.transform, false);
            
            var textRectTransform = textGO.AddComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.offsetMin = Vector2.zero;
            textRectTransform.offsetMax = Vector2.zero;
            
            var textComponent = textGO.AddComponent<TextMeshProUGUI>();
            textComponent.text = text;
            textComponent.fontSize = 16f;
            textComponent.color = Color.black;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.fontStyle = FontStyles.Bold;
            
            genericButton.buttonText = textComponent;
        }
        
        return buttonGO;
    }
    
    /// <summary>
    /// Configures a button for game-specific actions
    /// </summary>
    public static void ConfigureForBackgammon(GenericButton button, BackgammonButtonType buttonType)
    {
        switch (buttonType)
        {
            case BackgammonButtonType.RollDice:
                button.SetText("Roll Dice");
                button.buttonType = GenericButton.ButtonType.Primary;
                button.animationType = GenericButton.AnimationType.Bounce;
                button.SetButtonSize(GenericButton.ButtonSize.Large);
                break;
                
            case BackgammonButtonType.Done:
                button.SetText("Done");
                button.buttonType = GenericButton.ButtonType.Success;
                button.animationType = GenericButton.AnimationType.Pulse;
                button.SetButtonSize(GenericButton.ButtonSize.Medium);
                break;
                
            case BackgammonButtonType.Reset:
                button.SetText("Reset");
                button.buttonType = GenericButton.ButtonType.Destructive;
                button.animationType = GenericButton.AnimationType.Shake;
                button.SetButtonSize(GenericButton.ButtonSize.Medium);
                break;
                
            case BackgammonButtonType.Undo:
                button.SetText("Undo");
                button.buttonType = GenericButton.ButtonType.Secondary;
                button.animationType = GenericButton.AnimationType.Scale;
                button.SetButtonSize(GenericButton.ButtonSize.Small);
                break;
                
            case BackgammonButtonType.Menu:
                button.SetText("Menu");
                button.buttonType = GenericButton.ButtonType.Secondary;
                button.animationType = GenericButton.AnimationType.Scale;
                button.SetButtonSize(GenericButton.ButtonSize.Medium);
                break;
        }
    }
    
    public enum BackgammonButtonType
    {
        RollDice,
        Done,
        Reset,
        Undo,
        Menu
    }
} 