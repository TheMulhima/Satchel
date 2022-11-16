namespace Satchel
{
    /// <summary>
    /// The Mod class, you probably don't need this.
    /// </summary>
    public class Satchel : Mod{

        new public string GetName() => AssemblyUtils.name;
        public override string GetVersion() => $"{AssemblyUtils.GetAssemblyVersionHash(AssemblyUtils.ver)}";
        public static Satchel Instance;
        public override void Initialize()
        {
            if (Instance == null) 
            { 
                Instance = this;
            }
            
            FsmStateHooks.HookStateEnteredFromTransition(new FSMData("ProxyFSM", "Flower?"), (fsm, trans) => Log($"Flower entered from {trans}"));
            //FsmStateHooks.HookStateExitedViaTransition(new FSMData("ProxyFSM", "Flower?"), (fsm, trans) => Log($"Flower exited from {trans}"));
        }
    }
}