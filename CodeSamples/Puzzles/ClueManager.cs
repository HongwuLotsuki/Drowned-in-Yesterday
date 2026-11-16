using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ClueManager : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("Clue numbers; shuffled at runtime")]
    public List<int> clueNumber;
    [Tooltip("All clue sprites (a clueNumber value is an index into this list)")]
    public List<Sprite> spriteImage;

    [Header("UI")]
    public GameObject cluePrefab;
    public Transform clueContainer;

    [Header("Sync")]
    [Tooltip("After shuffling, the new order is synced to BellPuzzleManager")]
    public BellPuzzleManager bellPuzzleManager;

    async void Start()
    {
        await ShuffleAndGenerate();
    }

    /// <summary>
    /// Shuffle asynchronously, build the UI, then sync the bell puzzle order.
    /// GameManager also calls this when the game restarts.
    /// </summary>
    public async Task ShuffleAndGenerate()
    {
        // 1. Clear the old UI
        if (clueContainer != null)
        {
            for (int i = clueContainer.childCount - 1; i >= 0; i--)
                Destroy(clueContainer.GetChild(i).gameObject);
        }

        // Wait one frame so Destroy completes
        await Task.Yield();

        // 2. Shuffle
        Shuffle(clueNumber);

        // 3. Sync to the bell puzzle
        if (bellPuzzleManager != null)
            bellPuzzleManager.SetCorrectOrder(clueNumber);

        // 4. Build the UI
        if (cluePrefab != null && clueContainer != null && spriteImage != null)
        {
            for (int i = 0; i < clueNumber.Count; i++)
            {
                int idx = clueNumber[i];
                if (idx < 0 || idx >= spriteImage.Count) continue;

                GameObject btnObj = Instantiate(cluePrefab, clueContainer);
                var img = btnObj.GetComponent<Image>();
                if (img != null)
                    img.sprite = spriteImage[idx];
            }
        }
    }

    private void Shuffle(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
