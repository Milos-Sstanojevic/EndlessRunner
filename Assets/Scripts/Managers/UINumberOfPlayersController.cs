using UnityEngine;

public class UINumberOfPlayersController : MonoBehaviour
{
    private int currentNumber = 0;

    // Called when the add button is pressed
    public void OnAddButtonPressed()
    {
        currentNumber++;
        if (currentNumber >= 4)
            currentNumber = 4;
        UpdateNumberText();
    }

    // Called when the subtract button is pressed
    public void OnSubtractButtonPressed()
    {
        currentNumber--;
        if (currentNumber <= 1)
            currentNumber = 1;
        UpdateNumberText();
    }

    // Called when the save button is pressed
    public void SaveNumberOfPlayers()
    {
        EventManager.Instance.OnNumberOfPlayersSaved(currentNumber);
        SplitScreenManager.Instance.Split(currentNumber);
    }

    // Update the UI text with the current number
    private void UpdateNumberText()
    {
        EventManager.Instance.OnNumberOfPlayersChanged(currentNumber);
    }
}
