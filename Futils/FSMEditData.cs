using JetBrains.Annotations;
namespace Satchel.Futils;

public struct FSMData
{
    [CanBeNull] public string SceneName;
    [CanBeNull] public string GameObjectName;
    public string FsmName;
    public string StateName;

    public FSMData(string sceneName, string gameObjectName, string fsmName, string stateName)
    {
        SceneName = sceneName;
        GameObjectName = gameObjectName;
        FsmName = fsmName;
        StateName = stateName;
    }
    public FSMData(string gameObjectName, string fsmName, string stateName)
    {
        SceneName = null;
        GameObjectName = gameObjectName;
        FsmName = fsmName;
        StateName = stateName;
    }
    public FSMData(string fsmName, string stateName)
    {
        SceneName = null;
        GameObjectName = null;
        FsmName = fsmName;
        StateName = stateName;
    }

    public bool Match(string sceneName, string gameObjectName, string fsmName, string stateName)
    {
        if (SceneName == null && GameObjectName == null)
        {
            return fsmName == FsmName && stateName == StateName;
        }
        else if (SceneName == null)
        {
            return fsmName == FsmName && stateName == StateName && gameObjectName == GameObjectName;
        }
        else
        {
            return fsmName == FsmName && stateName == StateName && gameObjectName == GameObjectName && sceneName == SceneName;
        }
    }
}