using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Interface;

/// <summary>
/// Manages the home screen UI with play button, settings button, and other menu elements
/// Demonstrates usage of the GenericButton system
/// </summary>
public class HomeScreenCanvasManager : MonoBehaviour
{
    [Header("UI References")]
    public Canvas homeCanvas;
    public CanvasGroup mainContentGroup;
    
    [Header("Title Section")]
    public TextMeshProUGUI gameTitle;
    public Image gameLogo;
    public Animator titleAnimator;
    
    [Header("Main Buttons")]
    public Transform buttonContainer;
    public GenericButton playButton;
    public GenericButton settingsButton;
    public GenericButton achievementsButton;
    public GenericButton leaderboardButton;
    public GenericButton quitButton;
    
    [Header("Secondary Buttons")]
    public GenericButton profileButton;
    public GenericButton shopButton;
    public GenericButton tutorialButton;
    
    [Header("Player Info Panel")]
    public GameObject playerInfoPanel;
    public Image playerAvatar;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerLevelText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI gemsText;
    public Slider experienceSlider;
    public TextMeshProUGUI xpText;
    
    [Header("Daily Challenges Panel")]
    public GameObject dailyChallengesPanel;
    public Transform challengeContainer;
    public GameObject challengePrefab;
    
    [Header("Button Configuration")]
    public ButtonTheme homeScreenTheme;
    public bool createButtonsProgrammatically = false;
    public bool enableButtonAnimations = true;
    
    [Header("Layout Settings")]
    public float buttonSpacing = 20f;
    public Vector2 mainButtonSize = new Vector2(200f, 64f);
    public Vector2 secondaryButtonSize = new Vector2(120f, 48f);
    
    [Header("Animation Settings")]
    public float fadeInDuration = 1f;
    public float buttonAnimationDelay = 0.1f;
    public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private bool _isInitialized = false;
    
    private void Awake()
    {
        InitializeCanvas();
    }
    
    private void Start()
    {
        StartCoroutine(InitializeHomeScreen());
    }
    
    private void OnEnable()
    {
        // Subscribe to relevant messages
        MessageBus.Instance.Subscribe<MetaGameMessage.LevelUp>(OnLevelUp);
        MessageBus.Instance.Subscribe<MetaGameMessage.CurrencyChanged>(OnCurrencyChanged);
        MessageBus.Instance.Subscribe<UIMessage.ThemeChanged>(OnThemeChanged);
    }
    
    private void OnDisable()
    {
        // Unsubscribe from messages
        MessageBus.Instance.Unsubscribe<MetaGameMessage.LevelUp>(OnLevelUp);
        MessageBus.Instance.Unsubscribe<MetaGameMessage.CurrencyChanged>(OnCurrencyChanged);
        MessageBus.Instance.Unsubscribe<UIMessage.ThemeChanged>(OnThemeChanged);
    }
    
    private void InitializeCanvas()
    {
        // Ensure canvas is properly configured
        if (homeCanvas == null)
            homeCanvas = GetComponent<Canvas>();
            
        if (mainContentGroup == null)
            mainContentGroup = GetComponent<CanvasGroup>();
            
        // Set initial state
        if (mainContentGroup != null)
            mainContentGroup.alpha = 0f;
    }
    
    private IEnumerator InitializeHomeScreen()
    {
        // Wait for services to be ready
        yield return StartCoroutine(WaitForServices());
        
        // Setup buttons
        SetupButtons();
        
        // Update player info
        UpdatePlayerInfo();
        
        // Setup daily challenges
        SetupDailyChallenges();
        
        // Apply theme
        ApplyTheme();
        
        // Animate in
        yield return StartCoroutine(AnimateIn());
        
        _isInitialized = true;
    }
    
    private IEnumerator WaitForServices()
    {
        // Wait for essential services with timeout
        float timeout = 5f; // 5 second timeout
        float elapsedTime = 0f;
        
        while (PlayerDataManager.Instance == null && elapsedTime < timeout)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // If PlayerDataManager still null after timeout, log warning but continue
        if (PlayerDataManager.Instance == null)
        {
            Debug.LogWarning("PlayerDataManager not found after timeout. Some features may not work correctly.");
        }
        
        // Wait a frame to ensure everything is properly initialized
        yield return null;
    }
    
