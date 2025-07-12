# Generic Button System Documentation

## Overview
The Generic Button System provides a unified, customizable, and feature-rich button component that can be used throughout your backgammon game. It includes animations, theming, audio feedback, and consistent styling.

## Key Components

### 1. GenericButton
The main button component with comprehensive features:
- Multiple button types (Primary, Secondary, Destructive, Success, Icon, Text)
- Built-in animations (Scale, Bounce, Pulse, Shake, Glow)
- Audio and haptic feedback
- Color scheme customization
- Size variants (Small, Medium, Large, Extra Large)

### 2. ButtonFactory
Static factory class for creating buttons programmatically:
- Pre-configured button types
- Automatic component setup
- Backgammon-specific configurations

### 3. ButtonTheme
ScriptableObject for consistent styling:
- Theme-based color schemes
- Centralized visual settings
- Easy theme switching

### 4. ButtonThemeManager
Global theme management:
- Apply themes to all buttons
- Runtime theme switching
- Theme persistence

## Basic Usage

### Creating Buttons in Inspector
1. Add `GenericButton` component to a GameObject
2. Configure button type, size, and colors in inspector
3. Assign visual elements (background image, text, icon)
4. Set up event listeners

```csharp
// In your MonoBehaviour
[SerializeField] private GenericButton playButton;

private void Start()
{
    // Configure button
    playButton.SetButtonType(GenericButton.ButtonType.Primary);
    playButton.SetButtonSize(GenericButton.ButtonSize.Large);
    playButton.SetText("Play Game");
    
    // Add click listener
    playButton.AddClickListener(OnPlayButtonClicked);
}

private void OnPlayButtonClicked()
{
    Debug.Log("Play button clicked!");
    // Start game logic
}
```

### Creating Buttons Programmatically

```csharp
// Create a primary button
var playButton = ButtonFactory.CreatePrimaryButton(
    parentTransform, 
    "Play Game", 
    new Vector2(0, 100), 
    new Vector2(200, 64)
);
playButton.AddClickListener(() => StartGame());

// Create an icon button
var settingsButton = ButtonFactory.CreateIconButton(
    parentTransform, 
    settingsIcon, 
    new Vector2(300, 100)
);
settingsButton.AddClickListener(() => OpenSettings());

// Create a backgammon-specific button
var rollDiceButton = ButtonFactory.CreatePrimaryButton(
    parentTransform, 
    "Roll Dice", 
    Vector2.zero, 
    new Vector2(150, 60)
);
ButtonFactory.ConfigureForBackgammon(rollDiceButton, ButtonFactory.BackgammonButtonType.RollDice);
```

## Button Types

### Primary Buttons
Used for main actions (Play, Start, Confirm)
```csharp
button.buttonType = GenericButton.ButtonType.Primary;
```

### Secondary Buttons  
Used for secondary actions (Settings, Back, Cancel)
```csharp
button.buttonType = GenericButton.ButtonType.Secondary;
```

### Destructive Buttons
Used for warning actions (Delete, Reset, Remove)
```csharp
button.buttonType = GenericButton.ButtonType.Destructive;
```

### Success Buttons
Used for positive actions (Save, Done, Accept)
```csharp
button.buttonType = GenericButton.ButtonType.Success;
```

### Icon Buttons
Used for icon-only buttons
```csharp
button.buttonType = GenericButton.ButtonType.Icon;
button.SetIcon(myIcon);
```

## Animation Types

### Scale Animation
Subtle scaling on hover/press
```csharp
button.animationType = GenericButton.AnimationType.Scale;
```

### Bounce Animation
Bouncy effect on click
```csharp
button.animationType = GenericButton.AnimationType.Bounce;
```

### Pulse Animation
Pulsing effect for attention
```csharp
button.animationType = GenericButton.AnimationType.Pulse;
```

### Shake Animation
Shake effect for errors/warnings
```csharp
button.animationType = GenericButton.AnimationType.Shake;
```

### Glow Animation
Glow effect for highlights
```csharp
button.animationType = GenericButton.AnimationType.Glow;
```

## Theme System

### Creating a Theme
1. Right-click in Project window
2. Create → UI → Button Theme
3. Configure colors, sizes, and settings
4. Save as ScriptableObject

### Applying Themes

#### Method 1: ButtonThemeApplier Component
```csharp
// Add ButtonThemeApplier to parent GameObject
// Assign theme in inspector
// Set applyToChildren = true
// Theme will be applied automatically on Start
```

#### Method 2: Manual Application
```csharp
[SerializeField] private ButtonTheme myTheme;
[SerializeField] private GenericButton[] buttons;

private void Start()
{
    foreach(var button in buttons)
    {
        myTheme.ApplyToButton(button);
    }
}
```

#### Method 3: Global Theme Manager
```csharp
// Set theme globally
ButtonThemeManager.Instance.SetTheme(myTheme);

// Or by name
ButtonThemeManager.Instance.SetTheme("Dark Theme");
```

## Advanced Features

### Custom Color Schemes
```csharp
var customColors = new GenericButton.ColorScheme
{
    normalBackground = Color.blue,
    normalText = Color.white,
    hoverBackground = Color.cyan,
    hoverText = Color.black,
    pressedBackground = Color.darkBlue,
    pressedText = Color.white
};

button.colorScheme = customColors;
```

