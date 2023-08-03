using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBird.Main
{
    public class EnvironmentController : MonoBehaviour
    {
        [SerializeField] private Transform foregroundLoop;
        [SerializeField] private float foregroundSize;
        [SerializeField] private Transform backroundLoop;
        [SerializeField] private float backgroundSize;
        [SerializeField] private float backgroundSpeedMultiplier;

        private float moveSpeed;
        private bool isMoving;

        private float halfScreenWidth;
        private List<Transform> foregroundList;
        private Transform lastForegroundPiece;
        private List<Transform> backgroundList;
        private Transform lastBackgroundPiece;

        private const float negativePaddingSize = 0.1f;

        // Update is called once per frame
        void FixedUpdate()
        {
            if (this.isMoving == false)
            {
                return;
            }

            //for (var i = 0; i < this.foregroundList.Count; i++)
            //{
            //    this.foregroundList[i].transform.Translate(-this.moveSpeed * Time.fixedDeltaTime, 0f, 0f);
            //    if (this.foregroundList[i].transform.position.x + this.foregroundSize * 0.5f < -this.halfScreenWidth)
            //    {
            //        this.foregroundList[i].transform.position = new Vector2(this.lastForegroundPiece.position.x + this.foregroundSize - 0.1f, 0f);
            //        this.lastForegroundPiece = this.foregroundList[i];
            //    }
            //}

            this.lastForegroundPiece = this.UpdateLoopPiece(this.foregroundList, this.lastForegroundPiece,
                foregroundSize, this.moveSpeed);
            this.lastBackgroundPiece = this.UpdateLoopPiece(this.backgroundList, this.lastBackgroundPiece,
                backgroundSize, this.moveSpeed * this.backgroundSpeedMultiplier);
        }

        public void Init(float moveSpeed, float halfScreenWidth)
        {
            this.moveSpeed = moveSpeed;
            this.halfScreenWidth = halfScreenWidth;

            this.foregroundList = this.InitLoopPiece(this.foregroundList, this.foregroundLoop,
                this.foregroundSize, out this.lastForegroundPiece);
            this.backgroundList = this.InitLoopPiece(this.backgroundList, this.backroundLoop,
                this.backgroundSize, out this.lastBackgroundPiece);
        }

        public void SetMoving(bool isMoving)
        {
            this.isMoving = isMoving;
        }

        private List<Transform> InitLoopPiece(List<Transform> list, Transform original, float size, out Transform lastPiece)
        {
            var farRightPos = -this.halfScreenWidth + size;
            original.position = new Vector2(farRightPos - 0.5f * size, 0f);
            list = new List<Transform>()
        {
            original,
        };
            while (farRightPos < this.halfScreenWidth + size + 0.1f)
            {
                var loopPiece = Instantiate(original, this.transform);
                loopPiece.position = new Vector2(farRightPos + size * 0.5f, 0);
                farRightPos += size;
                list.Add(loopPiece);
            }

            lastPiece = list[list.Count - 1];
            return list;
        }

        private Transform UpdateLoopPiece(List<Transform> list, Transform lastPiece, float size, float speed)
        {
            var newLastPiece = lastPiece;
            for (var i = 0; i < list.Count; i++)
            {
                list[i].transform.Translate(-speed * Time.fixedDeltaTime, 0f, 0f);
                if (list[i].transform.position.x + size * 0.5f < -this.halfScreenWidth)
                {
                    list[i].transform.position = new Vector2(this.lastForegroundPiece.position.x + size - negativePaddingSize, 0f);
                    newLastPiece = list[i];
                }
            }

            return newLastPiece;
        }
    }
}