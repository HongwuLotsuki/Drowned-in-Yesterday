using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class BellPuzzleManager : MonoBehaviour
{
    [Header("Puzzle UI")]
    public GameObject puzzlePanel;
    public Button closeButton;

    [Header("Bell Buttons")]
    public Button bell6Button;
    public Button bell3Button;
    public Button bell5Button;
    public Button bell4Button;

    [Header("Bell Button Images")]
    public Image bell6Image;
    public Image bell3Image;
    public Image bell5Image;
    public Image bell4Image;

    [Header("Bell Colors")]
    public Color normalBellColor = Color.white;
    public Color correctBellColor = new Color(1f, 0.85f, 0.2f, 1f);
    public Color wrongFlashColor = Color.red;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip bell6Sound;
    public AudioClip bell3Sound;
    public AudioClip bell5Sound;
    public AudioClip bell4Sound;
    public AudioClip wrongSound;
    public AudioClip successSound;

    [Header("Events")]
    public UnityEvent onPuzzleSolved;

    private int[] correctOrder = { 3, 4, 5, 6 };
    private int currentIndex = 0;
    private bool isSolved = false;
    private bool isOpen = false;

private void Start()
    {
        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);
        }

        // Auto-fix Image references after hierarchy restructure (Image is on BellVisual child)
        if (bell6Image == null && bell6Button != null) bell6Image = bell6Button.transform.Find("BellVisual").GetComponent<Image>();
        if (bell3Image == null && bell3Button != null) bell3Image = bell3Button.transform.Find("BellVisual").GetComponent<Image>();
        if (bell5Image == null && bell5Button != null) bell5Image = bell5Button.transform.Find("BellVisual").GetComponent<Image>();
        if (bell4Image == null && bell4Button != null) bell4Image = bell4Button.transform.Find("BellVisual").GetComponent<Image>();

        bell6Button.onClick.AddListener(() => ClickBell(6));
        bell3Button.onClick.AddListener(() => ClickBell(3));
        bell5Button.onClick.AddListener(() => ClickBell(5));
        bell4Button.onClick.AddListener(() => ClickBell(4));

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePuzzle);
        }

        ResetAllBellColors();
    }

    private void Update()
    {
        if (!isOpen || isSolved)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePuzzle();
        }
    }

public void OpenPuzzle()
    {
        if (isSolved)
            return;

        isOpen = true;
        currentIndex = 0;
        PlayerController.InputLocked = true;

        // Hide the interaction prompt text
        HidePromptText();

        ResetAllBellColors();

        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(true);
        }
    }

    public void ClosePuzzle()
    {
        isOpen = false;
        PlayerController.InputLocked = false;

        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);
        }
    }

private void ClickBell(int sides)
    {
        if (isSolved)
            return;

        // Play the bell ringing animation
        PlayBellAnimation(sides);
        PlayBellSound(sides);

        if (sides == correctOrder[currentIndex])
        {
            SetBellImageColor(sides, correctBellColor);
            currentIndex++;

            if (currentIndex >= correctOrder.Length)
            {
                SolvePuzzle();
            }
        }
        else
        {
            FlashBellImage(sides);
            ResetPuzzle();
        }
    }

    private void PlayBellSound(int sides)
    {
        if (audioSource == null)
            return;

        AudioClip clip = sides switch
        {
            3 => bell3Sound,
            4 => bell4Sound,
            5 => bell5Sound,
            6 => bell6Sound,
            _ => null
        };

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void ResetPuzzle()
    {
        currentIndex = 0;
        ResetAllBellColors();

        if (audioSource != null && wrongSound != null)
        {
            audioSource.PlayOneShot(wrongSound);
        }
    }

    private void SolvePuzzle()
    {
        isSolved = true;
        isOpen = false;
        PlayerController.InputLocked = false;

        if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }

        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);
        }

        onPuzzleSolved?.Invoke();
    }

    private void SetBellImageColor(int sides, Color color)
    {
        Image img = GetBellImage(sides);
        if (img != null)
        {
            img.color = color;
        }
    }

    private void FlashBellImage(int sides)
    {
        Image img = GetBellImage(sides);
        if (img != null)
        {
            img.color = wrongFlashColor;
            // Restore all colors after 0.5 seconds
            Invoke(nameof(ResetAllBellColors), 0.5f);
        }
    }

    private Image GetBellImage(int sides)
    {
        return sides switch
        {
            3 => bell3Image,
            4 => bell4Image,
            5 => bell5Image,
            6 => bell6Image,
            _ => null
        };
    }

private Animator GetBellAnimator(int sides)
    {
        Button btn = sides switch
        {
            3 => bell3Button,
            4 => bell4Button,
            5 => bell5Button,
            6 => bell6Button,
            _ => null
        };
        if (btn == null) return null;
        var visual = btn.transform.Find("BellVisual");
        return visual != null ? visual.GetComponent<Animator>() : null;
    }

    private void PlayBellAnimation(int sides)
    {
        Animator anim = GetBellAnimator(sides);
        if (anim != null)
        {
            anim.SetTrigger("Ring");
        }
    }


    private void ResetAllBellColors()
    {
        if (bell3Image != null) bell3Image.color = normalBellColor;
        if (bell4Image != null) bell4Image.color = normalBellColor;
        if (bell5Image != null) bell5Image.color = normalBellColor;
        if (bell6Image != null) bell6Image.color = normalBellColor;
    }


private void HidePromptText()
    {
        // Find the BellPromptText in Canvas (shown by InteractableTrigger)
        var canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            var prompt = canvas.transform.Find("BellPromptText");
            if (prompt != null)
            {
                prompt.gameObject.SetActive(false);
            }
        }
    }

/// <summary>
    /// Called by ClueManager to set the shuffled correct order.
    /// </summary>
    public void SetCorrectOrder(List<int> newOrder)
    {
        if (newOrder != null && newOrder.Count > 0)
        {
            correctOrder = newOrder.ToArray();
        }
    }

}
