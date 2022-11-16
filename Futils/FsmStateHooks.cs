using System.Linq;
using System.Reflection;
using MonoMod.RuntimeDetour;

namespace Satchel.Futils;

public static class FsmStateHooks
{
    static FsmStateHooks()
    {
        _ = new Hook(typeof(Fsm).GetMethod("EnterState", BindingFlags.Instance | BindingFlags.NonPublic), EnterState);
        _ = new Hook(typeof(Fsm).GetMethod("DoTransition", BindingFlags.Instance | BindingFlags.NonPublic), DoTransition);
        _ = new Hook(typeof(Fsm).GetMethod("ExitState", BindingFlags.Instance | BindingFlags.NonPublic), ExitState);
    }

    private static Dictionary<FSMData, Action> StateEnterData = new Dictionary<FSMData, Action>();
    private static Dictionary<FSMData, Action> StateExitData = new Dictionary<FSMData, Action>();
    private static Dictionary<FSMData, Action<string>> StateEnteredFromTransitionData = new Dictionary<FSMData, Action<string>>();
    private static Dictionary<FSMData, Action<string>> StateExitedViaTransitionData = new Dictionary<FSMData, Action<string>>();

    public static void StateEntered(FSMData data, Action onStateEnter)
    {
        StateEnterData.Add(data, onStateEnter);
    }
    
    public static void StateEnteredFromTransition(FSMData data, Action<string> onStateEnterFromTransition)
    {
        StateEnteredFromTransitionData.Add(data, onStateEnterFromTransition);
    }
    
    public static void StateExitedViaTransition(FSMData data, Action<string> onStateExitViaTransition)
    {
        StateExitedViaTransitionData.Add(data, onStateExitViaTransition);
    }
    
    public static void StateExited(FSMData data, Action onStateExit)
    {
        StateExitData.Add(data, onStateExit);
    }

    private static void EnterState(Action<Fsm, FsmState> orig, Fsm self, FsmState state)
    {
        string sceneName = self.GameObject.scene.name;
        string gameObject = self.GameObjectName;
        string fsmName = self.Name;
        
        foreach (var (fsmEdit, act) in StateEnterData)
        {
            if (fsmEdit.Match(sceneName, gameObject, fsmName, state.Name))
            {
                try
                {
                    act();
                }
                catch (Exception e)
                {
                    Satchel.Instance.LogError(e);
                }
            }
        }
        
        orig(self, state);
    }
    
    private static bool DoTransition(Func<Fsm, FsmTransition, bool, bool> orig, Fsm self, FsmTransition transition, bool isGlobal)
    {
        // a check in the normal code
        if (transition.ToFsmState == null)
        {
            return orig(self, transition, isGlobal);
        }
        
        string sceneName = self.GameObject.scene.name;
        string gameObject = self.GameObjectName;
        string fsmName = self.Name;
        
        foreach (var (fsmEdit, act) in StateExitedViaTransitionData)
        {
            if (fsmEdit.Match(sceneName, gameObject, fsmName, self.ActiveStateName))
            {
                try
                {
                    act(transition.EventName);
                }
                catch (Exception e)
                {
                    Satchel.Instance.LogError(e);
                }
            }
        }

        foreach (var (fsmEdit, act) in StateEnteredFromTransitionData)
        {
            if (fsmEdit.Match(sceneName, gameObject, fsmName, transition.ToState))
            {
                try
                {
                    act(transition.EventName);
                }
                catch (Exception e)
                {
                    Satchel.Instance.LogError(e);
                }
            }
        }
        
        
        return orig(self, transition, isGlobal);
    }
    
    private static void ExitState(Action<Fsm, FsmState> orig, Fsm self, FsmState state)
    {
        string sceneName = self.GameObject.scene.name;
        string gameObject = self.GameObjectName;
        string fsmName = self.Name;
        string stateName = state.Name;
        
        foreach (var (fsmEdit, act) in StateExitData)
        {
            if (fsmEdit.Match(sceneName, gameObject, fsmName, stateName))
            {
                try
                {
                    act();
                }
                catch (Exception e)
                {
                    Satchel.Instance.LogError(e);
                }
            }
        }
        
        orig(self, state);
    }
}