// Example: Game-specific Audio Catalog

using MPLCore.GameAudio;
using UnityEngine;

[CreateAssetMenu(fileName = "GameAudioCatalog", menuName = "Game Audio/Game Audio Catalog")]
public class GameAudioCatalog : ScriptableObject
{
    [Header("UI Sounds")]
    public string buttonClickId = "ui_button_click";
    public string buttonHoverId = "ui_button_hover";
    public string menuOpenId = "ui_menu_open";
    public string menuCloseId = "ui_menu_close";
    
    [Header("Gameplay Sounds")]
    public string cardDrawId = "gameplay_card_draw";
    public string cardPlayId = "gameplay_card_play";
    public string cardShuffleId = "gameplay_card_shuffle";
    public string timerWarningId = "gameplay_timer_warning";
    
    [Header("Feedback Sounds")]
    public string winId = "feedback_win";
    public string loseId = "feedback_lose";
    public string errorId = "feedback_error";
    public string successId = "feedback_success";
    
    public void PlayButtonClick(Vector3? position = null)
    {
        AudioManager.Instance.PlayClip(buttonClickId, position);
    }
    
    public void PlayCardDraw(Vector3? position = null)
    {
        AudioManager.Instance.PlayClip(cardDrawId, position);
    }
    
    public void PlayRandomWinSound()
    {
        AudioManager.Instance.PlayRandomFromTag("win");
    }
    
    public void PlayUICategory()
    {
        AudioManager.Instance.PlayRandomFromCategory(AudioCategory.UI);
    }
}

// Example: Using in a MonoBehaviour
public class GameController : MonoBehaviour
{
    public GameAudioCatalog audioCatalog;
    
    private void Start()
    {
        // Play specific clip
        audioCatalog.PlayButtonClick();
        
        // Play by category
        AudioManager.Instance.PlayRandomFromCategory(AudioCategory.UI);
        
        // Play by tag
        AudioManager.Instance.PlayRandomFromTag("victory");
        
        // Play collection
        AudioManager.Instance.PlayCollection("VictorySequence");
        
        // Play with fade
        AudioManager.Instance.FadeInClip("background_music", 2f);
    }
    
    private void OnButtonClick()
    {
        audioCatalog.PlayButtonClick(transform.position);
    }
}