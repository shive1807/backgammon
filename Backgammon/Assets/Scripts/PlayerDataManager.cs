using UnityEngine;
using System;

/// <summary>
/// Manages player data and progression
/// </summary>
public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }
    
    [Header("Player Data")]
    [SerializeField] private PlayerData _playerData;
    
    public PlayerData Data => _playerData;
    
    private void Awake()
    {
        // Ensure singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialize player data if not already set
        if (_playerData == null)
        {
            _playerData = new PlayerData();
            LoadPlayerData();
        }
    }
    
    private void Start()
    {
        // Subscribe to game events that might affect player data
        MessageBus.Instance.Subscribe<MetaGameMessage.LevelUp>(OnLevelUp);
        MessageBus.Instance.Subscribe<MetaGameMessage.CurrencyChanged>(OnCurrencyChanged);
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            MessageBus.Instance.Unsubscribe<MetaGameMessage.LevelUp>(OnLevelUp);
            MessageBus.Instance.Unsubscribe<MetaGameMessage.CurrencyChanged>(OnCurrencyChanged);
        }
    }
    
    private void LoadPlayerData()
    {
        // Load from PlayerPrefs or use default values
        _playerData.playerName = PlayerPrefs.GetString("PlayerName", "Player");
        _playerData.level = PlayerPrefs.GetInt("PlayerLevel", 1);
        _playerData.coins = PlayerPrefs.GetInt("PlayerCoins", 1000);
        _playerData.gems = PlayerPrefs.GetInt("PlayerGems", 50);
        _playerData.experience = PlayerPrefs.GetInt("PlayerExperience", 0);
        
        Debug.Log($"Loaded player data: {_playerData.playerName}, Level {_playerData.level}");
    }
    
    public void SavePlayerData()
    {
        PlayerPrefs.SetString("PlayerName", _playerData.playerName);
        PlayerPrefs.SetInt("PlayerLevel", _playerData.level);
        PlayerPrefs.SetInt("PlayerCoins", _playerData.coins);
        PlayerPrefs.SetInt("PlayerGems", _playerData.gems);
        PlayerPrefs.SetInt("PlayerExperience", _playerData.experience);
        PlayerPrefs.Save();
        
        Debug.Log("Player data saved");
    }
    
    #region Public API
    
    public void AddCoins(int amount)
    {
        _playerData.coins += amount;
        SavePlayerData();
        MessageBus.Instance.Publish(new MetaGameMessage.CurrencyChanged(_playerData.coins, _playerData.gems));
    }
    
    public void AddGems(int amount)
    {
        _playerData.gems += amount;
        SavePlayerData();
        MessageBus.Instance.Publish(new MetaGameMessage.CurrencyChanged(_playerData.coins, _playerData.gems));
    }
    
    public void AddExperience(int amount)
    {
        _playerData.experience += amount;
        CheckLevelUp();
        SavePlayerData();
    }
    
    public bool SpendCoins(int amount)
    {
        if (_playerData.coins >= amount)
        {
            _playerData.coins -= amount;
            SavePlayerData();
            MessageBus.Instance.Publish(new MetaGameMessage.CurrencyChanged(_playerData.coins, _playerData.gems));
            return true;
        }
        return false;
    }
    
    public bool SpendGems(int amount)
    {
        if (_playerData.gems >= amount)
        {
            _playerData.gems -= amount;
            SavePlayerData();
            MessageBus.Instance.Publish(new MetaGameMessage.CurrencyChanged(_playerData.coins, _playerData.gems));
            return true;
        }
        return false;
    }
    
    public void SetPlayerName(string newName)
    {
        _playerData.playerName = newName;
        SavePlayerData();
    }
    
    #endregion
    
    #region Private Methods
    
    private void CheckLevelUp()
    {
        int requiredXP = _playerData.level * 500; // 500 XP per level
        if (_playerData.experience >= requiredXP)
        {
            _playerData.level++;
            Debug.Log($"Player leveled up to level {_playerData.level}!");
            MessageBus.Instance.Publish(new MetaGameMessage.LevelUp(_playerData.level));
            
            // Give level up rewards
            AddCoins(100 * _playerData.level);
            AddGems(10 * _playerData.level);
        }
    }
    
    private void OnLevelUp(MetaGameMessage.LevelUp message)
    {
        // Handle level up effects
        Debug.Log($"Congratulations! You reached level {message.NewLevel}!");
    }
    
    private void OnCurrencyChanged(MetaGameMessage.CurrencyChanged message)
    {
        // Handle currency change effects
        Debug.Log($"Currency updated: {message.Coins} coins, {message.Gems} gems");
    }
    
    #endregion
    
    #region Editor Helpers
    
    #if UNITY_EDITOR
    [ContextMenu("Reset Player Data")]
    private void EditorResetPlayerData()
    {
        _playerData = new PlayerData();
        SavePlayerData();
        Debug.Log("Player data reset to defaults");
    }
    
    [ContextMenu("Add Test Currency")]
    private void EditorAddTestCurrency()
    {
        AddCoins(500);
        AddGems(25);
        AddExperience(100);
    }
    #endif
    
    #endregion
}

/// <summary>
/// Player data structure
/// </summary>
[System.Serializable]
public class PlayerData
{
    public string playerName = "Player";
    public int level = 1;
    public int coins = 1000;
    public int gems = 50;
    public int experience = 0;
    
    public PlayerData()
    {
        playerName = "Player";
        level = 1;
        coins = 1000;
        gems = 50;
        experience = 0;
    }
} 