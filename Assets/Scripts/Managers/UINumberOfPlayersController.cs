using UnityEngine;

public class UINumberOfPlayersController : MonoBehaviour
{
    private const int MinimumNumberOfPlayers = 1;
    private const int MaximumNumberOfPlayers = 4;
    private int currentNumber = 1;


    // Called when the add button is pressed
    public void OnAddButtonPressed()
    {
        if (currentNumber >= MaximumNumberOfPlayers)
            currentNumber = MaximumNumberOfPlayers;
        else
            currentNumber++;
        UpdateNumberText();
    }

    // Called when the subtract button is pressed
    public void OnSubtractButtonPressed()
    {
        if (currentNumber <= MinimumNumberOfPlayers)
            currentNumber = MinimumNumberOfPlayers;
        else
            currentNumber--;
        UpdateNumberText();
    }

    // Called when the save button is pressed
    public void SaveNumberOfPlayers()
    {
        EventManager.Instance.OnNumberOfPlayersSaved();
        SplitScreenManager.Instance.Split(currentNumber);
    }

    // Update the UI text with the current number
    private void UpdateNumberText()
    {
        EventManager.Instance.OnNumberOfPlayersChanged(currentNumber);
    }
}
