using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Alteruna
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(XRGrabInteractable), typeof(RigidbodySynchronizable))]
	public class XRGrabInteractableSync : MonoBehaviour
	{
		private XRGrabInteractable _grabInteractable;
		private RigidbodySynchronizable _rigidbody;
		
		private void Start()
		{
			_grabInteractable = GetComponent<XRGrabInteractable>();
			_rigidbody = GetComponent<RigidbodySynchronizable>();
			
			_grabInteractable.selectEntered.AddListener(OnSelectEntered);
			_grabInteractable.selectExited.AddListener(OnSelectExited);
		}

		private void OnSelectExited(SelectExitEventArgs arg0)
		{
			_rigidbody.SyncSettings();
		}

		private void OnSelectEntered(SelectEnterEventArgs arg0)
		{
			_rigidbody.SyncSettings();
		}
	}
}