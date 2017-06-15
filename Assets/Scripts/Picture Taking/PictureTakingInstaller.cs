using UnityEngine.UI;
using UnityEngine;
using Zenject;
using Futulabs.HoloFramework.ImageCapture;

public class PictureTakingInstaller : MonoInstaller<PictureTakingInstaller>
{
    public Text     _infoText;

    public override void InstallBindings()
    {
        Container.BindInstance(_infoText).WithId("Info text");
        Container.Bind<ICameraManager>().To<CameraManager>().AsSingle().NonLazy();
    }
}