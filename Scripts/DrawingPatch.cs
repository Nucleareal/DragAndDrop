using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace StellaDragAndDropNS
{
    [HarmonyPatch]
    internal static class DrawingPatch
    {
        private static Vector3 _startPosition;
        private static Vector3 _endPosition;

        private static Vector3 _startView;
        private static GameObject _object;
        private static Image _image;
        private static RectTransform _rect;
        private static List<int> _highlights = new List<int>();
        private static bool _isDragging = false;

        [HarmonyPatch(typeof(GameCard), "Update")]
        public static void Postfix(GameCard __instance)
        {
            if (__instance.IsDemoCard || !Plugin.Hightlights || !_isDragging)
            {
                return;
            }
            if(_highlights.Contains(__instance.GetHashCode()))
            {
                __instance.HighlightRectangle.enabled = true;
                __instance.HighlightRectangle.Color = Color.red;
            }
        }


        private static void OnDragStart(GameCamera __instance)
        {
            _startPosition = WorldManager.instance.mouseWorldPosition;
            _isDragging = true;
        }


        private static void OnDragUpdate(GameCamera __instance)
        {
            if (!Plugin.Hightlights) return;

            ClearHighlights();

            _endPosition = WorldManager.instance.mouseWorldPosition;

            var counter = 0;
            List<GameCard> inRangeCards = new List<GameCard>();
            foreach (var draggable in WorldManager.instance.AllDraggables)
            {
                if (counter >= Plugin.MaxStacking) break;

                if (IsInRange(draggable, _startPosition, _endPosition))
                {
                    if (draggable is GameCard card && IsStackable(draggable, card))
                    {
                        _highlights.Add(card.GetHashCode());
                    }
                }
            }
        }

        private static void ClearHighlights()
        {
            if (!Plugin.Hightlights) return;
            _highlights.Clear();
        }

        private static bool IsStackable(Draggable draggable, GameCard card)
        {
            return card.CanBeDragged();
        }

        private static void OnDragEnd(GameCamera __instance)
        {
            _isDragging = false;
            ClearHighlights();
         
            _endPosition = WorldManager.instance.mouseWorldPosition;

            var counter = 0;
            List<GameCard> inRangeCards = new List<GameCard>();
            foreach (var draggable in WorldManager.instance.AllDraggables)
            {
                if (counter >= Plugin.MaxStacking) break;

                if (IsInRange(draggable, _startPosition, _endPosition))
                {
                    if (draggable is GameCard card && IsStackable(draggable, card))
                    {
                        card.SetParent(null);
                        inRangeCards.Add(card);
                        card.HighlightRectangle.Color = Color.white;
                        card.HighlightRectangle.enabled = true;
                        counter++;
                    }
                }
            }

            if (inRangeCards.Count == 0) return;

            inRangeCards.Sort((a, b) => a.CardData.Id.CompareTo(b.CardData.Id));
            inRangeCards.Sort((a, b) => a.CardData.MyCardType.CompareTo(b.CardData.MyCardType));

            List<GameCard> stack = new List<GameCard>();
            stack.Add(inRangeCards[0]);
            for (int i = 1; i < inRangeCards.Count; i++)
            {
                inRangeCards[i].RemoveFromStack();
                if (stack.Last().CardData.CanHaveCardOnTop(inRangeCards[i].CardData))
                    stack.Add(inRangeCards[i]);
                else if (inRangeCards[i].CardData.CanHaveCardOnTop(stack.First().CardData))
                    stack.Insert(0, inRangeCards[i]);
                else
                {
                    WorldManager.instance.Restack(stack);
                    stack.Clear();
                    stack.Add(inRangeCards[i]);
                }
            }
            WorldManager.instance.Restack(stack);
        }

        private static bool IsInRange(Draggable draggable, Vector3 start, Vector3 end)
        {
            var minX = Math.Min(start.x, end.x);
            var minZ = Math.Min(start.z, end.z);
            var maxX = Math.Max(start.x, end.x);
            var maxZ = Math.Max(start.z, end.z);

            var p = draggable.transform.position;

            return minX <= p.x && p.x <= maxX && minZ <= p.z && p.z <= maxZ;
        }

        [HarmonyPatch(typeof(GameCamera), "Update")]
        [HarmonyPostfix]
        public static void OnCameraUpdate(GameCamera __instance)
        {
            var m = Mouse.current;

            if (m == null) return;

            var subButton = m.rightButton;

            if (subButton.wasPressedThisFrame)
            {
                //押した瞬間
                OnDragStart(__instance);
            }

            if (subButton.IsPressed())
            {
                //押されているならアップデート
                OnDragUpdate(__instance);
            }

            if (subButton.wasReleasedThisFrame)
            {
                //離した瞬間
                OnDragEnd(__instance);
            }
        }
        
        [HarmonyPatch(typeof(GameScreen), "Update")]
        [HarmonyPostfix]
        public static void OnScreenUpdate(GameScreen __instance)
        {
            var m = Mouse.current;

            if (m == null) return;

            var subButton = m.rightButton;

            if (subButton.wasPressedThisFrame)
            {
                //押した瞬間
                _object = new GameObject();
                _object.transform.SetParent(__instance.transform, false);

                _startView = m.position.ReadValue();
                _object.transform.position = _startView;

                _image = _object.AddComponent<Image>();
                _rect = _image.rectTransform;
                _image.color = new Color(1f, 1f, 1f, .5f);
                _rect.pivot = Vector2.zero;
            }

            if (subButton.IsPressed())
            {
                //押されているならアップデート
                var pos = m.position.ReadValue();

                var minX = Math.Min(pos.x, _startView.x);
                var minY = Math.Min(pos.y, _startView.y);
                var sizeX = Math.Max(pos.x, _startView.x) - minX;
                var sizeY = Math.Max(pos.y, _startView.y) - minY;

                _object.transform.position = new Vector3(minX, minY, 0f);
                _rect.sizeDelta = new Vector2(sizeX, sizeY);
            }

            if (subButton.wasReleasedThisFrame)
            {
                //離した瞬間
                UnityEngine.Object.Destroy(_object.gameObject);
            }
        }
    }
}
