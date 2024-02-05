using UnityEngine;

namespace Alteruna
{
	[RequireComponent(typeof(Canvas))]
	public class CanvasCameraFix : CommunicationBridge
	{
		
		void Start()
		{
			Multiplayer.OnRoomJoined.AddListener(RoomJoined);
		}

		private void RoomJoined(Multiplayer arg0, Room arg1, User arg2)
		{
			GetComponent<Canvas>().worldCamera = Camera.main;
		}
	}
}