    private void SetupButtons()
    {
        if (createButtonsProgrammatically)
        {
            CreateButtonsProgrammatically();
        }
        else
        {
            ConfigureExistingButtons();
        }
        
        SetupButtonListeners();
    }
    
    private void CreateButtonsProgrammatically()
    {
        if (buttonContainer == null)
        {
            Debug.LogWarning("Button container not assigned for programmatic button creation!");
            return;
        }
        
        // Create main buttons
        float yOffset = 0f;
        
        // Play Button - Primary action
        playButton = ButtonFactory.CreatePrimaryButton(
            buttonContainer,
            "Play Game",
            new Vector2(0f, yOffset),
            mainButtonSize
        );
        playButton.animationType = GenericButton.AnimationType.Bounce;
        yOffset -= (mainButtonSize.y + buttonSpacing);
        
        // Settings Button
        settingsButton = ButtonFactory.CreateSecondaryButton(
            buttonContainer,
            "Settings",
            new Vector2(0f, yOffset),
            mainButtonSize
        );
        settingsButton.animationType = GenericButton.AnimationType.Scale;
        yOffset -= (mainButtonSize.y + buttonSpacing);
        
        // Achievements Button
        achievementsButton = ButtonFactory.CreateSecondaryButton(
            buttonContainer,
            "Achievements",
            new Vector2(0f, yOffset),
            mainButtonSize
        );
        achievementsButton.animationType = GenericButton.AnimationType.Pulse;
        yOffset -= (mainButtonSize.y + buttonSpacing);
        
        // Leaderboard Button
        leaderboardButton = ButtonFactory.CreateSecondaryButton(
            buttonContainer,
            "Leaderboard",
            new Vector2(0f, yOffset),
            mainButtonSize
        );
        leaderboardButton.animationType = GenericButton.AnimationType.Scale;
        yOffset -= (mainButtonSize.y + buttonSpacing);
        
        // Quit Button
        quitButton = ButtonFactory.CreateWarningButton(
            buttonContainer,
            "Quit",
            new Vector2(0f, yOffset),
            new Vector2(mainButtonSize.x * 0.7f, mainButtonSize.y * 0.8f)
        );
        quitButton.animationType = GenericButton.AnimationType.Shake;
        
        // Create secondary buttons (smaller, positioned differently)
        CreateSecondaryButtons();
        
        Debug.Log("Created home screen buttons programmatically");
    }
    
    private void CreateSecondaryButtons()
    {
        // Create profile button (top-left)
        profileButton = ButtonFactory.CreateIconButton(
            homeCanvas.transform,
            null, // Icon will be set separately
            new Vector2(-350f, 250f),
            48f
        );
        
        // Create shop button (top-right)
        shopButton = ButtonFactory.CreateSecondaryButton(
            homeCanvas.transform,
            "Shop",
            new Vector2(300f, 250f),
            secondaryButtonSize
        );
        
        // Create tutorial button
        tutorialButton = ButtonFactory.CreateSecondaryButton(
            homeCanvas.transform,
            "Tutorial",
            new Vector2(0f, -200f),
            secondaryButtonSize
        );
    }
    
