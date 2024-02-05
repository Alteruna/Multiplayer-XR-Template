using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Alteruna
{
    public class Hand : AttributesSync
    {
        
        //smooth velocity
        private const float VELOCITY_SMOOTHING = 0.4f;
        private const float MAX_GRAB_DISTANCE = 0.4f;
        
        [Header("Input")]
        public InputActionReference PickupAction;

        public float ThrowForceMultiplier = 1000;
        
        // buffer of all objects inside trigger
        private readonly List<Grabbable> _objectsInTrigger = new List<Grabbable>();
        private Grabbable _grabbedObject;
        private bool _isGrabbing;
        
        private Vector3 _velocity = new Vector3();
        private Vector3 oldPos;

        public override void Possessed(bool isMe, User user)
        {
            if (!isMe)
            {
                enabled = false;
                return;
            }
            
            // setup inputs
            PickupAction.action.performed += GrabAndSync;
            PickupAction.action.canceled += ReleaseAndSync;
            
            PickupAction.action.Enable();

        }

        private void Start()
        {
            oldPos = transform.position;
        }

        private void Update()
        {
            // calculate a smoothed out velocity.
            var pos = transform.position;
            _velocity = _velocity * VELOCITY_SMOOTHING + (pos - oldPos) * (1f-VELOCITY_SMOOTHING) * ThrowForceMultiplier * Time.deltaTime;
            oldPos = pos;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Add object as a possible grabbable object.
            if (other.TryGetComponent(out Grabbable grable))
            {
                _objectsInTrigger.Add(grable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // remove object as a possible grabbable object.
            if (other.TryGetComponent(out Grabbable grable))
            {
                _objectsInTrigger.Remove(grable);
            }
        }

        private void GrabAndSync(InputAction.CallbackContext callbackContext)
        {
            if (_isGrabbing) return;
            
            //find closest object
            float minDistance = MAX_GRAB_DISTANCE;
            Grabbable closestObject = null;
            foreach (Grabbable grable in _objectsInTrigger)
            {
                float distance = Vector3.Distance(grable.transform.position, transform.position);
                if (distance < minDistance && grable.Locked == false)
                {
                    minDistance = distance;
                    closestObject = grable;
                }
            }
            // if no object found, exit
            if (closestObject == null) return;
            
            // locally grab it
            Grab(closestObject);
            // grab it for other clients by UID
            InvokeRemoteMethod(nameof(GrabSynced), UserId.All, _grabbedObject.UID);
        }

        [SynchronizableMethod]
        private void GrabSynced(Guid uid)
        {
            // Find object by UID
            if (Multiplayer.GetGameObjectById(uid).TryGetComponent(out Grabbable grable))
            {
                Grab(grable);
            }
            else
            {
                Debug.LogError("Object with id " + uid + " not found.");
            }
        }
        
        [SynchronizableMethod]
        private void ReleaseSynced(Vector3 pos, Vector3 rot, Vector3 vel)
        {
            Release(pos, rot, vel);
        }

        private void Grab(Grabbable obj)
        {
            if (_isGrabbing)
            {
                Release();
            }
            
            if (!obj.Grab(this)) return;
            
            
            _isGrabbing = true;
            _grabbedObject = obj;
        }

        private void ReleaseAndSync(InputAction.CallbackContext callbackContext)
        {
            if (!_isGrabbing) return;
            var objTransform = _grabbedObject.transform;
            // invoke on locally and on other clients.
            // We define position, rotation, and velocity so that it will be the consistent across all players.
            BroadcastRemoteMethod(nameof(ReleaseSynced), objTransform.position, objTransform.eulerAngles, _velocity);
        }
        
        private void Release(Vector3 pos, Vector3 rot, Vector3 vel)
        {
            if (!_isGrabbing) return;
            _isGrabbing = false;
            _grabbedObject.Release(pos, rot, vel);
        }
        
        private void Release()
        {
            if (!_isGrabbing) return;
            _isGrabbing = false;
            _grabbedObject.Release();
        }
    }
}

