using UnityEngine;
using Futulabs.HoloFramework.Targeting;
using UnityEngine.VR.WSA.Input;

public class ActivateGravityInteractable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private Rigidbody           _rigidbody;

    private GestureRecognizer   _switchGravityRecognizer;

    private void Awake()
    {
        _switchGravityRecognizer = new GestureRecognizer();
        _switchGravityRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        _switchGravityRecognizer.TappedEvent += SwitchGravity;
    }

    public void GainFocus()
    {
        _switchGravityRecognizer.StartCapturingGestures();
    }

    public void LoseFocus()
    {
        _switchGravityRecognizer.StopCapturingGestures();
    }

    private void SwitchGravity(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        _rigidbody.useGravity = !_rigidbody.useGravity;
        _rigidbody.constraints = _rigidbody.useGravity ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeAll;
    }
}
