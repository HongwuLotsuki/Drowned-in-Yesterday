using System.Collections;
using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [Header("Event Channel")]
    public DialogEventSO dialogEvent;

    [Header("Database")]
    public DialogDatabaseSO dialogDatabase;

    [Header("UI")]
    public CanvasGroup panelGroup;
    public TextMeshProUGUI dialogText;

    [Header("Animation")]
    public float fadeInDuration = 0.3f;
    public float fadeOutDuration = 0.3f;
    [Tooltip("Typewriter effect: characters per second, 0 shows everything at once")]
    public float typewriterSpeed = 0f;

    private Coroutine currentRoutine;

    private void OnEnable()
    {
        if (dialogEvent != null)
            dialogEvent.onTriggerDialog.AddListener(HandleDialogEvent);
    }

    private void OnDisable()
    {
        if (dialogEvent != null)
            dialogEvent.onTriggerDialog.RemoveListener(HandleDialogEvent);
    }

    private void Start()
    {
        if (panelGroup != null)
            panelGroup.alpha = 0f;
    }

    /// <summary>
    /// Handles a message from the event channel (an ID or plain text)
    /// </summary>
    private void HandleDialogEvent(string idOrText)
    {
        if (dialogDatabase != null)
        {
            var entry = dialogDatabase.GetEntry(idOrText);
            if (entry != null)
            {
                Show(entry.text, entry.duration);
                return;
            }
        }
        // Not found in the database, show it as plain text
        Show(idOrText, 3f);
    }

    /// <summary>
    /// Show text directly (called from code)
    /// </summary>
    public void Show(string text, float duration)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine(text, duration));
    }

    /// <summary>
    /// Show by ID (text is looked up in the database)
    /// </summary>
    public void ShowById(string id)
    {
        if (dialogDatabase != null)
        {
            var entry = dialogDatabase.GetEntry(id);
            if (entry != null)
            {
                Show(entry.text, entry.duration);
                return;
            }
        }
        Debug.LogWarning($"[DialogManager] Dialog ID not found: {id}");
    }

    /// <summary>
    /// Immediately clear the current dialog
    /// </summary>
    public void Clear()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }
        if (panelGroup != null)
            panelGroup.alpha = 0f;
    }

    private IEnumerator ShowRoutine(string text, float duration)
    {
        // If typewriter speed is 0, show the full text
        if (typewriterSpeed <= 0f)
        {
            if (dialogText != null)
                dialogText.text = text;
        }
        else
        {
            // Typewriter: reveal one character at a time
            if (dialogText != null)
            {
                dialogText.text = "";
                float charDelay = 1f / typewriterSpeed;
                foreach (char c in text)
                {
                    dialogText.text += c;
                    yield return new WaitForSeconds(charDelay);
                }
            }
        }

        // Fade in
        yield return StartCoroutine(FadeTo(1f, fadeInDuration));

        // Wait
        if (duration > 0f)
        {
            yield return new WaitForSeconds(duration);
            // Fade out
            yield return StartCoroutine(FadeTo(0f, fadeOutDuration));
        }
        // duration = 0 keeps it on screen until Clear() is called

        currentRoutine = null;
    }

    private IEnumerator FadeTo(float target, float dur)
    {
        if (panelGroup == null) yield break;

        float start = panelGroup.alpha;
        float elapsed = 0f;
        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            panelGroup.alpha = Mathf.Lerp(start, target, elapsed / dur);
            yield return null;
        }
        panelGroup.alpha = target;
    }
}