    private void ConfigureExistingButtons()
    {
        // Configure buttons that are already assigned in inspector
        if (playButton != null)
        {
            playButton.SetButtonType(GenericButton.ButtonType.Primary);
            playButton.SetButtonSize(GenericButton.ButtonSize.Large);
            playButton.animationType = GenericButton.AnimationType.Bounce;
            playButton.SetText("Play Game");
        }
        
        if (settingsButton != null)
        {
            settingsButton.SetButtonType(GenericButton.ButtonType.Secondary);
            settingsButton.SetButtonSize(GenericButton.ButtonSize.Medium);
            settingsButton.animationType = GenericButton.AnimationType.Scale;
            settingsButton.SetText("Settings");
        }
        
        if (achievementsButton != null)
        {
            achievementsButton.SetButtonType(GenericButton.ButtonType.Secondary);
            achievementsButton.SetButtonSize(GenericButton.ButtonSize.Medium);
            achievementsButton.animationType = GenericButton.AnimationType.Pulse;
            achievementsButton.SetText("Achievements");
        }
        
        if (leaderboardButton != null)
        {
            leaderboardButton.SetButtonType(GenericButton.ButtonType.Secondary);
            leaderboardButton.SetButtonSize(GenericButton.ButtonSize.Medium);
            leaderboardButton.animationType = GenericButton.AnimationType.Scale;
            leaderboardButton.SetText("Leaderboard");
        }
        
        if (quitButton != null)
        {
            quitButton.SetButtonType(GenericButton.ButtonType.Destructive);
            quitButton.SetButtonSize(GenericButton.ButtonSize.Small);
            quitButton.animationType = GenericButton.AnimationType.Shake;
            quitButton.SetText("Quit");
        }
        
        // Configure secondary buttons
        if (profileButton != null)
        {
            profileButton.SetButtonType(GenericButton.ButtonType.Icon);
            profileButton.SetButtonSize(GenericButton.ButtonSize.Medium);
        }
        
        if (shopButton != null)
        {
            shopButton.SetButtonType(GenericButton.ButtonType.Secondary);
            shopButton.SetButtonSize(GenericButton.ButtonSize.Medium);
            shopButton.SetText("Shop");
        }
        
        if (tutorialButton != null)
        {
            tutorialButton.SetButtonType(GenericButton.ButtonType.Secondary);
            tutorialButton.SetButtonSize(GenericButton.ButtonSize.Medium);
            tutorialButton.SetText("Tutorial");
        }
    }
    
    private void SetupButtonListeners()
    {
        // Main buttons
        if (playButton != null)
            playButton.AddClickListener(OnPlayButtonClicked);
            
        if (settingsButton != null)
            settingsButton.AddClickListener(OnSettingsButtonClicked);
            
        if (achievementsButton != null)
            achievementsButton.AddClickListener(OnAchievementsButtonClicked);
            
        if (leaderboardButton != null)
            leaderboardButton.AddClickListener(OnLeaderboardButtonClicked);
            
        if (quitButton != null)
            quitButton.AddClickListener(OnQuitButtonClicked);
        
        // Secondary buttons
        if (profileButton != null)
            profileButton.AddClickListener(OnProfileButtonClicked);
            
        if (shopButton != null)
            shopButton.AddClickListener(OnShopButtonClicked);
            
        if (tutorialButton != null)
            tutorialButton.AddClickListener(OnTutorialButtonClicked);
    }
    
    private void ApplyTheme()
    {
        if (homeScreenTheme == null)
        {
            // Use global theme if available
            if (ButtonThemeManager.Instance != null && ButtonThemeManager.Instance.CurrentTheme != null)
            {
                homeScreenTheme = ButtonThemeManager.Instance.CurrentTheme;
            }
        }
        
        if (homeScreenTheme != null)
        {
            // Apply theme to all buttons
            var allButtons = GetComponentsInChildren<GenericButton>();
            foreach (var button in allButtons)
            {
                homeScreenTheme.ApplyToButton(button);
            }
        }
    }
    
    private void UpdatePlayerInfo()
    {
        if (PlayerDataManager.Instance == null) return;
        
        var playerData = PlayerDataManager.Instance.Data;
        
        // Update player info UI
        if (playerNameText != null)
            playerNameText.text = playerData.playerName;
            
        if (playerLevelText != null)
            playerLevelText.text = $"Level {playerData.level}";
            
        if (coinsText != null)
            coinsText.text = playerData.coins.ToString();
            
        if (gemsText != null)
            gemsText.text = playerData.gems.ToString();
        
        // Update experience slider
        if (experienceSlider != null)
        {
            int currentLevelXP = (playerData.level - 1) * 500;
            int nextLevelXP = playerData.level * 500;
            float progress = (float)(playerData.experience - currentLevelXP) / (nextLevelXP - currentLevelXP);
            experienceSlider.value = progress;
        }
        
        if (xpText != null)
        {
            int currentLevelXP = (playerData.level - 1) * 500;
            int nextLevelXP = playerData.level * 500;
            int currentXP = playerData.experience - currentLevelXP;
            int requiredXP = nextLevelXP - currentLevelXP;
            xpText.text = $"{currentXP} / {requiredXP} XP";
        }
    }
    
