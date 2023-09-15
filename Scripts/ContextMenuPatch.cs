using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using TMPro;

namespace StellaDragAndDropNS
{
    [HarmonyPatch]
    public static class ContextMenuPatch
    {
        private static bool _isOpening;
        private static GameObject _menuObject;
        private static Vector2 _menuOrigin;
        private static Image _menuImage;
        private static RectTransform _menuRect;
        private static List<ContextMenuItem> _contextMenuItems = new List<ContextMenuItem>();
        private static GameCard _target;

        public static void AddItem(ContextMenuItem item)
        {
            _contextMenuItems.Add(item);
        }

        [HarmonyPatch(typeof(GameScreen), "Update")]
        [HarmonyPostfix]
        public static void OnScreenUpdate(GameScreen __instance)
        {
            //マウスの接続検知
            var m = Mouse.current;
            if (m == null) return;

            var openMenuButton = m.middleButton;

            if (openMenuButton.wasPressedThisFrame)
            {
                //押した瞬間
                
                if(!_isOpening)
                {
                    //if the menu is not opening and a card was clicked then open a menu
                    //メニューが開いていなくて、カードをクリックした場合はメニューを開く
                    //押した瞬間

                    //カーソルが当たっているカード
                    var card = WorldManager.instance.HoveredDraggable;

                    if (WorldManager.instance.HoveredDraggable is null) return;
                    if (WorldManager.instance.HoveredDraggable is not GameCard) return;

                    Plugin._Logger.Log("MiddleClick Detected");

                    _target = (GameCard)WorldManager.instance.HoveredDraggable;

                    _menuObject = new GameObject();
                    _menuObject.transform.SetParent(__instance.transform, false);

                    _menuOrigin = m.position.ReadValue();
                    //TODO: メニューを出す位置の調整
                    _menuObject.transform.position = _menuOrigin;

                    _menuImage = _menuObject.AddComponent<Image>();
                    _menuImage.color = new Color(1f, 1f, 1f, .5f);
                    _menuRect = _menuImage.rectTransform;
                    _menuRect.pivot = Vector2.zero;
                    _menuRect.sizeDelta = new Vector2(400, 400);

                    //メニューを追加
                    var layout = _menuObject.AddComponent<VerticalLayoutGroup>();
                    layout.childForceExpandHeight = false;
                    layout.spacing = 2f;

                    var fitter = _menuObject.AddComponent<ContentSizeFitter>();
                    fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                    foreach (var item in _contextMenuItems)
                    {
                        if (!item.IsVisiable(_target)) continue;

                        var viewItem = new GameObject();
                        viewItem.transform.SetParent(_menuRect.transform, false);

                        var viewModel = viewItem.AddComponent<ViewContextMenuItem>();
                        viewModel.SetLogic(item);
                        viewModel.SetTarget(_target);

                        var image = viewItem.AddComponent<Image>();
                        image.color = new Color(1f, 1f, 1f, .5f);
                        image.rectTransform.sizeDelta = new Vector2(400f, 50f);

                        var le = viewItem.AddComponent<LayoutElement>();
                        le.minHeight = 50f;
                        if (item.IsSeparator) le.minHeight /= 2;

                        {
                            var viewChildText = new GameObject();
                            
                            var textview = viewChildText.AddComponent<TextMeshProUGUI>();
                            textview.text = item.Text;
                            textview.color = Color.black;

                            var textRect = viewChildText.GetComponent<RectTransform>();
                            textRect.sizeDelta = image.rectTransform.sizeDelta;

                            viewChildText.transform.SetParent(viewItem.transform, false);
                        }
                    }

                    _isOpening = true;
                }
                else
                {
                    //if the menu is opening and place clicked nonmenu area then close the menu
                    //メニューが開いていて、メニュー外をクリックした場合はメニューを閉じる

                    UnityEngine.Object.Destroy(_menuObject.gameObject);
                    _isOpening = false;
                }
            }

            GameScreen.instance.ControllerIsInUI = _isOpening;
            //カーソルがアイテムの上にある
        }

        public static void CloseMenu()
        {
            UnityEngine.Object.Destroy(_menuObject.gameObject);
            _isOpening = false; 
        }
    }
}
