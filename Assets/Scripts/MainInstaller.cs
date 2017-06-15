using Futulabs.HoloFramework;
using UniRx;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller<MainInstaller>
{
    [SerializeField]
    private float _stabPlaneDefaultDistance;

    public override void InstallBindings()
    {
        // Bind the main camera (the Hololens) as the default for anyone that needs a camera
        Container.Bind<Camera>().FromInstance(Camera.main);
        // Reactive properties for tracking the head's current position and view direction
        Container.BindInstance(new ReactiveProperty<Vector3>(Vector3.zero)).WithId("Head position");
        Container.BindInstance(new ReactiveProperty<Vector3>(Vector3.forward)).WithId("View direction");
        // Bind the head and gaze handler, which updates the reactive properties
        Container.Bind(typeof(IHeadTracker), typeof(IGazeTracker), typeof(ITickable)).To<HeadAndGazeHandler>().AsSingle().NonLazy();

        // Bind the raycaster implementation
        Container.Bind<IRaycaster>().To<Raycaster>().AsSingle().NonLazy();

        // Bind the stabilization plane manager
        Container.BindInstance(_stabPlaneDefaultDistance).WithId("Stab plane default distance");
        Container.Bind<IStabilizationPlaneManager>().To<StabilizationPlaneManager>().AsSingle().NonLazy();
    }
}