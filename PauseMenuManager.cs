using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class PauseMenuManager : MonoBehaviour
{
    [Header("References")]
    public GameObject pauseMenuCanvas;
    public TextMeshProUGUI resumeButton;
    public TextMeshProUGUI settingsButton;
    public TextMeshProUGUI mainMenuButton;
    public TextMeshProUGUI quitButton;

    [Header("Settings")]
    public string mainMenuSceneName = "TitleScreen";

    [Header("Colors")]
    public Color normalColor    = new Color(0.7f, 0.94f, 1f, 1f);
    public Color highlightColor = new Color(0f, 0.86f, 1f, 1f);

    private bool isPaused = false;
    private int selectedIndex = 0;
    private TextMeshProUGUI[] buttons;

    void Start()
    {
        buttons = new TextMeshProUGUI[]
        {
            resumeButton, settingsButton, mainMenuButton, quitButton
        };

        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }

        if (!isPaused) return;

        if (Keyboard.current.downArrowKey.wasPressedThisFrame ||
            Keyboard.current.sKey.wasPressedThisFrame)
        {
            selectedIndex = (selectedIndex + 1) % buttons.Length;
            UpdateHighlight();
        }

        if (Keyboard.current.upArrowKey.wasPressedThisFrame ||
            Keyboard.current.wKey.wasPressedThisFrame)
        {
            selectedIndex = (selectedIndex - 1 + buttons.Length) % buttons.Length;
            UpdateHighlight();
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame ||
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ConfirmSelection();
        }
    }

    void UpdateHighlight()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;
            buttons[i].color = (i == selectedIndex) ? highlightColor : normalColor;
            string baseText = buttons[i].text.Replace("> ", "").Replace("   ", "");
            buttons[i].text = (i == selectedIndex) ? "> " + baseText : "   " + baseText;
        }
    }

    void ConfirmSelection()
    {
        switch (selectedIndex)
        {
            case 0: ResumeGame();        break;
            case 1: OpenSettings();      break;
            case 2: ReturnToMainMenu();  break;
            case 3: QuitGame();          break;
        }
    }

    void PauseGame()
    {
        isPaused = true;
        selectedIndex = 0;
        Time.timeScale = 0f;
        pauseMenuCanvas.SetActive(true);
        UpdateHighlight();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Show objectives panel on the right
        if (ObjectiveManager.Instance != null)
            ObjectiveManager.Instance.SetObjectivesPanelVisible(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Hide objectives panel
        if (ObjectiveManager.Instance != null)
            ObjectiveManager.Instance.SetObjectivesPanelVisible(false);
    }

    public void OpenSettings()
    {
        Debug.Log("[SYSTEM] Settings not yet implemented.");
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Debug.Log("[SYSTEM] Force shutdown initiated...");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}