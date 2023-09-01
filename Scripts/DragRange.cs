using Shapes;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StellaDragAndDropNS
{
    internal class DragRange : MonoBehaviour
    {
        private RectTransform _t;
        private Image _image;
        private Vector2 _start;
        private Transform _parent;

        public DragRange(Transform transform, Vector2 start)
        {
            _parent = transform;
            _start = start;

            _image = gameObject.AddComponent<Image>();
            _t = _image.rectTransform;
            _t.position = _start;
            _t.pivot = Vector3.zero;
            _image.transform.SetParent(_parent.transform, false);
            _image.color = new Color(1f, 1f, 1f, .5f);
        }

        private void Awake()
        { 
        }

        public void SetPrepareStats(Transform parent, Vector3 start)
        {
            _parent = parent;
            _start = start;
        }

        internal void UpdateRect(Vector2 end)
        {
            var minx = Math.Min(_start.x, end.x);
            var miny = Math.Min(_start.y, end.y);

            var xsize = Math.Max(_start.x, end.x) - minx;
            var ysize = Math.Max(_start.y, end.y) - miny;

            _t.position = new Vector3(minx, miny);
            _t.sizeDelta = new Vector2(xsize, ysize);
        }

        internal void OnReleased(Vector2 end)
        {
            var minx = Math.Min(_start.x, end.x);
            var miny = Math.Min(_start.y, end.y);
            var xsize = Math.Max(_start.x, end.x) - minx;
            var ysize = Math.Max(_start.y, end.y) - miny;

            DragAndDrop._Logger.Log("OnReleased_0");

            var body = gameObject.AddComponent<Rigidbody>();
            body.isKinematic = true;

            DragAndDrop._Logger.Log("OnReleased_1");

            var box = gameObject.AddComponent<BoxCollider>();
            box.isTrigger = true;

            DragAndDrop._Logger.Log("OnReleased_2");

            box.transform.position = new Vector3(minx, miny, -1000f);
            box.size = new Vector3(xsize, ysize, 2000f);

            DragAndDrop._Logger.Log("OnReleased_3");
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"Collide with {collision.gameObject.name}, ({collision.gameObject.tag})");
        }
    }
}
