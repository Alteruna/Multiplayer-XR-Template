using System;
using Alteruna.Trinity;
using UnityEngine;

namespace Alteruna
{
    public class Grabbable : CommunicationBridgeUID
    {
        [HideInInspector]
        public CommunicationBridge Hand;
        public bool Locked;
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public bool Grab(CommunicationBridge hand)
        {
            if (Locked) return false;
            Hand = hand;
            Locked = true;
            if (_rb != null)
            {
                _rb.isKinematic = true;
            }

            transform.parent = hand.transform;
            return true;
        }
        
        public void Release()
        {
            if (!Locked) return;
            Hand = null;
            Locked = false;
            transform.parent = null;
            if (_rb != null)
            {
                _rb.isKinematic = false;
            }
        }
        
        public void Release(Vector3 pos, Vector3 rot, Vector3 vel)
        {
            if (!Locked) return;
            Hand = null;
            Locked = false;
            var t = transform;
            t.parent = null;
            t.position = pos;
            t.eulerAngles = rot;
            if (_rb != null)
            {
                _rb.isKinematic = false;
                _rb.velocity = vel;
            }
        }
        
        public override void Serialize(ITransportStreamWriter processor, byte level, bool forceSync = false)
        {
            
        }

        public override void Unserialize(ITransportStreamReader processor, byte level, uint length)
        {
            
        }
    }
}

