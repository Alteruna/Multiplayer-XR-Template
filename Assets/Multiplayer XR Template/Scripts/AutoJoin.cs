using Alteruna.Multiplayer.Unity;

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