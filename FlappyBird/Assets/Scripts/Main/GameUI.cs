using FlappyBird.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyBird.Main
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private GameObject gameStartPanel;
        [SerializeField] private ImageNumber scoreText;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private ImageNumber resultScoreText;
        [SerializeField] private ImageNumber highScoreText;
        [SerializeField] private GameObject newHighScore;
        [SerializeField] private UnityEngine.UI.Image medalImage;
        [SerializeField] private Sprite[] medalSprites;
        [SerializeField] private int[] medalScores;

        public System.Action onStartClicked;
        public System.Action onRePlayClicked;

        public void Init()
        {
            this.gameStartPanel.SetActive(true);
        }

        public void ShowScore(int score)
        {
            this.scoreText.SetNumber(score);
        }

        public void ShowGameOver(int score, int highScore)
        {
            this.gameOverPanel.SetActive(true);
            this.resultScoreText.SetNumber(score);
            if (score > highScore)
            {
                this.highScoreText.SetNumber(score);
                this.newHighScore.SetActive(true);
            }
            else
            {
                this.highScoreText.SetNumber(highScore);
                this.newHighScore.SetActive(false);
            }

            this.medalImage.enabled = false;
            for (var i = 0; i < this.medalSprites.Length; i++)
            {
                if (score >= this.medalScores[i])
                {
                    this.medalImage.sprite = this.medalSprites[i];
                    this.medalImage.enabled = true;
                }
            }
        }

        public void OnPlayButtonClicked()
        {
            this.onStartClicked?.Invoke();
            this.gameStartPanel.SetActive(false);
        }

        public void OnRePlayButtonClicked()
        {
            this.onRePlayClicked?.Invoke();
            this.gameOverPanel.SetActive(false);
            SoundManager.Instance.PlayOneShot("Button");
        }
    }
}