﻿using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;

namespace StellaDragAndDropNS
{
    public class Plugin : Mod
    {
        private Harmony? harmony;

        public static ConfigEntry<int> MaxStackingEntry;
        public static int MaxStacking => MaxStackingEntry.Value;

        public static ConfigEntry<bool> HighlightsEntry;
        public static bool Hightlights => HighlightsEntry.Value;


        public static ModLogger? _Logger;

        private void Awake()
        {
            harmony = new Harmony("StellaDragAndDropNS.DragAndDrop");
            harmony.PatchAll();
        }

        public override void Ready()
        {
            _Logger = Logger;

            MaxStackingEntry = Config.GetEntry<int>("stacking_limit", 30);
            HighlightsEntry = Config.GetEntry<bool>("highlights", true);

            Logger.Log("Range selection Ready!");
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}