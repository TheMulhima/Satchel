using System.Reflection;
using MonoMod.RuntimeDetour;

namespace Satchel.Futils;

/// <summary>
/// A class that allows you to hook entering and exiting states
/// </summary>
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

    /// <summary>
    /// Hook that gets called when a state is entered by any means (a transition, a global transition, or from Fsm.SetState)
    /// </summary>
    /// <param name="data">The data necessary to find the fsm to be edited</param>
    /// <param name="onStateEnter">The action that will be invoked when the state is entered, the parameter passed into the action is the fsm</param>
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
    /// <summary>
    /// Hook that gets called when a state is exited by any means (a transition, a global transition, or from Fsm.SetState)
    /// </summary>
    /// <param name="data">The data necessary to find the fsm to be edited</param>
    /// <param name="onStateExit">The action that will be invoked when the state is exited, the parameter passed into the action is the fsm</param>
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
    /// <summary>
    /// Unhook your action from the hook that gets called when a state is entered by any means (a transition, a global transition, or from Fsm.SetState)
    /// </summary>
    /// <param name="data">The data necessary to find the fsm to be edited</param>
    /// <param name="onStateEnter">The action that will be removed</param>
    public static void UnHookStateEntered(FSMData data, Action<PlayMakerFSM> onStateEnter)
    {
        if (StateEnterData.ContainsKey(data))
        {
            StateEnterData[data] -= onStateEnter;
        }
    }
    /// <summary>
    /// Unhook your action from the hook that gets called when a state is exited by any means (a transition, a global transition, or from Fsm.SetState)
    /// </summary>
    /// <param name="data">The data necessary to find the fsm to be edited</param>
    /// <param name="onStateExit">The action that will be removed</param>
    public static void UnHookStateExited(FSMData data, Action<PlayMakerFSM> onStateExit)
    {
        if (StateExitData.ContainsKey(data))
        {
            StateExitData[data] -= onStateExit;
        }
    }
    /// <summary>
    /// Hook that gets called when a state is entered by a transition (could be global or local). The transition from which it happened is passed into the action. 
    /// </summary>
    /// <param name="data">The data necessary to find the fsm to be edited</param>
    /// <param name="onStateEnteredFromTransition">The action that will be invoked when the state is entered, the parameter passed into the action is the fsm and the transition from which the state enter happened</param>
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
    /// <summary>
    /// Hook that gets called when a state is exited via a transition (could be global or local). The transition from which it happened is passed into the action.
    /// </summary>
    /// <param name="data">The data necessary to find the fsm to be edited</param>
    /// <param name="onStateExitViaTransition">The action that will be invoked when the state is exited, the parameter passed into the action is the fsm and the transition from which the state exit happened</param>
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
    /// <summary>
    /// Unhook your action from the hook that gets called when a state is entered by a transition (could be global or local)
    /// </summary>
    /// <param name="data">The data necessary to find the fsm to be edited</param>
    /// <param name="onStateEnteredFromTransition">The action that will be removed</param>
    public static void UnHookStateEnteredFromTransitionedFromTransition(FSMData data, Action<PlayMakerFSM, string> onStateEnteredFromTransition)
    {
        if (StateEnteredFromTransitionData.ContainsKey(data))
        {
            StateEnteredFromTransitionData[data] -= onStateEnteredFromTransition;
        }
    }
    /// <summary>
    /// Unhook your action from the hook that gets called when a state is exited via a transition (could be global or local)
    /// </summary>
    /// <param name="data">The data necessary to find the fsm to be edited</param>
    /// <param name="onStateExitViaTransition">The action that will be removed</param>
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
            onStateEnter_1.TryInvokeActions(self.FsmComponent);
        }
        if (StateEnterData.TryGetValue(new FSMData(gameObject, fsmName, stateName), out var onStateEnter_2))
        {
            onStateEnter_2.TryInvokeActions(self.FsmComponent);
        }
        if (StateEnterData.TryGetValue(new FSMData(fsmName, stateName), out var onStateEnter_3))
        {
            onStateEnter_3.TryInvokeActions(self.FsmComponent);
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
            onStateExitedViaTransition_1.TryInvokeActions(self.FsmComponent, transition.EventName);
        }
        if (StateExitedViaTransitionData.TryGetValue(new FSMData(gameObject, fsmName, self.ActiveStateName), out var onStateExitedViaTransition_2))
        {
            onStateExitedViaTransition_2.TryInvokeActions(self.FsmComponent, transition.EventName);
        }
        if (StateExitedViaTransitionData.TryGetValue(new FSMData(fsmName, self.ActiveStateName), out var onStateExitedViaTransition_3))
        {
            onStateExitedViaTransition_3.TryInvokeActions(self.FsmComponent, transition.EventName);
        }
        
        
        if (StateEnteredFromTransitionData.TryGetValue(new FSMData(sceneName, gameObject, fsmName, transition.ToState), out var onStateEnteredFromTransition_1))
        {
            onStateEnteredFromTransition_1.TryInvokeActions(self.FsmComponent, transition.EventName);
        }
        if (StateEnteredFromTransitionData.TryGetValue(new FSMData(gameObject, fsmName, transition.ToState), out var onStateEnteredFromTransition_2))
        {
            onStateEnteredFromTransition_2.TryInvokeActions(self.FsmComponent, transition.EventName);
        }
        if (StateEnteredFromTransitionData.TryGetValue(new FSMData(fsmName, transition.ToState), out var onStateEnteredFromTransition_3))
        {
            onStateEnteredFromTransition_3.TryInvokeActions(self.FsmComponent, transition.EventName);
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
            onStateExit_1.TryInvokeActions(self.FsmComponent);
        }
        if (StateExitData.TryGetValue(new FSMData(gameObject, fsmName, stateName), out var onStateExit_2))
        {
            onStateExit_2.TryInvokeActions(self.FsmComponent);
        }
        if (StateExitData.TryGetValue(new FSMData(fsmName, stateName), out var onStateExit_3))
        {
            onStateExit_3.TryInvokeActions(self.FsmComponent);
        }
        
        orig(self, state);
    }

    private static void TryInvokeActions<T>(this Action<T> action, T param)
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
    
    private static void TryInvokeActions<T, V>(this Action<T, V> action, T param1, V param2)
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