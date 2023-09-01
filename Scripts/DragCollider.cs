using StellaDragAndDropNS;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace StellaDragAndDropNS
{
    internal class DragCollider : MonoBehaviour
    {
        public ColliderEvent _OnColliderEvent;

        private void OnTriggerEnter(Collider other)
        {
            DragAndDrop._Logger.Log($"Collide with: {other.name} {other.tag}");
        }

        private void OnTriggerExit(Collider other)
        {
        }
    }

    [System.Serializable]
    public class ColliderEvent : UnityEvent<Collider> { }
}
