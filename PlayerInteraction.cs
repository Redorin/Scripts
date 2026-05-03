using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public Camera playerCamera;

    [Header("UI")]
    public GameObject interactionPrompt;
    public TextMeshProUGUI interactionTooltip;

    [Header("Crosshair")]
    public TextMeshProUGUI crosshairText;
    public string normalCrosshair = "+";
    public string interactCrosshair = "●";
    public Color normalColor = new Color(1f, 1f, 1f, 0.8f);
    public Color interactColor = new Color(0f, 1f, 1f, 1f);
    public float normalSize = 20f;
    public float interactSize = 28f;

    private InteractableDoor currentDoor;
    private DoorWithTrap currentTrapDoor;
    private InteractableObject currentObject;
    private Teleporter currentTeleporter;
    private HoldableItem currentHoldableItem;
    private ItemHolder itemHolder;
    private PatternAnchor currentPatternAnchor;
    private LightSwitchToggle currentLightSwitch;
    private FuseBoxPuzzle currentFuseBox;
    private CableConnectionPuzzle currentCable;
    private BookSlot currentBookSlot;
    private ChapterTransition currentChapterTransition;
    private Chapter4PuzzleChoice currentChapter4Choice;
    private Chapter3To4Transition currentCh3To4;
    private CeilingCollapseOnDoor currentCollapseOnDoor;
    private ArchivesDoor currentArchivesDoor;
    private MaintenanceRoomDoor currentMaintenanceDoor;
    private BreakerSwitch currentBreaker;

    void Start()
    {
        itemHolder = GetComponent<ItemHolder>();
        SetCrosshairNormal();
    }

    void Update()
    {
        CheckForInteractable();

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (currentCollapseOnDoor != null)
            {
                DoorCutsceneManager cutscene = currentCollapseOnDoor
                    .GetComponent<DoorCutsceneManager>();
                if (cutscene != null && !cutscene.hasPlayed)
                    cutscene.TriggerCutscene();
                else
                    currentCollapseOnDoor.InteractAndCollapse();
            }
            else if (currentMaintenanceDoor != null)
                currentMaintenanceDoor.TryOpen();
            else if (currentArchivesDoor != null)
                currentArchivesDoor.TryOpen();
            else if (currentDoor != null)
                currentDoor.Interact();
            else if (currentTrapDoor != null)
                currentTrapDoor.Interact();
            else if (currentTeleporter != null)
                currentTeleporter.Interact();
            else if (currentObject != null)
                currentObject.Interact();
            else if (currentPatternAnchor != null)
                currentPatternAnchor.Interact();
            else if (currentLightSwitch != null)
                currentLightSwitch.Toggle();
            else if (currentFuseBox != null)
                currentFuseBox.Interact();
            else if (currentCable != null)
                currentCable.Interact();
            else if (currentBookSlot != null)
                currentBookSlot.Interact();
            else if (currentBreaker != null)
                currentBreaker.Interact();
            else if (currentChapterTransition != null)
                currentChapterTransition.Interact();
            else if (currentChapter4Choice != null)
                currentChapter4Choice.Interact();
            else if (currentCh3To4 != null)
                currentCh3To4.Interact();
            else if (currentHoldableItem != null)
                itemHolder.AddToInventory(currentHoldableItem);
        }
    }

    void CheckForInteractable()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            CeilingCollapseOnDoor collapseOnDoor = hit.collider
                .GetComponentInParent<CeilingCollapseOnDoor>();
            if (collapseOnDoor != null) { SetOnly(collapseOnDoor: collapseOnDoor); ShowInteractionPrompt(true); SetCrosshairInteract(); return; }

            MaintenanceRoomDoor maintenanceDoor = hit.collider
                .GetComponentInParent<MaintenanceRoomDoor>();
            if (maintenanceDoor != null) { SetOnly(maintenanceDoor: maintenanceDoor); ShowInteractionPrompt(true); SetCrosshairInteract(); return; }

            ArchivesDoor archivesDoor = hit.collider
                .GetComponentInParent<ArchivesDoor>();
            if (archivesDoor != null) { SetOnly(archivesDoor: archivesDoor); ShowInteractionPrompt(true); SetCrosshairInteract(); return; }

            InteractableDoor door = hit.collider
                .GetComponentInParent<InteractableDoor>();
            if (door != null && door.enabled) { SetOnly(door: door); ShowInteractionPrompt(true); SetCrosshairInteract(); return; }

            DoorWithTrap trapDoor = hit.collider
                .GetComponentInParent<DoorWithTrap>();
            if (trapDoor != null) { SetOnly(trapDoor: trapDoor); ShowInteractionPrompt(true); SetCrosshairInteract(); return; }

            Teleporter teleporter = hit.collider.GetComponent<Teleporter>();
            if (teleporter != null) { SetOnly(teleporter: teleporter); ShowInteractionPrompt(true); SetCrosshairInteract(); return; }

            InteractableObject obj = hit.collider.GetComponent<InteractableObject>();
            if (obj != null) { SetOnly(obj: obj); ShowInteractionPrompt(true); SetCrosshairInteract(); return; }

            HoldableItem holdable = hit.collider.GetComponent<HoldableItem>();
            if (holdable != null && !holdable.IsBeingHeld()) { SetOnly(holdable: holdable); ShowInteractionPrompt(true); SetCrosshairInteract(); return; }

            LightSwitchToggle lightSwitch = hit.collider
                .GetComponent<LightSwitchToggle>();
            if (lightSwitch != null) { SetOnly(lightSwitch: lightSwitch); ShowInteractionPrompt(true); SetCrosshairInteract(); return; }

            FuseBoxPuzzle fuseBox = hit.collider.GetComponent<FuseBoxPuzzle>();
            if (fuseBox != null) { SetOnly(fuseBox: fuseBox); ShowInteractionPrompt(true); SetCrosshairInteract(); return; }

            CableConnectionPuzzle cable = hit.collider
                .GetComponent<CableConnectionPuzzle>();
            if (cable != null) { SetOnly(cable: cable); ShowInteractionPrompt(true); SetCrosshairInteract(); return; }

            BookSlot bookSlot = hit.collider.GetComponent<BookSlot>();
            if (bookSlot != null) { SetOnly(bookSlot: bookSlot); ShowInteractionPrompt(true); SetCrosshairInteract(); return; }

            BreakerSwitch breaker = hit.collider.GetComponent<BreakerSwitch>();
            if (breaker != null) { SetOnly(breaker: breaker); ShowInteractionPrompt(true); SetCrosshairInteract(); return; }

            ChapterTransition chapterTransition = hit.collider
                .GetComponent<ChapterTransition>();
            if (chapterTransition != null) { SetOnly(chapterTransition: chapterTransition); ShowInteractionPrompt(true); SetCrosshairInteract(); return; }

            Chapter4PuzzleChoice ch4Choice = hit.collider
                .GetComponent<Chapter4PuzzleChoice>();
            if (ch4Choice != null) { SetOnly(ch4Choice: ch4Choice); ShowInteractionPrompt(true); SetCrosshairInteract(); return; }

            Chapter3To4Transition ch3to4 = hit.collider
                .GetComponent<Chapter3To4Transition>();
            if (ch3to4 != null) { SetOnly(ch3to4: ch3to4); ShowInteractionPrompt(true); SetCrosshairInteract(); return; }

            PatternAnchor pattern = hit.collider.GetComponent<PatternAnchor>();
            if (pattern != null) { SetOnly(pattern: pattern); ShowInteractionPrompt(true); SetCrosshairInteract(); return; }
        }

        ClearAll();
        ShowInteractionPrompt(false);
        SetCrosshairNormal();
    }

    void SetOnly(
        InteractableDoor door = null,
        DoorWithTrap trapDoor = null,
        Teleporter teleporter = null,
        InteractableObject obj = null,
        HoldableItem holdable = null,
        LightSwitchToggle lightSwitch = null,
        FuseBoxPuzzle fuseBox = null,
        CableConnectionPuzzle cable = null,
        BookSlot bookSlot = null,
        ChapterTransition chapterTransition = null,
        Chapter4PuzzleChoice ch4Choice = null,
        Chapter3To4Transition ch3to4 = null,
        CeilingCollapseOnDoor collapseOnDoor = null,
        ArchivesDoor archivesDoor = null,
        MaintenanceRoomDoor maintenanceDoor = null,
        PatternAnchor pattern = null,
        BreakerSwitch breaker = null)
    {
        currentDoor = door;
        currentTrapDoor = trapDoor;
        currentTeleporter = teleporter;
        currentObject = obj;
        currentHoldableItem = holdable;
        currentLightSwitch = lightSwitch;
        currentFuseBox = fuseBox;
        currentCable = cable;
        currentBookSlot = bookSlot;
        currentChapterTransition = chapterTransition;
        currentChapter4Choice = ch4Choice;
        currentCh3To4 = ch3to4;
        currentCollapseOnDoor = collapseOnDoor;
        currentArchivesDoor = archivesDoor;
        currentMaintenanceDoor = maintenanceDoor;
        currentPatternAnchor = pattern;
        currentBreaker = breaker;
    }

    void ClearAll() { SetOnly(); }

    void SetCrosshairNormal()
    {
        if (crosshairText == null) return;
        crosshairText.text = normalCrosshair;
        crosshairText.color = normalColor;
        crosshairText.fontSize = normalSize;
    }

    void SetCrosshairInteract()
    {
        if (crosshairText == null) return;
        crosshairText.text = interactCrosshair;
        crosshairText.color = interactColor;
        crosshairText.fontSize = interactSize;
    }

    void ShowInteractionPrompt(bool show)
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        if (interactionTooltip != null)
            interactionTooltip.gameObject.SetActive(show);
    }

    void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Gizmos.DrawRay(ray.origin, ray.direction * interactionDistance);
        }
    }
}