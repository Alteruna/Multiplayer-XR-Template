using Alteruna.Multiplayer;
using Alteruna.Multiplayer.Core;
using UnityEngine;

namespace Alteruna
{
	public class ControllerSync : AttributesSync
	{
		public GameObject ControllerPrefab;
		public GameObject[] ObjectsToRemoveWhenNotOwned;

		private bool _sync;

		private new void OnEnable()
		{
			base.OnEnable();

			if (_sync) InvokeRemoteMethod(0, UserId.All, true);
		}

		private void OnDisable()
		{
			if (_sync) InvokeRemoteMethod(0, UserId.All, false);
		}

		public override void Possessed(bool isMe, User user)
		{
			if (isMe)
			{
				_sync = true;
			}
			else
			{
				foreach (var obj in ObjectsToRemoveWhenNotOwned)
					if (obj != null)
						Destroy(obj);

				ObjectsToRemoveWhenNotOwned = null;

				if (ControllerPrefab != null) Instantiate(ControllerPrefab, transform);
			}
		}

		[SynchronizableMethod]
		private void SetActiveSync(bool active)
		{
			gameObject.SetActive(active);
		}
	}
}