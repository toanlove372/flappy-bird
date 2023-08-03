using FlappyBird.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBird.Main
{
    public class BirdController : MonoBehaviour
    {
        public enum BirdState
        {
            Idle,
            Flying,
            Hit,
            Dead,
        }

        [Header("Settings")]
        [SerializeField] private float jumpSpeed;
        [SerializeField] private float gravity;
        [SerializeField] private float minVerticalSpeed;
        [SerializeField] private float fallSpeed;

        [SerializeField] private GameObject hitFx;
        [SerializeField] private float hitFxDuration;

        [SerializeField] private Animator animator;
        public BoundingBox2D box;

        [Header("Testing only")]
        [SerializeField] private bool immortal;

        private BirdState state;
        private float verticalSpeed;
        private float topPosY;
        private float yGround;
        private BlockSpawner blockSpawner;

        public System.Action<BirdState> OnBirdStateChanged;

        private static int AnimStateHash = Animator.StringToHash("State");
        private const int IdleAnimState = 0;
        private const int FlyAnimState = 1;
        private const int HurtAnimState = 2;
        private const int KOAnimState = 3;

        private void FixedUpdate()
        {
            this.UpdateState();
        }

        public void Init(BlockSpawner blockSpawner, float yGround, float topPosY)
        {
            this.state = BirdState.Idle;
            this.animator.SetInteger(AnimStateHash, IdleAnimState);
            this.blockSpawner = blockSpawner;
            this.topPosY = topPosY;
            this.yGround = yGround;
        }

        public void StartPlaying()
        {
            this.SetState(BirdState.Flying);
            SoundManager.Instance.PlayOneShot("Flap");
        }

        public void Replay()
        {
            this.transform.position = Vector2.zero;
            this.SetState(BirdState.Flying);
            SoundManager.Instance.PlayOneShot("Flap");
        }

        public void SetState(BirdState state)
        {
            this.state = state;
            switch (this.state)
            {
                case BirdState.Idle:
                    this.state = BirdState.Idle;
                    this.transform.position = Vector2.zero;
                    this.animator.SetInteger(AnimStateHash, IdleAnimState);
                    break;
                case BirdState.Flying:
                    this.state = BirdState.Flying;
                    this.verticalSpeed = this.jumpSpeed;
                    this.animator.SetInteger(AnimStateHash, FlyAnimState);
                    break;
                case BirdState.Hit:
                    this.state = BirdState.Hit;
                    this.animator.SetInteger(AnimStateHash, HurtAnimState);
                    break;
                case BirdState.Dead:
                    this.state = BirdState.Dead;
                    this.animator.SetInteger(AnimStateHash, KOAnimState);
                    break;
            }

            this.OnBirdStateChanged?.Invoke(this.state);
        }

        private void UpdateState()
        {
            switch (this.state)
            {
                case BirdState.Idle:
                    break;
                case BirdState.Flying:
                    if (CheckCollision())
                    {
                        this.SetState(BirdState.Hit);
                        SoundManager.Instance.PlayOneShot("Hit");
                        SoundManager.Instance.PlaySingle("Fall");
                    }
                    else
                    {
                        if (Input.anyKeyDown)
                        {
                            this.verticalSpeed = this.jumpSpeed;
                            SoundManager.Instance.PlayOneShot("Flap");
                        }
                    }

                    this.verticalSpeed -= this.gravity * Time.fixedDeltaTime;
                    if (this.verticalSpeed < this.minVerticalSpeed)
                    {
                        this.verticalSpeed = this.minVerticalSpeed;
                    }
                    this.transform.position = new Vector2(0f, this.transform.position.y + this.verticalSpeed);
                    break;
                case BirdState.Hit:
                    this.transform.position = new Vector2(0f, this.transform.position.y - this.fallSpeed * Time.fixedDeltaTime);
                    if (this.transform.position.y < this.yGround + 0.1f)
                    {
                        this.transform.position = new Vector2(0f, this.yGround + 0.05f);
                        this.SetState(BirdState.Dead);
                        SoundManager.Instance.StopSound();
                    }

                    break;
                case BirdState.Dead:
                    break;
            }

        }

        private bool CheckCollision()
        {
            if (this.immortal)
            {
                return false;
            }

            if (this.transform.position.y < this.yGround)
            {
                return true;
            }

            if (this.blockSpawner.IsCollide(this.box.GetRealBounds(), out var hitPoint))
            {
                this.ShowHitFx(hitPoint);
                return true;
            }

            return false;
        }

        private void ShowHitFx(Vector2 pos)
        {
            StartCoroutine(IeHitFx());
            IEnumerator IeHitFx()
            {
                this.hitFx.gameObject.SetActive(true);
                this.hitFx.transform.position = pos;
                yield return new WaitForSeconds(this.hitFxDuration);
                this.hitFx.gameObject.SetActive(false);
            }
        }
    }
}