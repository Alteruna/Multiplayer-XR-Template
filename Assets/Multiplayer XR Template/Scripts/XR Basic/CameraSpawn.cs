using System;
using UnityEngine;

namespace Alteruna
{
    public class CameraSpawn : CommunicationBridge
    {
        private void Start()
        {
            // If it does not have an Avatar component, use it as standard camera position for when not in room.
            if (gameObject.GetComponentInParent<Avatar>() == null)
            {
                Multiplayer.OnRoomLeft.AddListener(arg0 => Possessed(true, null));
                if (!Multiplayer.InRoom) Possessed(true, null);
            }
        }

        public override void Possessed(bool isMe, User user)
        {
            if (!isMe) return;
            if (Camera.main == null)
            {
                Debug.LogError("Camera not found.");
                return;
            }
        
            Transform cameraTransform = Camera.main.transform;
            cameraTransform.parent = transform;
            cameraTransform.localPosition = Vector3.zero;
            cameraTransform.localRotation = Quaternion.identity;
        }

        public override void Unpossessed()
        {
            if (Camera.main != null && Camera.main.transform.parent == transform)
            {
                Camera.main.transform.parent = null;
            }
        }
    }
}
