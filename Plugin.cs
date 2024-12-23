using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace EggStatus;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[HarmonyPatch]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} is here!");

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EggGrow), nameof(EggGrow.GetHoverText))]
    public static string EggGetHoverText_Patch(string __result, EggGrow __instance)
    {
        if (__instance == null) return __result;

        int remainTime = GetRemainTimeToEggGrowUp(__instance);

        if (remainTime <= 0) return __result;

        return __result.Replace(")", $", Born in {remainTime}s)");
    }

    public static int GetRemainTimeToEggGrowUp(EggGrow __instance)
    {
        ZDO zdo = Traverse.Create(__instance).Field("m_nview").Method("GetZDO").GetValue<ZDO>();
        if (zdo == null) return 0;

        float num = zdo.GetFloat(ZDOVars.s_growStart);

        return (int)(num + __instance.m_growTime) - (int)ZNet.instance.GetTimeSeconds();
    }
}
