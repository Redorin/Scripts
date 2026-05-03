using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AdminDialogue : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    
    [Header("Display Settings")]
    public float displayDuration = 3f; // How long dialogue stays on screen
    public float typingSpeed = 0.05f; // Speed of text appearing (0 = instant)
    public bool useTypingEffect = true;
    
    [Header("Queue Settings")]
    public bool autoDisplayQueue = true; // Automatically show next line after previous finishes
    public float delayBetweenLines = 0.5f; // Delay between queued lines
    
    // Singleton
    public static AdminDialogue Instance;
    
    private Queue<string> dialogueQueue = new Queue<string>();
    private bool isDisplaying = false;
    private Coroutine currentDialogueCoroutine;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Hide dialogue at start
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }
    
    // Main function to show Admin dialogue
    public void ShowDialogue(string message, float duration = -1f)
    {
        // Use custom duration or default
        float displayTime = duration > 0 ? duration : displayDuration;
        
        // Add to queue
        dialogueQueue.Enqueue(message);
        
        // If not currently displaying, start
        if (!isDisplaying)
        {
            StartCoroutine(ProcessDialogueQueue());
        }
    }
    
    // Process queued dialogue lines
    IEnumerator ProcessDialogueQueue()
    {
        isDisplaying = true;
        
        while (dialogueQueue.Count > 0)
        {
            string message = dialogueQueue.Dequeue();
            
            // Show the dialogue
            if (currentDialogueCoroutine != null)
            {
                StopCoroutine(currentDialogueCoroutine);
            }
            currentDialogueCoroutine = StartCoroutine(DisplayDialogue(message));
            
            // Wait for it to finish
            yield return currentDialogueCoroutine;
            
            // Delay before next line
            if (dialogueQueue.Count > 0)
            {
                yield return new WaitForSeconds(delayBetweenLines);
            }
        }
        
        isDisplaying = false;
    }
    
    IEnumerator DisplayDialogue(string message)
    {
        // Show panel
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }
        
        // Display text with typing effect
        if (useTypingEffect && typingSpeed > 0)
        {
            dialogueText.text = "";
            foreach (char letter in message)
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        else
        {
            // Instant display
            dialogueText.text = message;
        }
        
        // Keep on screen for duration
        yield return new WaitForSeconds(displayDuration);
        
        // Hide panel
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }
    
    // Utility functions
    public void ClearQueue()
    {
        dialogueQueue.Clear();
    }
    
    public void SkipCurrentDialogue()
    {
        if (currentDialogueCoroutine != null)
        {
            StopCoroutine(currentDialogueCoroutine);
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }
        }
    }
    
    // Quick access functions for common Admin messages
    public void AdminWarning(string message)
    {
        ShowDialogue("[ADMIN] " + message);
    }
    
    public void AdminInfo(string message)
    {
        ShowDialogue("[SYSTEM] " + message);
    }
}