    private void SetupDailyChallenges()
    {
        if (dailyChallengesPanel == null || challengeContainer == null) return;
        
        // Clear existing challenges
        foreach (Transform child in challengeContainer)
        {
            if (child.gameObject != challengePrefab)
                Destroy(child.gameObject);
        }
        
        // Add sample daily challenges (replace with actual challenge system)
        CreateSampleChallenges();
    }
    
    private void CreateSampleChallenges()
    {
        // Sample challenges - replace with actual data
        var challenges = new[]
        {
            new { description = "Win 3 games", progress = 1, target = 3, reward = "100 Coins" },
            new { description = "Play 5 matches", progress = 3, target = 5, reward = "50 XP" },
            new { description = "Use all dice in a turn", progress = 0, target = 1, reward = "25 Gems" }
        };
        
        foreach (var challenge in challenges)
        {
            if (challengePrefab != null)
            {
                var challengeObj = Instantiate(challengePrefab, challengeContainer);
                // Configure challenge UI - this would depend on your challenge prefab structure
                var challengeUI = challengeObj.GetComponent<DailyChallengeUI>();
                if (challengeUI != null)
                {
                    // challengeUI.SetChallenge(challenge);
                }
            }
        }
    }
    
    #region Button Event Handlers
    
    private void OnPlayButtonClicked()
    {
        Debug.Log("Play button clicked!");
        
        // Add special animation for play button
        if (enableButtonAnimations)
            playButton.PlayClickAnimation();
        
        // Transition to game selection or directly to game
        StartCoroutine(TransitionToGame());
    }
    
    private void OnSettingsButtonClicked()
    {
        Debug.Log("Settings button clicked!");
        
        // Navigate to settings screen
        // SceneManager.Instance.LoadScene(SceneManager.SceneType.Settings);
        
        // Or show settings panel
        ShowSettingsPanel();
    }
    
    private void OnAchievementsButtonClicked()
    {
        Debug.Log("Achievements button clicked!");
        
        // Navigate to achievements screen
        // SceneManager.Instance.LoadScene(SceneManager.SceneType.Achievements);
    }
    
    private void OnLeaderboardButtonClicked()
    {
        Debug.Log("Leaderboard button clicked!");
        
        // Navigate to leaderboard screen
        // SceneManager.Instance.LoadScene(SceneManager.SceneType.Leaderboard);
    }
    
    private void OnQuitButtonClicked()
    {
        Debug.Log("Quit button clicked!");
        
        // Show confirmation dialog
        ShowQuitConfirmation();
    }
    
    private void OnProfileButtonClicked()
    {
        Debug.Log("Profile button clicked!");
        
        // Show player profile panel
        ShowProfilePanel();
    }
    
    private void OnShopButtonClicked()
    {
        Debug.Log("Shop button clicked!");
        
        // Navigate to shop
        // SceneManager.Instance.LoadScene(SceneManager.SceneType.Shop);
    }
    
    private void OnTutorialButtonClicked()
    {
        Debug.Log("Tutorial button clicked!");
        
        // Start tutorial
        // SceneManager.Instance.LoadScene(SceneManager.SceneType.Tutorial);
    }
    
    #endregion
    
    #region Animation Methods
    
    private IEnumerator AnimateIn()
    {
        if (mainContentGroup == null) yield break;
        
        // Animate title first
        if (titleAnimator != null)
        {
            titleAnimator.SetTrigger("FadeIn");
            yield return new WaitForSeconds(0.5f);
        }
        
        // Fade in main content
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeInDuration;
            mainContentGroup.alpha = fadeInCurve.Evaluate(t);
            yield return null;
        }
        mainContentGroup.alpha = 1f;
        
