using UnityEngine;
using TMPro;

public class InstabilityManager : MonoBehaviour
{
    [Header("Instability Settings")]
    public float maxInstability = 100f;
    public float currentInstability = 0f;
    
    [Header("Instability Increase Rates")]
    public float resetIncreaseAmount = 5f; // How much instability increases per Reset use
    
    [Header("Thresholds")]
    public float safeThreshold = 25f;      // 0-25: Safe
    public float warningThreshold = 50f;   // 26-50: Warning
    public float dangerThreshold = 75f;    // 51-75: Danger
    // 76-100: Critical
    
    [Header("UI Reference")]
    public TextMeshProUGUI instabilityText;
    
    [Header("Admin Dialogue (Optional for now)")]
    public bool showAdminWarnings = true;
    
    // Singleton pattern so we can access this from anywhere
    public static InstabilityManager Instance;
    
    private string currentLevel = "Safe";
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persists between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        UpdateUI();
    }
    
    // Call this function whenever Reset is used
    public void IncreaseInstability(float amount)
    {
        currentInstability += amount;
        
        // Clamp between 0 and max
        currentInstability = Mathf.Clamp(currentInstability, 0f, maxInstability);
        
        // Check if we crossed a threshold
        CheckThresholds();
        
        // Update UI
        UpdateUI();
        
        Debug.Log("Instability increased to: " + currentInstability);
    }
    
    void CheckThresholds()
    {
        string newLevel = GetCurrentLevel();
        
        // If level changed, show warning
        if (newLevel != currentLevel)
        {
            currentLevel = newLevel;
            ShowThresholdWarning(newLevel);
        }
    }
    
    string GetCurrentLevel()
    {
        if (currentInstability <= safeThreshold)
            return "Safe";
        else if (currentInstability <= warningThreshold)
            return "Warning";
        else if (currentInstability <= dangerThreshold)
            return "Danger";
        else
            return "Critical";
    }
    
    void ShowThresholdWarning(string level)
{
    if (!showAdminWarnings) return;
    
    // Use AdminDialogue instead of Debug.Log
    if (AdminDialogue.Instance != null)
    {
        switch (level)
        {
            case "Warning":
                AdminDialogue.Instance.AdminWarning("System strain measurable.");
                break;
            case "Danger":
                AdminDialogue.Instance.AdminWarning("Rollback frequency increasing. System strain critical.");
                break;
            case "Critical":
                AdminDialogue.Instance.AdminWarning("WARNING: Structural integrity compromised.");
                break;
        }
    }
    else
    {
        // Fallback to Debug.Log if AdminDialogue doesn't exist
        Debug.Log("[ADMIN] " + level + " threshold crossed.");
    }
}
    
    void UpdateUI()
    {
        if (instabilityText != null)
        {
            instabilityText.text = "Instability: " + currentInstability.ToString("F0") + "% [" + currentLevel + "]";
            
            // Color code based on level
            switch (currentLevel)
            {
                case "Safe":
                    instabilityText.color = Color.green;
                    break;
                case "Warning":
                    instabilityText.color = Color.yellow;
                    break;
                case "Danger":
                    instabilityText.color = new Color(1f, 0.5f, 0f); // Orange
                    break;
                case "Critical":
                    instabilityText.color = Color.red;
                    break;
            }
        }
    }
    
    // Utility functions
    public float GetInstability()
    {
        return currentInstability;
    }
    
    public string GetLevel()
    {
        return currentLevel;
    }
    
    public void ResetInstability()
    {
        currentInstability = 0f;
        currentLevel = "Safe";
        UpdateUI();
    }
}