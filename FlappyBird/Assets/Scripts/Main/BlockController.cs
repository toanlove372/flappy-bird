using FlappyBird.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBird.Main
{
    public class BlockController : MonoBehaviour
    {
        public BoundingBox2D topBox;
        public BoundingBox2D bottomBox;

        public bool MarkedScore { get; set; }

        public bool IsCollide(Bounds other, out Vector2 hitPoint)
        {
            return this.topBox.IsCollide(other, out hitPoint)
                || this.bottomBox.IsCollide(other, out hitPoint);
        }
    }
}