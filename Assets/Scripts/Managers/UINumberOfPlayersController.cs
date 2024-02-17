using UnityEngine;

public class UINumberOfPlayersController : MonoBehaviour
{
    private int currentNumber = 1;

    // Called when the add button is pressed
    public void OnAddButtonPressed()
    {

        if (currentNumber >= 4)
            currentNumber = 4;
        else
            currentNumber++;
        UpdateNumberText();
    }

    // Called when the subtract button is pressed
    public void OnSubtractButtonPressed()
    {
        if (currentNumber <= 1)
            currentNumber = 1;
        else
            currentNumber--;
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
