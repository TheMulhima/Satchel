using JetBrains.Annotations;
namespace Satchel.Futils;

public record FSMData(string SceneName, string GameObjectName, string FsmName, string StateName)
{
    public FSMData(string gameObjectName, string fsmName, string stateName) : this(null, gameObjectName, fsmName, stateName)
    {
    }
    public FSMData(string fsmName, string stateName) : this(null, null, fsmName, stateName)
    {
    }
}