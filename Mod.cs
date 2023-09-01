using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;

namespace StellaDragAndDropNS
{
    public class DragAndDrop : Mod
    {
        private Harmony? harmony;
        public static ModLogger? _Logger;

        private void Awake()
        {
            harmony = new Harmony("StellaDragAndDropNS.DragAndDrop");
            harmony.PatchAll();
        }

        public override void Ready()
        {
            _Logger = Logger;

            Logger.Log("Range selection Ready!");
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}