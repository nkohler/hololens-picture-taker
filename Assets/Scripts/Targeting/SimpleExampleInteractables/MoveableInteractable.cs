using UnityEngine;
using Futulabs.HoloFramework.Targeting;
using UnityEngine.VR.WSA.Input;
using Zenject;

public class MoveableInteractable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private float               _speedAtOneUnit;
    private bool                _moving;
    private Vector3             _moveDir;
    private Vector3             _prevDelta;

    private ITargetingManager   _targetingManager;

    private GestureRecognizer   _moveObjectRecognizer;

    [Inject]
    public void Initialize(
        [Inject] ITargetingManager targetingManager)
    {
        _targetingManager = targetingManager;

        _moveObjectRecognizer = new GestureRecognizer();
        _moveObjectRecognizer.SetRecognizableGestures(GestureSettings.ManipulationTranslate);
        _moveObjectRecognizer.ManipulationStartedEvent += MoveObjectStarted;
        _moveObjectRecognizer.ManipulationUpdatedEvent += MoveObjectUpdated;
        _moveObjectRecognizer.ManipulationCompletedEvent += MoveObjectCompleted;
        _moveObjectRecognizer.ManipulationCanceledEvent += MoveObjectCanceled;
    }

    private void Update()
    {
        if (_moving)
        {
            float distanceFromHead = (Camera.main.transform.position - transform.position).magnitude;
            transform.position += _moveDir * (_speedAtOneUnit * distanceFromHead) * Time.deltaTime;
        }
    }

    public void GainFocus()
    {
        _moveObjectRecognizer.StartCapturingGestures();
    }

    public void LoseFocus()
    {
        _moveObjectRecognizer.StopCapturingGestures();
    }

    private void MoveObjectStarted(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
    {
        _targetingManager.Updating = false;
        _moving = true;

        _prevDelta = Vector3.zero;
        _moveDir = cumulativeDelta - _prevDelta;
        _prevDelta = cumulativeDelta;
    }

    private void MoveObjectUpdated(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
    {
        _moveDir = cumulativeDelta - _prevDelta;
        _prevDelta = cumulativeDelta;
    }

    private void MoveObjectCompleted(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
    {
        _moveDir = Vector3.zero;

        _moving = false;
        _targetingManager.Updating = true;
    }

    private void MoveObjectCanceled(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
    {
        _moveDir = Vector3.zero;

        _moving = false;
        _targetingManager.Updating = true;
    }
}
