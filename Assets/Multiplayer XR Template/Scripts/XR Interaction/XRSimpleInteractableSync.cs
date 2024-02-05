using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Alteruna
{
    [RequireComponent(typeof(XRSimpleInteractable))]
    public class XRSimpleInteractableSync : AttributesSync
    {
        
        private XRSimpleInteractable _simpleInteractable;
        private int _lastActivationFrame = 0;
        private int _lastDeactivationFrame = 0;
        
        void Start()
        {
            _simpleInteractable = GetComponent<XRSimpleInteractable>();
            _simpleInteractable.selectEntered.AddListener(OnSelectEntered);
            _simpleInteractable.selectExited.AddListener(OnSelectExited);
        }

        private void OnSelectEntered(SelectEnterEventArgs arg0)
        {
            // only activate once per frame to avoid infinite loop
            if (Time.frameCount == _lastActivationFrame) return;
            _lastActivationFrame = Time.frameCount;
            InvokeRemoteMethod(nameof(SelectedSynced), UserId.All, true);
        }
        
        private void OnSelectExited(SelectExitEventArgs arg0)
        {
            // only activate once per frame to avoid infinite loop
            if (Time.frameCount == _lastDeactivationFrame) return;
            _lastDeactivationFrame = Time.frameCount;
            InvokeRemoteMethod(nameof(SelectedSynced), UserId.All, false);
        }
        
        private void SelectedSynced(bool selected)
        {
            if (selected)
            {
                var args = new SelectEnterEventArgs();
                args.manager = _simpleInteractable.interactionManager;
                _simpleInteractable.selectEntered.Invoke(args);
            }
            else
            {
                var args = new SelectExitEventArgs();
                args.manager = _simpleInteractable.interactionManager;
                _simpleInteractable.selectExited.Invoke(args);
            }
        }
    }
}

