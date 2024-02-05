namespace Alteruna
{
    public class AutoJoin : CommunicationBridge
    {
        void Start()
        {
            Multiplayer.JoinFirstAvailable();
        }
    }
}

