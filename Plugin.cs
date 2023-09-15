using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;

namespace StellaDragAndDropNS
{
    public class Plugin : Mod, ITranslator
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

            //Test
            ContextMenuPatch.AddItem(new ConcreteContextMenuItem("menu_item_1", this, (e) => { }));
            ContextMenuPatch.AddItem(new ConcreteContextMenuItem("menu_item_2", this, (e) => { }));
            ContextMenuPatch.AddItem(new SeparatorContextMenuItem(this));
            ContextMenuPatch.AddItem(new DeleteContextMenuItem(this));

            Logger.Log("Range selection Ready!");
        }

        private void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        public string Translate(string term)
        {
            return term;
        }
    }
}