using FlappyBird.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBird.Main
{
    public class Gameplay : MonoBehaviour
    {
        [SerializeField] private Camera mainCam;
        [SerializeField] private BirdController bird;
        [SerializeField] private BlockSpawner blockSpawner;
        [SerializeField] private GameUI gameUI;
        [SerializeField] private float yGround;
        [SerializeField] private EnvironmentController environment;

        private int score;

        public Vector2 MinOffset { get; private set; }
        public Vector2 MaxOffset { get; private set; }

        public int Score
        {
            get => score; set
            {
                this.score = value;
                this.gameUI.ShowScore(value);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            this.Init();
            this.bird.Init(this.blockSpawner, this.yGround, this.MaxOffset.y);
            this.bird.OnBirdStateChanged += this.OnBirdStateChanged;
            this.blockSpawner.Init(this.bird.box.GetRealBounds().size.x, this.MaxOffset.x, this.MinOffset.x);
            this.blockSpawner.onScore += this.OnScore;
            this.gameUI.Init();
            this.gameUI.onStartClicked += this.OnStartClicked;
            this.gameUI.onRePlayClicked += this.OnRePlayClicked;
            this.environment.Init(this.blockSpawner.MoveSpeed, this.MaxOffset.x);
        }


        private void Init()
        {
            var camSize = this.mainCam.orthographicSize;
            var screenRatio = (float)Screen.width / Screen.height;
            var halfWidth = camSize * screenRatio;
            this.MinOffset = new Vector2(-halfWidth, -camSize);
            this.MaxOffset = new Vector2(halfWidth, camSize);
            this.Score = 0;
        }

        private void OnStartClicked()
        {
            this.bird.StartPlaying();
        }

        private void OnRePlayClicked()
        {
            this.bird.Replay();
            this.blockSpawner.Replay();
            this.Score = 0;
        }

        private void OnBirdStateChanged(BirdController.BirdState state)
        {
            switch (state)
            {
                case BirdController.BirdState.Idle:
                    break;
                case BirdController.BirdState.Flying:
                    this.blockSpawner.StartSpawning();
                    this.environment.SetMoving(true);
                    break;
                case BirdController.BirdState.Hit:
                    this.blockSpawner.SetMoving(false);
                    this.environment.SetMoving(false);
                    break;
                case BirdController.BirdState.Dead:
                    this.GameOver();
                    break;
            }
        }

        private void OnScore()
        {
            this.Score++;
            SoundManager.Instance.PlayOneShot("Score");
        }

        private void GameOver()
        {
            var highScore = PlayerPrefs.GetInt("HighScore", 0);
            if (this.Score > highScore)
            {
                PlayerPrefs.SetInt("HighScore", this.Score);
            }
            this.gameUI.ShowGameOver(this.Score, highScore);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector3(-10f, this.yGround), new Vector3(10f, this.yGround));
        }
    }
}