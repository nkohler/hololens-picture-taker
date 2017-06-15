using NUnit.Framework;
using Zenject;
using Futulabs.HoloFramework;
using UnityEngine;

public class RaycasterTest : ZenjectUnitTestFixture
{
    private GameObject target;
    private BoxCollider col;

    [SetUp]
    public void CommonInstall()
    {
        Container.BindInstance(new Raycaster.Settings());
        Container.Bind<Raycaster>().AsSingle();
        target = new GameObject("Target");
        col = target.AddComponent<BoxCollider>();
    }

	[Test]
	public void ObjectHit()
    {
        //Arrange
        Raycaster caster = Container.Resolve<Raycaster>();

        //Act
        RaycastResult result = caster.CastRay(-Vector3.forward * 2.0f, Vector3.forward);

        //Assert
        Assert.That(result.WasHit);
        Assert.That(result.HitObject == target);
    }

    [Test]
    public void ObjectMissed()
    {
        //Arrange
        Raycaster caster = Container.Resolve<Raycaster>();

        //Act
        RaycastResult result = caster.CastRay(Vector3.forward * 2.0f, Vector3.forward);

        //Assert
        Assert.That(!result.WasHit);
        Assert.That(result.HitObject == null);
    }
}
