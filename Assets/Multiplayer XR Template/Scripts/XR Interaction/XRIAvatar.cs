using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Alteruna
{
	public class XRIAvatar : CommunicationBridge
	{
		private const string XRI_NAMESPACE = "UnityEngine.XR.Interaction.Toolkit";
		private const string XR_CORE_NAMESPACE = "Unity.XR.CoreUtils";

		private static readonly Type[] _componentsToRemove = new Type[]
		{
			typeof(UnityEngine.InputSystem.XR.TrackedPoseDriver),
			typeof(CharacterController),
			typeof(Camera),
			typeof(AudioListener)
		};

		public override void Possessed(bool isMe, User user)
		{
			if (isMe)
			{
				StartCoroutine(CharacterControllerFix());
				return;
			}

			transform.GetChild(0).localPosition = Vector3.zero;
			Destroy(transform.GetChild(1).gameObject);
			
			RemoveComponents();
			StartCoroutine(DestroyEmptyChildrenNextFrame());
		}
		
		private void RemoveComponents()
		{

			var components = GetComponentsInChildren<Behaviour>(true);
			List<Behaviour> toDestroy = new List<Behaviour>();

			foreach (Behaviour component in components)
			{
				var type = component.GetType();
				
				// ignore Transform
				if (type == typeof(Transform))
				{
					continue;
				}

				// Destroy Vignette Controller objects
				if (type == typeof(TunnelingVignetteController))
				{
					Destroy(component.gameObject);
					continue;
				}

				// have to destroy Event Systems one last.
				if (type == typeof(UnityEngine.EventSystems.EventSystem))
				{
					toDestroy.Add(component);
					continue;
				}

				// if matched any from _componentsToRemove, destroy it.
				if (ComplementMatch(component, _componentsToRemove))
				{
					Destroy(component);
					continue;
				}

				string name = type.Namespace;
				if (name.Length < XRI_NAMESPACE.Length)
				{
					if (name == XR_CORE_NAMESPACE)
					{
						Destroy(component);
					}
				}
				else
				{
					name = name.Substring(0, XRI_NAMESPACE.Length);
					if (name == XRI_NAMESPACE)
					{
						Destroy(component);
					}
				}
			}

			// destroy remaining components
			foreach (Component compoment in toDestroy)
			{
				Destroy(compoment);
			}
		}

		private static bool ComplementMatch(Component component, Type[] match) =>
			match.Any(type => component.GetType() == type);

		IEnumerator DestroyEmptyChildrenNextFrame()
		{
			List<Transform> toDestroy = new List<Transform>();
			DestroyEmptyChildren(transform, ref toDestroy);
			yield return null;
			foreach (Transform obj in toDestroy)
			{
				if (obj != null)
					Destroy(obj.gameObject);
			}
			yield return null;
			toDestroy.Clear();
			DestroyEmptyChildren(transform, ref toDestroy, false);
			foreach (Transform obj in toDestroy)
			{
				if (obj != null)
					Destroy(obj.gameObject);
			}
		}

		IEnumerator CharacterControllerFix()
		{
			// Wait until next frame
			yield return null;
			var cc = GetComponent<CharacterController>();
			cc.center = new Vector3(0, cc.center.y, 0);
		}

		private bool DestroyEmptyChildren(Transform transform, ref List<Transform> toDestroy, bool enableObjs = true)
		{
			bool hasChildren = false;
			foreach (Transform child in transform)
			{
				if (!DestroyEmptyChildren(child, ref toDestroy))
				{
					hasChildren = true;
				}
			}

			if (!hasChildren && transform.GetComponents<Component>().Length == 1)
			{
				toDestroy.Add(transform);
				return true;
			}

			if (enableObjs)
			{
				transform.gameObject.SetActive(true);
			}
			return false;
		}
	}
}