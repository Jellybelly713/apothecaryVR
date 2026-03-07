using System.Collections;
using TMPro;
using UnityEngine;

public class FeedbackUI : MonoBehaviour
{
    [SerializeField] private TMP_Text feedbackText;

    // How long each message should stay visible
    [SerializeField] private float showTime = 2f;

    private Coroutine currentRoutine;

    public void ShowMessage(string message)
    {
        if (feedbackText == null) return;

        // Stop message timer if one is already running
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        // Start showing the new message
        currentRoutine = StartCoroutine(ShowRoutine(message));
    }

    private IEnumerator ShowRoutine(string message)
    {
        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);

        // Keep text visible for the set amount of time
        yield return new WaitForSeconds(showTime);

        feedbackText.gameObject.SetActive(false);
    }
}