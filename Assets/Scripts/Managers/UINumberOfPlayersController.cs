using UnityEngine;

public class UINumberOfPlayersController : MonoBehaviour
{
    private const string NumberOfPlayers = "NumberOfPlayers";
    private const int MinimumNumberOfPlayers = 1;
    private const int MaximumNumberOfPlayers = 4;
    private int currentNumber = 1;
    private bool isGameOnline;

    private void OnEnable()
    {
        EventManager.Instance.SubscribeToOnLoadNumberOfPlayersFromStart(SetNumberOfPlayers);
        EventManager.Instance.SubscribeToOnNumberOfPlayersChosen(SaveNumberOfPlayers);
        EventManager.Instance.SubscribeToOnSetGameToOnlineAction(SetGameToOnline);
        EventManager.Instance.SubscribeToOnSetGameToOfflineAction(SetGameToOffline);
    }

    private void SetNumberOfPlayers(int number)
    {
        currentNumber = number;
        UpdateNumberText();
    }

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
        PlayerPrefs.SetInt(NumberOfPlayers, currentNumber);
        EventManager.Instance.OnNumberOfPlayersSaved();
        if (!isGameOnline)
            SplitScreenManager.Instance.Split(currentNumber);
    }

    // Update the UI text with the current number
    private void UpdateNumberText()
    {
        EventManager.Instance.OnNumberOfPlayersChanged(currentNumber);
    }

    public void SetGameToOnline()
    {
        isGameOnline = true;
    }

    public void SetGameToOffline()
    {
        isGameOnline = false;
    }

    private void OnDisable()
    {
        EventManager.Instance.UnsubscribeFromOnNumberOfPlayersChosen(SaveNumberOfPlayers);
        EventManager.Instance.UnsubscribeFromOnLoadNumberOfPlayersFromStart(SetNumberOfPlayers);
        EventManager.Instance.UnsubscribeToOnSetGameToOnlineAction(SetGameToOnline);
        EventManager.Instance.UnsubscribeToOnSetGameToOfflineAction(SetGameToOffline);
    }
}
