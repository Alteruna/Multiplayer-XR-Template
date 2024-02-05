using System.Collections;
using UnityEngine;

namespace Alteruna
{
	public class ControllerSync : AttributesSync
	{
		public GameObject ControllerPrefab;
		public GameObject[] ObjectsToRemoveWhenNotOwned;

		bool _sync = false;

		public override void Possessed(bool isMe, User user)
		{
			if (isMe)
			{
				_sync = true;
			}
			else
			{
				foreach (GameObject obj in ObjectsToRemoveWhenNotOwned)
				{
					if (obj != null)
					{
						Destroy(obj);
					}
				}
				ObjectsToRemoveWhenNotOwned = null;

				if (ControllerPrefab != null)
				{
					Instantiate(ControllerPrefab, transform);
				}
			}
		}

		private void OnDisable()
		{
			if (_sync)
			{
				InvokeRemoteMethod(0, UserId.All, false);
			}
		}

		private new void OnEnable()
		{
			base.OnEnable();

			if (_sync)
			{
				InvokeRemoteMethod(0, UserId.All, true);
			}
		}

		[SynchronizableMethod]
		private void SetActiveSync(bool active)
		{
			gameObject.SetActive(active);
		}
	}
}