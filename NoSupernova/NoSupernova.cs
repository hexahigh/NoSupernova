using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XGamingRuntime;



namespace NoSupernova
{
    public class NoSupernova : ModBehaviour
    {
        OWScene currentScene;
        DeathManager deathManager;
        SupernovaDestructionVolume SDV;
        SupernovaEffectController SEC;

        public static NoSupernova Instance;
        private void Awake()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.
        }

        [HarmonyPatch]
        public class PatchSun1
        {
            [HarmonyTargetMethod]
            static MethodBase CalculateMethod()
            {
                var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
                var original = typeof(SunController).GetMethods(bindingFlags)
                    .FirstOrDefault(m => m.Name == "Update");
                return original;
            }

            [HarmonyPrefix]
            public static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch]
        public class PatchSun2
        {
            [HarmonyTargetMethod]
            static MethodBase CalculateMethod()
            {
                var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
                var original = typeof(SunController).GetMethods(bindingFlags)
                    .FirstOrDefault(m => m.Name == "UpdateScale");
                return original;
            }

            [HarmonyPrefix]
            public static bool Prefix()
            {
                return false;
            }
        }
        [HarmonyPatch]
        public class PatchAudio1
        {
            [HarmonyTargetMethod]
            static MethodBase CalculateMethod()
            {
                var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
                var original = typeof(GlobalMusicController).GetMethods(bindingFlags)
                    .FirstOrDefault(m => m.Name == "OnTriggerSupernova");
                return original;
            }

            [HarmonyPrefix]
            public static bool Prefix()
            {
                return false;
            }
        }
        [HarmonyPatch]
        public class PatchAudio2
        {
            [HarmonyTargetMethod]
            static MethodBase CalculateMethod()
            {
                var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
                var original = typeof(OWAudioMixer).GetMethods(bindingFlags)
                    .FirstOrDefault(m => m.Name == "OnTriggerSupernova");
                return original;
            }

            [HarmonyPrefix]
            public static bool Prefix()
            {
                return false;
            }
        }


        private void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"Mod {nameof(NoSupernova)} is loaded!", MessageType.Success);


            // Example of accessing game code.
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene != OWScene.SolarSystem) return;
                ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
                currentScene = loadScene;
                if (currentScene is not OWScene.SolarSystem)
                {
                    SDV = null;
                    SEC = null;
                    return;
                }

            };

            SDV = FindObjectOfType<SupernovaDestructionVolume>();
            SEC = FindObjectOfType<SupernovaEffectController>();
            deathManager = FindObjectOfType<DeathManager>();

            // Disable timeloop and supernova deaths
            ModHelper.HarmonyHelper.AddPrefix<DeathManager>(nameof(DeathManager.KillPlayer), typeof(NoSupernova), nameof(NoSupernova.DeathManagerPrefix_KillPlayer));
        }
        private static bool DeathManagerPrefix_KillPlayer(DeathType deathType)
        {
            return deathType is not (DeathType.TimeLoop or DeathType.Supernova); //Skip original method and don't kill player on timeloop or supernova
        }
    }
}