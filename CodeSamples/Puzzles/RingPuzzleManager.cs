using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum PreSolvedRing { None, Outer, Middle, Inner }

public class RingPuzzleManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject puzzlePanel;
    public Button closeButton;
    public Button resetButton;

    [Header("Rings")]
    public RectTransform outerRing;
    public RectTransform middleRing;
    public RectTransform innerRing;

    [Header("Ring Images")]
    public Image outerRingImage;
    public Image middleRingImage;
    public Image innerRingImage;

    [Header("Rotation")]
    public float rotateStep = 45f;

    [Header("Start Angles")]
    public float outerStartAngle = 90f;
    public float middleStartAngle = 180f;
    public float innerStartAngle = 270f;

    [Header("Correct Angles")]
    public float outerCorrectAngle = 0f;
    public float middleCorrectAngle = 0f;
    public float innerCorrectAngle = 0f;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = new Color(1f, 0.8f, 0.2f, 1f);
    public Color correctColor = new Color(1f, 0.85f, 0.2f, 1f);

    [Header("Solved Event")]
    
    [Tooltip("A ring that starts at its correct angle and does not need to be rotated")]
    public PreSolvedRing preSolvedRing = PreSolvedRing.None;
public UnityEvent onPuzzleSolved;

    private RectTransform selectedRing;
    private Image selectedRingImage;

    private bool isOpen = false;
    private bool isSolved = false;

    private void Start()
    {
        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePuzzle);
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetPuzzle);
        }
    }

    private void Update()
    {
        if (!isOpen || isSolved)
            return;

        // Rotation input
        if (Input.GetKeyDown(KeyCode.A))
        {
            RotateSelectedRing(rotateStep);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            RotateSelectedRing(-rotateStep);
        }

        // Close
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePuzzle();
        }

        // Refresh ring color feedback every frame
        UpdateRingColors();
    }

public void OpenPuzzle()
    {
        if (isSolved)
            return;

        isOpen = true;
        PlayerController.InputLocked = true;

        if (puzzlePanel != null)
            puzzlePanel.SetActive(true);

        SetRingAngle(outerRing, outerStartAngle);
        SetRingAngle(middleRing, middleStartAngle);
        SetRingAngle(innerRing, innerStartAngle);

        ApplyPreSolvedRing();

        SelectOuterRing();
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

public void ResetPuzzle()
    {
        if (isSolved)
            return;

        SetRingAngle(outerRing, outerStartAngle);
        SetRingAngle(middleRing, middleStartAngle);
        SetRingAngle(innerRing, innerStartAngle);

        ApplyPreSolvedRing();

        SelectOuterRing();
    }

private void ApplyPreSolvedRing()
    {
        switch (preSolvedRing)
        {
            case PreSolvedRing.Outer:
                SetRingAngle(outerRing, outerCorrectAngle);
                break;
            case PreSolvedRing.Middle:
                SetRingAngle(middleRing, middleCorrectAngle);
                break;
            case PreSolvedRing.Inner:
                SetRingAngle(innerRing, innerCorrectAngle);
                break;
        }
    }


public void SelectOuterRing()
    {
        if (preSolvedRing == PreSolvedRing.Outer) return;
        SelectRing(outerRing, outerRingImage);
    }

public void SelectMiddleRing()
    {
        if (preSolvedRing == PreSolvedRing.Middle) return;
        SelectRing(middleRing, middleRingImage);
    }

public void SelectInnerRing()
    {
        if (preSolvedRing == PreSolvedRing.Inner) return;
        SelectRing(innerRing, innerRingImage);
    }

    private void SelectRing(RectTransform ring, Image image)
    {
        selectedRing = ring;
        selectedRingImage = image;
    }

    private void RotateSelectedRing(float amount)
    {
        if (selectedRing == null)
            return;

        float currentAngle = NormalizeAngle(selectedRing.localEulerAngles.z);
        float newAngle = NormalizeAngle(currentAngle + amount);
        SetRingAngle(selectedRing, newAngle);

        CheckSolved();
    }

    private void SetRingAngle(RectTransform ring, float angle)
    {
        if (ring == null)
            return;

        ring.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void UpdateRingColors()
    {
        // Color each ring by whether it is at its correct angle
        // The selected ring keeps selectedColor
        SetRingImageColor(outerRing, outerRingImage, outerCorrectAngle);
        SetRingImageColor(middleRing, middleRingImage, middleCorrectAngle);
        SetRingImageColor(innerRing, innerRingImage, innerCorrectAngle);

        // Make sure the selected ring always shows the selected color
        if (selectedRingImage != null)
        {
            selectedRingImage.color = selectedColor;
        }
    }

    private void SetRingImageColor(RectTransform ring, Image image, float correctAngle)
    {
        if (ring == null || image == null)
            return;

        // Skip the selected ring, it is colored afterwards
        if (ring == selectedRing)
            return;

        if (IsAngleCorrect(ring, correctAngle))
        {
            image.color = correctColor;
        }
        else
        {
            image.color = normalColor;
        }
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;

        if (angle < 0f)
        {
            angle += 360f;
        }

        return angle;
    }

    private bool IsAngleCorrect(RectTransform ring, float correctAngle)
    {
        float currentAngle = NormalizeAngle(ring.localEulerAngles.z);
        float targetAngle = NormalizeAngle(correctAngle);

        return Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) < 0.1f;
    }

private void CheckSolved()
    {
        bool outerCorrect = preSolvedRing == PreSolvedRing.Outer || IsAngleCorrect(outerRing, outerCorrectAngle);
        bool middleCorrect = preSolvedRing == PreSolvedRing.Middle || IsAngleCorrect(middleRing, middleCorrectAngle);
        bool innerCorrect = preSolvedRing == PreSolvedRing.Inner || IsAngleCorrect(innerRing, innerCorrectAngle);

        if (outerCorrect && middleCorrect && innerCorrect)
        {
            SolvePuzzle();
        }
    }

    private void SolvePuzzle()
    {
        isSolved = true;
        isOpen = false;
        PlayerController.InputLocked = false;

        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);
        }

        Debug.Log("Ring puzzle solved");

        onPuzzleSolved?.Invoke();
    }
}
