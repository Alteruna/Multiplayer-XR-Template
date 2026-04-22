using Alteruna.Multiplayer;
using Alteruna.Multiplayer.Core;
using Alteruna.Multiplayer.Core.MethodArguments;
using Alteruna.Multiplayer.Core.PacketProcessing.Reader;
using Alteruna.Multiplayer.Core.PacketProcessing.Writer;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.Management;
#else
using UnityEngine.XR.Management;
#endif

namespace Alteruna
{
	public class ControllerSetup : Synchronizable
	{
		public Transform[] Transforms;

		private ushort _id;

		public override void Possessed(bool isMe, User user)
		{
			if (!isMe) return;

#if UNITY_EDITOR
			var loaders = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone).Manager.activeLoaders;
#else
            var loaders = XRGeneralSettings.Instance.Manager.activeLoaders;
#endif

			foreach (var loader in loaders)
				if (loader.name.Equals("Oculus Loader"))
				{
					SetTransformOffset(1);
					Multiplayer.Sync(this);
					return;
				}

			SetTransformOffset(0);
			Multiplayer.Sync(this);
		}


		public override void AssembleData(Writer writer, SerializeInfo info)
		{
			writer.Write(_id);
		}

		public override void DisassembleData(Reader reader, UnserializeInfo info)
		{
			SetTransformOffset(reader.ReadUshort());
		}

		public void SetTransformOffset(ushort id)
		{
			_id = id;

			var t = transform;
			t.localPosition = Transforms[id].localPosition;
			t.localRotation = Transforms[id].localRotation;
		}
	}
}