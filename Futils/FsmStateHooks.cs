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

    private static Dictionary<FSMData, Action<PlayMakerFSM>> StateEnterData = new Dictionary<FSMData, Action<PlayMakerFSM>>();
    private static Dictionary<FSMData, Action<PlayMakerFSM>> StateExitData = new Dictionary<FSMData, Action<PlayMakerFSM>>();
    private static Dictionary<FSMData, Action<PlayMakerFSM, string>> StateEnteredFromTransitionData = new Dictionary<FSMData, Action<PlayMakerFSM, string>>();
    private static Dictionary<FSMData, Action<PlayMakerFSM, string>> StateExitedViaTransitionData = new Dictionary<FSMData, Action<PlayMakerFSM, string>>();

    public static void HookStateEntered(FSMData data, Action<PlayMakerFSM> onStateEnter)
    {
        if (!StateEnterData.ContainsKey(data))
        {
            StateEnterData.Add(data, onStateEnter);
        }
        else
        {
            StateEnterData[data] += onStateEnter;
        }
    }
    public static void HookStateExited(FSMData data, Action<PlayMakerFSM> onStateExit)
    {
        if (!StateExitData.ContainsKey(data))
        {
            StateExitData.Add(data, onStateExit);
        }
        else
        {
            StateExitData[data] += onStateExit;
        }
    }
    public static void UnHookStateEntered(FSMData data, Action<PlayMakerFSM> onStateEnter)
    {
        if (StateEnterData.ContainsKey(data))
        {
            StateEnterData[data] -= onStateEnter;
        }
    }
    public static void UnHookStateExited(FSMData data, Action<PlayMakerFSM> onStateExit)
    {
        if (StateExitData.ContainsKey(data))
        {
            StateExitData[data] -= onStateExit;
        }
    }
    
    public static void HookStateEnteredFromTransition(FSMData data, Action<PlayMakerFSM, string> onStateEnteredFromTransition)
    {
        if (!StateEnteredFromTransitionData.ContainsKey(data))
        {
            StateEnteredFromTransitionData.Add(data, onStateEnteredFromTransition);
        }
        else
        {
            StateEnteredFromTransitionData[data] += onStateEnteredFromTransition;
        }
    }
    public static void HookStateExitedViaTransition(FSMData data, Action<PlayMakerFSM, string> onStateExitViaTransition)
    {
        if (!StateExitedViaTransitionData.ContainsKey(data))
        {
            StateExitedViaTransitionData.Add(data, onStateExitViaTransition);
        }
        else
        {
            StateExitedViaTransitionData[data] += onStateExitViaTransition;
        }
    }
    public static void UnHookStateEnteredFromTransitionedFromTransition(FSMData data, Action<PlayMakerFSM, string> onStateEnteredFromTransition)
    {
        if (StateEnteredFromTransitionData.ContainsKey(data))
        {
            StateEnteredFromTransitionData[data] -= onStateEnteredFromTransition;
        }
    }
    public static void UnHookStateExitedViaTransition(FSMData data, Action<PlayMakerFSM, string> onStateExitViaTransition)
    {
        if (StateExitedViaTransitionData.ContainsKey(data))
        {
            StateExitedViaTransitionData[data] -= onStateExitViaTransition;
        }
    }

    private static void EnterState(Action<Fsm, FsmState> orig, Fsm self, FsmState state)
    {
        string sceneName = self.GameObject.scene.name;
        string gameObject = self.GameObjectName;
        string fsmName = self.Name;
        string stateName = state.Name;

        if (StateEnterData.TryGetValue(new FSMData(sceneName, gameObject, fsmName, stateName), out var onStateEnter_1))
        {
            onStateEnter_1.TryInvoke(self.FsmComponent);
        }
        if (StateEnterData.TryGetValue(new FSMData(gameObject, fsmName, stateName), out var onStateEnter_2))
        {
            onStateEnter_2.TryInvoke(self.FsmComponent);
        }
        if (StateEnterData.TryGetValue(new FSMData(fsmName, stateName), out var onStateEnter_3))
        {
            onStateEnter_3.TryInvoke(self.FsmComponent);
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


        if (StateExitedViaTransitionData.TryGetValue(new FSMData(sceneName, gameObject, fsmName, self.ActiveStateName), out var onStateExitedViaTransition_1))
        {
            onStateExitedViaTransition_1.TryInvoke(self.FsmComponent, transition.EventName);
        }
        if (StateExitedViaTransitionData.TryGetValue(new FSMData(gameObject, fsmName, self.ActiveStateName), out var onStateExitedViaTransition_2))
        {
            onStateExitedViaTransition_2.TryInvoke(self.FsmComponent, transition.EventName);
        }
        if (StateExitedViaTransitionData.TryGetValue(new FSMData(fsmName, self.ActiveStateName), out var onStateExitedViaTransition_3))
        {
            onStateExitedViaTransition_3.TryInvoke(self.FsmComponent, transition.EventName);
        }
        
        
        if (StateEnteredFromTransitionData.TryGetValue(new FSMData(sceneName, gameObject, fsmName, transition.ToState), out var onStateEnteredFromTransition_1))
        {
            onStateEnteredFromTransition_1.TryInvoke(self.FsmComponent, transition.EventName);
        }
        if (StateEnteredFromTransitionData.TryGetValue(new FSMData(gameObject, fsmName, transition.ToState), out var onStateEnteredFromTransition_2))
        {
            onStateEnteredFromTransition_2.TryInvoke(self.FsmComponent, transition.EventName);
        }
        if (StateEnteredFromTransitionData.TryGetValue(new FSMData(fsmName, transition.ToState), out var onStateEnteredFromTransition_3))
        {
            onStateEnteredFromTransition_3.TryInvoke(self.FsmComponent, transition.EventName);
        }
        
        
        return orig(self, transition, isGlobal);
    }
    
    private static void ExitState(Action<Fsm, FsmState> orig, Fsm self, FsmState state)
    {
        string sceneName = self.GameObject.scene.name;
        string gameObject = self.GameObjectName;
        string fsmName = self.Name;
        string stateName = state.Name;

        if (StateExitData.TryGetValue(new FSMData(sceneName, gameObject, fsmName, stateName), out var onStateExit_1))
        {
            onStateExit_1.TryInvoke(self.FsmComponent);
        }
        if (StateExitData.TryGetValue(new FSMData(gameObject, fsmName, stateName), out var onStateExit_2))
        {
            onStateExit_2.TryInvoke(self.FsmComponent);
        }
        if (StateExitData.TryGetValue(new FSMData(fsmName, stateName), out var onStateExit_3))
        {
            onStateExit_3.TryInvoke(self.FsmComponent);
        }
        
        orig(self, state);
    }

    private static void TryInvoke<T>(this Action<T> action, T param)
    {
        if (action != null)
        {
            foreach (Action<T> toInvoke in action.GetInvocationList())
            {
                try
                {
                    toInvoke?.Invoke(param);
                }
                catch (Exception e)
                {
                    Satchel.Instance.LogError(e);
                }
            }
        }
    }
    
    private static void TryInvoke<T, V>(this Action<T, V> action, T param1, V param2)
    {
        if (action != null)
        {
            foreach (Action<T, V> toInvoke in action.GetInvocationList())
            {
                try
                {
                    toInvoke?.Invoke(param1, param2);
                }
                catch (Exception e)
                {
                    Satchel.Instance.LogError(e);
                }
            }
        }
    }
}