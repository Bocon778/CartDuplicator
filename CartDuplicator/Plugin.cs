using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using CartDuplicator.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CartDuplicator
{
    [BepInPlugin(modGUID, modeName, modVersion)]
    public class CartDuplicator : BaseUnityPlugin
    {
        private const string modGUID = "Bocon.CartDuplicator";
        private const string modeName = "Cart Duplicator";
        private const string modVersion = "1.2.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static CartDuplicator Instance;

        internal ManualLogSource mls;

        // Configuration fields
        private ConfigEntry<Vector3> duplicationOffset;
        private ConfigEntry<int> duplicationAmount;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            // Define configuration settings
            duplicationOffset = Config.Bind("General", "DuplicationOffset", new Vector3(2f, 0f, 0f), "Offset for the duplicated cart position");
            duplicationAmount = Config.Bind("General", "DuplicationAmount", 1, "Number of additional carts to duplicate");

            mls.LogInfo("Cart Duplicator Mod Loaded");

            harmony.PatchAll(typeof(CartDuplicator));
            harmony.PatchAll(typeof(DuplicateCartPatch));
        }

        // Methods to access configuration values
        public static Vector3 GetDuplicationOffset() => Instance.duplicationOffset.Value;
        public static int GetDuplicationAmount() => Instance.duplicationAmount.Value;
    }
}