### Audio Integration
```csharp
button.playAudio = true;
button.hoverSoundId = "ui_hover";
button.clickSoundId = "ui_click";
```

### Haptic Feedback
```csharp
button.enableHaptics = true;
button.hapticType = GenericButton.HapticType.Medium;
```

### Events and Callbacks
```csharp
// Button click
button.OnButtonClicked.AddListener(() => Debug.Log("Clicked!"));

// Hover events
button.OnButtonHoverEnter.AddListener(() => Debug.Log("Hover Enter"));
button.OnButtonHoverExit.AddListener(() => Debug.Log("Hover Exit"));

// Programmatic events
button.AddClickListener(MyMethod);
button.RemoveClickListener(MyMethod);
```

### Runtime Modifications
```csharp
// Change text
button.SetText("New Text");

// Change icon
button.SetIcon(newSprite);

// Change interactability
button.SetInteractable(false);

// Change button type
button.SetButtonType(GenericButton.ButtonType.Success);

// Change size
button.SetButtonSize(GenericButton.ButtonSize.Large);

// Trigger animation
button.PlayClickAnimation();

// Simulate click
button.SimulateClick();
```

## Best Practices

### 1. Consistent Theming
- Create 2-3 themes maximum (Light, Dark, High Contrast)
- Use ButtonThemeManager for global theme switching
- Apply themes consistently across all scenes

### 2. Appropriate Button Types
- **Primary**: Main actions users should take
- **Secondary**: Alternative or less important actions  
- **Destructive**: Actions that delete/remove/reset
- **Success**: Positive confirmations
- **Icon**: Space-constrained areas

### 3. Animation Guidelines
- **Scale**: General hover/focus feedback
- **Bounce**: Playful interactions (games, fun apps)
- **Pulse**: Attention-grabbing (notifications, CTAs)
- **Shake**: Error states or warnings
- **Glow**: Highlights or special states

### 4. Size Guidelines
- **Small (32px)**: Toolbars, compact interfaces
- **Medium (48px)**: Standard buttons, most common
- **Large (64px)**: Primary actions, important buttons
- **Extra Large (80px)**: Hero buttons, main CTAs

### 5. Accessibility
- Minimum button size: 44x44px (iOS) / 48x48px (Android)
- High contrast color schemes for visibility
- Clear, descriptive button text
- Appropriate font sizes for readability

## Backgammon-Specific Usage

### Game UI Buttons
```csharp
// Roll Dice button
ButtonFactory.ConfigureForBackgammon(rollButton, ButtonFactory.BackgammonButtonType.RollDice);

// Done button  
ButtonFactory.ConfigureForBackgammon(doneButton, ButtonFactory.BackgammonButtonType.Done);

// Reset button
ButtonFactory.ConfigureForBackgammon(resetButton, ButtonFactory.BackgammonButtonType.Reset);

// Undo button
ButtonFactory.ConfigureForBackgammon(undoButton, ButtonFactory.BackgammonButtonType.Undo);

// Menu button
ButtonFactory.ConfigureForBackgammon(menuButton, ButtonFactory.BackgammonButtonType.Menu);
```

### Theme Integration
```csharp
// Create backgammon theme
var backgammonTheme = ButtonTheme.CreateBackgammonTheme();

// Apply to button theme manager
ButtonThemeManager.Instance.SetTheme(backgammonTheme);
```

## Migration Guide

### From Legacy Unity Buttons
1. Replace `Button` references with `GenericButton`
2. Change `button.onClick.AddListener()` to `button.AddClickListener()`
3. Change `button.interactable` to `button.SetInteractable()`
4. Apply theme for consistent styling

### Example Migration
```csharp
// OLD
[SerializeField] private Button myButton;
myButton.onClick.AddListener(OnClick);
myButton.interactable = false;

// NEW  
[SerializeField] private GenericButton myButton;
myButton.AddClickListener(OnClick);
myButton.SetInteractable(false);
```

## Performance Considerations

### Optimization Tips
1. **Object Pooling**: For frequently created/destroyed buttons
2. **Animation Batching**: Limit simultaneous animations
3. **Theme Caching**: Cache color calculations
4. **Audio Pooling**: Reuse audio sources

### Memory Management
```csharp
// Clean up event listeners
button.RemoveClickListener(MyMethod);

// Disable animations for better performance
button.enableAnimations = false;

// Reduce audio overhead
button.playAudio = false;
```

## Troubleshooting

### Common Issues

1. **Button not responding to clicks**
   - Check if button is interactable
   - Verify Canvas has GraphicRaycaster
   - Ensure EventSystem exists in scene

2. **Animations not playing**
   - Check `enableAnimations` is true
   - Verify animation duration > 0
   - Check if coroutines are being stopped

3. **Theme not applying**
   - Ensure ButtonTheme asset is assigned
   - Check if ApplyToButton() is called
   - Verify color values are not all the same

4. **Audio not playing**
   - Check `playAudio` is true
   - Verify AudioManager integration
   - Ensure audio clips are assigned

### Debug Helpers
```csharp
// Test button state in editor
[ContextMenu("Preview Hover")]
button.PreviewHover();

[ContextMenu("Test Animation")]  
button.TestAnimation();

// Runtime debugging
Debug.Log($"Button state: {button.isInteractable}");
Debug.Log($"Button type: {button.buttonType}");
```

This generic button system provides a robust, scalable solution for all button needs in your backgammon game while maintaining consistency and providing rich user feedback. 