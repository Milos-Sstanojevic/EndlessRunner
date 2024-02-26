using Fusion;

public class CameraSplitController : NetworkBehaviour
{
    [Networked][Capacity(4)] public NetworkLinkedList<OneScreenController> screensInGame { get; }
    private OneScreenController oneScreen;

    public override void Spawned()
    {
        screensInGame.Add(oneScreen);
    }

    public void SetNewScreen(OneScreenController screen)
    {
        oneScreen = screen;
    }
}