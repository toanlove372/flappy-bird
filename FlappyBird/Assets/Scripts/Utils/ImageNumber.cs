using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyBird.Utils
{
    public class ImageNumber : MonoBehaviour
    {
        [SerializeField] private float textHeight;
        [SerializeField] private Image prefabImage;
        [SerializeField] private Sprite[] numberSprites;

        private List<Image> activeImages = new List<Image>();
        private float[] numberImageSizes = new float[10];

        private bool isInitialized;

        // Start is called before the first frame update
        void Start()
        {
            this.Init();
        }

        private void Init()
        {
            if (this.isInitialized)
            {
                return;
            }

            for (var i = 0; i < this.numberSprites.Length; i++)
            {
                var originalWidth = this.numberSprites[i].bounds.size.x;
                var originalHeight = this.numberSprites[i].bounds.size.y;
                var ratio = originalHeight / originalWidth;
                var newWidth = this.textHeight / ratio;
                this.numberImageSizes[i] = newWidth;
            }
            this.prefabImage.gameObject.SetActive(false);

            this.isInitialized = true;
        }

        public void SetNumber(int number)
        {
            this.Init();

            var digits = this.GetIntArray(number);
            var i = 0;
            for (; i < digits.Length; i++)
            {
                Image image;
                if (i == this.activeImages.Count)
                {
                    image = Instantiate(this.prefabImage, this.transform);
                    this.activeImages.Add(image);
                }

                image = this.activeImages[i];
                image.gameObject.SetActive(true);
                image.sprite = this.numberSprites[digits[i]];
                image.GetComponent<RectTransform>().sizeDelta = new Vector2(this.numberImageSizes[digits[i]], this.textHeight);
            }

            for (var indexInactive = i; indexInactive < this.activeImages.Count; indexInactive++)
            {
                this.activeImages[indexInactive].gameObject.SetActive(false);
            }
        }

        private int[] GetIntArray(int num)
        {
            if (num == 0)
            {
                return new int[] { 0 };
            }

            var listOfInts = new List<int>();
            while (num > 0)
            {
                listOfInts.Add(num % 10);
                num = num / 10;
            }
            listOfInts.Reverse();
            return listOfInts.ToArray();
        }

        public int test;
        [ContextMenu("Test")]
        private void Test()
        {
            this.SetNumber(this.test);
        }
    }
}