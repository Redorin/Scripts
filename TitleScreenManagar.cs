using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameSceneName = "SampleScene";

    [Header("Save System")]
    public bool hasSaveData = false;

    [Header("Sound")]
    public AudioSource keyBeep;

    [Header("Transition")]
    public SceneTransition sceneTransition;

    [Header("Buttons")]
    public Button newGameButton;
    public Button continueButton;
    public Button quitButton;

    void Start()
    {
        CheckForSaveData();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Wire up buttons
        if (newGameButton != null)
            newGameButton.onClick.AddListener(() => { PlayBeep(); StartNewGame(); });
        if (continueButton != null)
            continueButton.onClick.AddListener(() => { PlayBeep(); ContinueGame(); });
        if (quitButton != null)
            quitButton.onClick.AddListener(() => { PlayBeep(); QuitGame(); });
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame || 
                Keyboard.current.numpad1Key.wasPressedThisFrame)
            { PlayBeep(); StartNewGame(); }

            if (Keyboard.current.digit2Key.wasPressedThisFrame || 
                Keyboard.current.numpad2Key.wasPressedThisFrame)
            { PlayBeep(); ContinueGame(); }

            if (Keyboard.current.digit3Key.wasPressedThisFrame || 
                Keyboard.current.numpad3Key.wasPressedThisFrame)
            { PlayBeep(); QuitGame(); }
        }
    }

    void PlayBeep()
    {
        if (keyBeep != null)
            keyBeep.Play();
    }

    void CheckForSaveData()
    {
        if (PlayerPrefs.HasKey("PlayerPosition_X"))
        {
            hasSaveData = true;
            Debug.Log("[SYSTEM] Previous session found.");
        }
        else
        {
            hasSaveData = false;
            Debug.Log("[SYSTEM] No previous session found.");
        }
    }

    public void StartNewGame()
    {
        Debug.Log("[SYSTEM] Initializing new session...");
        PlayerPrefs.DeleteAll();

        if (sceneTransition != null)
            sceneTransition.StartTransition(gameSceneName);
        else
            SceneManager.LoadScene(gameSceneName);
    }

    public void ContinueGame()
    {
        if (hasSaveData)
        {
            Debug.Log("[SYSTEM] Resuming previous session...");
            if (sceneTransition != null)
                sceneTransition.StartTransition(gameSceneName);
            else
                SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.Log("[ERROR] No previous session found.");
        }
    }

    public void QuitGame()
    {
        Debug.Log("[SYSTEM] Force shutdown initiated...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}