using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBird.Main
{
    public class BlockSpawner : MonoBehaviour
    {
        [SerializeField] private float space;
        [SerializeField] private float randomYRange;
        [SerializeField] private float distance;
        [SerializeField] private float moveSpeed;
        [SerializeField] private BlockController blockPrefab;

        private bool isMoving;
        private List<BlockController> activeBlocks = new List<BlockController>();
        private Queue<BlockController> blockPool = new Queue<BlockController>();
        private BlockController lastBlock;
        private BlockController colliderChecker;
        private float spawnPosX;
        private float endPosX;
        private float outColliderPosX;

        public float MoveSpeed => this.moveSpeed;

        public System.Action onScore;

        public void Init(float birdWidth, float maxOffsetX, float minOffsetX)
        {
            var halfBlockSize = this.blockPrefab.topBox.GetRealBounds().size.x * 0.5f;
            this.spawnPosX = maxOffsetX + halfBlockSize;
            this.endPosX = minOffsetX - halfBlockSize;
            this.outColliderPosX = -(birdWidth * 0.5f + halfBlockSize);

            this.blockPrefab.topBox.transform.localPosition = new Vector3(0f, this.space * 0.5f);
            this.blockPrefab.bottomBox.transform.localPosition = new Vector3(0f, -this.space * 0.5f);
            this.blockPrefab.gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (this.isMoving == false)
            {
                return;
            }

            if (this.activeBlocks.Count == 0)
            {
                return;
            }

            for (var i = 0; i < this.activeBlocks.Count; i++)
            {
                this.activeBlocks[i].transform.position -= new Vector3(this.moveSpeed * Time.fixedDeltaTime, 0f);
                if (this.activeBlocks[i].MarkedScore == false && this.activeBlocks[i].transform.position.x < 0f)
                {
                    this.onScore?.Invoke();
                    this.activeBlocks[i].MarkedScore = true;
                }
            }

            if (this.spawnPosX - this.lastBlock.transform.position.x > this.distance)
            {
                this.SpawnBlock();
            }

            if (this.activeBlocks[0].transform.position.x < this.endPosX)
            {
                var outOfViewBlock = this.activeBlocks[0];
                this.activeBlocks.RemoveAt(0);
                outOfViewBlock.gameObject.SetActive(false);
                this.blockPool.Enqueue(outOfViewBlock);
            }

            if (this.colliderChecker != null && this.colliderChecker.transform.position.x < this.outColliderPosX)
            {
                var index = this.activeBlocks.IndexOf(this.colliderChecker);
                index++;
                if (index == this.activeBlocks.Count)
                {
                    this.colliderChecker = null;
                }
                else
                {
                    this.colliderChecker = this.activeBlocks[index];
                }
            }
        }

        public void StartSpawning()
        {
            this.isMoving = true;
            if (this.lastBlock == null)
            {
                this.SpawnBlock();
            }
        }

        public void SetMoving(bool isMoving)
        {
            this.isMoving = isMoving;
        }

        public bool IsCollide(Bounds other, out Vector2 hitPoint)
        {
            if (this.colliderChecker == null)
            {
                hitPoint = Vector2.zero;
                return false;
            }

            return this.colliderChecker.IsCollide(other, out hitPoint);
        }

        public void Replay()
        {
            for (var i = 0; i < this.activeBlocks.Count; i++)
            {
                Destroy(this.activeBlocks[i].gameObject);
            }

            this.activeBlocks.Clear();
            this.lastBlock = null;
            this.colliderChecker = null;
            this.StartSpawning();
        }

        private void SpawnBlock()
        {
            if (this.blockPool.Count > 0)
            {
                this.lastBlock = this.blockPool.Dequeue();
            }
            else
            {
                this.lastBlock = Instantiate(this.blockPrefab, this.transform);
            }

            var randomY = Random.Range(-this.randomYRange, this.randomYRange);
            this.lastBlock.transform.position = new Vector2(this.spawnPosX, randomY);
            this.lastBlock.gameObject.SetActive(true);
            this.lastBlock.MarkedScore = false;
            this.activeBlocks.Add(this.lastBlock);

            if (this.colliderChecker == null)
            {
                this.colliderChecker = this.lastBlock;
            }
        }

        private void OnDrawGizmos()
        {
            if (this.colliderChecker != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(this.colliderChecker.transform.position, 0.3f * Vector3.one);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(new Vector3(this.outColliderPosX, 5f), new Vector3(this.outColliderPosX, -5f));
            }
        }
    }
}