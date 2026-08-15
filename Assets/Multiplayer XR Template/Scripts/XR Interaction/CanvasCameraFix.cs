using Alteruna.Multiplayer.Unity;
using Alteruna.Multiplayer.Unity.EventArgument;
using UnityEngine;

namespace Alteruna
{
	[RequireComponent(typeof(Canvas))]
	public class CanvasCameraFix : CommunicationBridge
	{
		private void Start()
		{
			Multiplayer.OnRoomJoined.AddListener(RoomJoined);
		}

		private void RoomJoined(RoomJoinedEvent _)
		{
			GetComponent<Canvas>().worldCamera = Camera.main;
		}
	}
}