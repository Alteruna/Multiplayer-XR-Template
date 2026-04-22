using Alteruna.Multiplayer;

namespace Alteruna
{
	public class AutoJoin : CommunicationBridge
	{
		private void Start()
		{
			Multiplayer.JoinFirstAvailable();
		}
	}
}