using UnityEngine;
using Futulabs.HoloFramework.Targeting;
using UnityEngine.VR.WSA.Input;
using Zenject;

public class ScalableInteractable : MonoBehaviour, IInteractable
{
    [SerializeField]
    private float               _minScaleMultiplier;
    [SerializeField]
    private float               _maxScaleMultiplier;
    [SerializeField]
    private float               _scaleFactorPerEvent;

    private Vector3             _originalScale;
    private float               _currentScaleMultiplier;
    private float               _startScaleMultiplier;
    private float               _currentMaxScaleDelta;
    private float               _currentMinScaleDelta;

    private ITargetingManager   _targetingManager;

    private GestureRecognizer   _scaleObjectRecognizer;

    [Inject]
    public void Initialize(
        [Inject] ITargetingManager targetingManager)
    {
        _targetingManager = targetingManager;

        _scaleObjectRecognizer = new GestureRecognizer();
        _scaleObjectRecognizer.SetRecognizableGestures(GestureSettings.NavigationX);
        _scaleObjectRecognizer.NavigationStartedEvent += ScaleObjectStarted;
        _scaleObjectRecognizer.NavigationUpdatedEvent += ScaleObjectUpdated;
        _scaleObjectRecognizer.NavigationCompletedEvent += ScaleObjectCompleted;
        _scaleObjectRecognizer.NavigationCanceledEvent += ScaleObjectCanceled;

        _originalScale = transform.localScale;
        _currentScaleMultiplier = 1.0f;
    }

    public void GainFocus()
    {
        _scaleObjectRecognizer.StartCapturingGestures();
    }

    public void LoseFocus()
    {
        _scaleObjectRecognizer.StopCapturingGestures();
    }

    private void ScaleObjectStarted(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
    {
        // Turn off targeting updates while we're doing the scaling;
        _targetingManager.Updating = false;
        
        // Calculate the deltas between for max and min scale
        _startScaleMultiplier = _currentScaleMultiplier;
        _currentMaxScaleDelta = _currentScaleMultiplier * _scaleFactorPerEvent - _currentScaleMultiplier;
        _currentMinScaleDelta = _currentScaleMultiplier - _currentScaleMultiplier / _scaleFactorPerEvent;

        // Only use the x-component to determine scaling
        CalculateScaleMultiplier(normalizedOffset.x);

        transform.localScale = _originalScale * _currentScaleMultiplier;
    }

    private void ScaleObjectUpdated(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
    {
        // Only use the x-component to determine scaling
        CalculateScaleMultiplier(normalizedOffset.x);

        transform.localScale = _originalScale * _currentScaleMultiplier;
    }

    private void ScaleObjectCompleted(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
    {
        // Only use the x-component to determine scaling
        CalculateScaleMultiplier(normalizedOffset.x);

        transform.localScale = _originalScale * _currentScaleMultiplier;

        _targetingManager.Updating = true;
    }

    private void ScaleObjectCanceled(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
    {
        _targetingManager.Updating = true;
    }

    private void CalculateScaleMultiplier(float normalizedOffset)
    {
        // Use the offset's sign to determine if we're scaling down or up
        if (normalizedOffset > 0)
        {
            _currentScaleMultiplier = _startScaleMultiplier + normalizedOffset * _currentMaxScaleDelta;
        }
        else if (normalizedOffset < 0)
        {
            _currentScaleMultiplier = _startScaleMultiplier + normalizedOffset * _currentMinScaleDelta;
        }

        _currentScaleMultiplier = Mathf.Clamp(_currentScaleMultiplier, _minScaleMultiplier, _maxScaleMultiplier);
    }
}