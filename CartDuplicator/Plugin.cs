﻿using BepInEx;
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
        private const string modVersion = "1.5.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static CartDuplicator Instance;

        internal ManualLogSource mls;

        // Configuration fields
        private ConfigEntry<Vector3> duplicationOffset;
        private ConfigEntry<int> duplicationAmount;
        private ConfigEntry<bool> smallCartReplacement;
        private ConfigEntry<bool> dynamicSpawning;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            // Define configuration settings
            duplicationOffset = Config.Bind("General", "DuplicationOffset", new Vector3(2f, 0f, 0f), "Offset for the duplicated cart position");
            duplicationAmount = Config.Bind("General", "DuplicationAmount", 1,
                new ConfigDescription("Number of additional carts to duplicate",
                new AcceptableValueRange<int>(0, 10)));
            smallCartReplacement = Config.Bind("General", "SmallCartReplacement", false, "Replace the duplicated carts with the pocket C.A.R.T.");
            dynamicSpawning = Config.Bind("General", "DynamicSpawning", false, "Enable dynamic spawning based on player count");


            mls.LogInfo("Cart Duplicator Mod Loaded");

            harmony.PatchAll(typeof(CartDuplicator));
            harmony.PatchAll(typeof(DuplicateCartPatch));
        }

        // Methods to access configuration values
        public static Vector3 GetDuplicationOffset() => Instance.duplicationOffset.Value;
        public static int GetDuplicationAmount() => Instance.duplicationAmount.Value;
        public static bool GetSmallCartReplacement() => Instance.smallCartReplacement.Value;
        public static bool GetDynamicSpawning() => Instance.dynamicSpawning.Value;
    }
}
