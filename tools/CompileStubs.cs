using System;
using System.Reflection;

public class Mod { }

[AttributeUsage(AttributeTargets.Method)]
public sealed class ConsoleCommandAttribute : Attribute {
    public ConsoleCommandAttribute(string name, string description) { }
}

namespace HarmonyLib {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class HarmonyPatch : Attribute {
        public HarmonyPatch(Type type, string methodName) { }
    }

    public sealed class Harmony {
        public Harmony(string id) { }
        public void PatchAll(Assembly assembly) { }
        public void UnpatchAll(string id) { }
    }

    public static class AccessTools {
        public static MethodInfo Method(Type type, string name) { return null; }
    }
}
