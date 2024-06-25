using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(GlobalSelectors))]
public class GlobalSelectorsInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GlobalSelectors>().FromComponentInHierarchy().AsSingle();
    }
}