        // Animate buttons in sequence
        yield return StartCoroutine(AnimateButtonsIn());
    }
    
    private IEnumerator AnimateButtonsIn()
    {
        var buttons = new GenericButton[] 
        { 
            playButton, settingsButton, achievementsButton, 
            leaderboardButton, quitButton 
        };
        
        foreach (var button in buttons)
        {
            if (button != null && enableButtonAnimations)
            {
                button.PlayClickAnimation();
                yield return new WaitForSeconds(buttonAnimationDelay);
            }
        }
    }
    
    private IEnumerator TransitionToGame()
    {
        // Animate out
        if (mainContentGroup != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeInDuration * 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (fadeInDuration * 0.5f);
                mainContentGroup.alpha = 1f - t;
                yield return null;
            }
            mainContentGroup.alpha = 0f;
        }
        
        // Load game scene
        // SceneManager.Instance.LoadScene(SceneManager.SceneType.GameScene);
        
        // For now, just log
        Debug.Log("Transitioning to game...");
    }
    
    #endregion
    
    #region UI Panel Methods
    
    private void ShowSettingsPanel()
    {
        // Implementation for showing settings panel
        Debug.Log("Showing settings panel...");
    }
    
    private void ShowQuitConfirmation()
    {
        // Implementation for quit confirmation dialog
        Debug.Log("Showing quit confirmation...");
    }
    
    private void ShowProfilePanel()
    {
        // Implementation for showing profile panel
        Debug.Log("Showing profile panel...");
    }
    
    #endregion
    
    #region Message Handlers
    
    private void OnLevelUp(MetaGameMessage.LevelUp message)
    {
        Debug.Log($"Player leveled up to {message.NewLevel}!");
        UpdatePlayerInfo();
        
        // Show level up celebration
        // ShowLevelUpCelebration(message.NewLevel);
    }
    
    private void OnCurrencyChanged(MetaGameMessage.CurrencyChanged message)
    {
        UpdatePlayerInfo();
    }
    
    private void OnThemeChanged(UIMessage.ThemeChanged message)
    {
        homeScreenTheme = message.NewTheme;
        ApplyTheme();
    }
    
    #endregion
    
    #region Public API
    
    public void SetButtonsInteractable(bool interactable)
    {
        var buttons = GetComponentsInChildren<GenericButton>();
        foreach (var button in buttons)
        {
            button.SetInteractable(interactable);
        }
    }
    
    public void RefreshPlayerInfo()
    {
        UpdatePlayerInfo();
    }
    
    public void RefreshDailyChallenges()
    {
        SetupDailyChallenges();
    }
    
    public void SetTheme(ButtonTheme theme)
    {
        homeScreenTheme = theme;
        ApplyTheme();
    }
    
    #endregion
    
    #region Editor Helpers
    
    #if UNITY_EDITOR
    [ContextMenu("Create Buttons")]
    private void EditorCreateButtons()
    {
        createButtonsProgrammatically = true;
        CreateButtonsProgrammatically();
    }
    
    [ContextMenu("Apply Theme")]
    private void EditorApplyTheme()
    {
        ApplyTheme();
    }
    
    [ContextMenu("Update Player Info")]
    private void EditorUpdatePlayerInfo()
    {
        UpdatePlayerInfo();
    }
    #endif
    
    #endregion
}

/// <summary>
/// Placeholder for daily challenge UI component
/// </summary>
public class DailyChallengeUI : MonoBehaviour
{
    // Placeholder for daily challenge UI implementation
    public void SetChallenge(object challenge)
    {
        // Implementation would depend on challenge data structure
    }
}

/// <summary>
/// Meta game messages for player progression
/// </summary>
public static class MetaGameMessage
{
    public class LevelUp : IMessage
    {
        public readonly int NewLevel;
        public LevelUp(int newLevel) => NewLevel = newLevel;
    }
    
    public class CurrencyChanged : IMessage
    {
        public readonly int Coins;
        public readonly int Gems;
        public CurrencyChanged(int coins, int gems)
        {
            Coins = coins;
            Gems = gems;
        }
    }
} 