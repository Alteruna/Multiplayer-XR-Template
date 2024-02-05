using UnityEngine;
using UnityEngine.XR.Management;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.Management;
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
            {
                if (loader.name.Equals("Oculus Loader"))
                {
                    SetTransformOffset(1);
                    Multiplayer.Sync(this);
                    return;
                }
            }
            
            SetTransformOffset(0);
            Multiplayer.Sync(this);
        }


        public override void AssembleData(Writer writer, byte LOD = 100)
        {
            writer.Write(_id);
        }

        public override void DisassembleData(Reader reader, byte LOD = 100)
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
