using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBird.Utils
{
    public class BoundingBox2D : MonoBehaviour
    {
        [SerializeField] Bounds bounds;

        public bool IsCollide(Bounds other, out Vector2 hitPoint)
        {
            var realBounds = new Bounds(this.transform.position + this.bounds.center, this.bounds.size);
            if (realBounds.Intersects(other))
            {
                hitPoint = realBounds.ClosestPoint(other.center);
                return true;
            }
            hitPoint = Vector2.zero;
            return false;
        }

        public Bounds GetRealBounds()
        {
            return new Bounds(this.transform.position + this.bounds.center, this.bounds.size);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(this.transform.position + this.bounds.center, this.bounds.size);
        }
    }
}