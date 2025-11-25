using UnityEngine;
using UnityEngine.UI;

namespace Ouroboros.Common.Utils.UI
{
    [RequireComponent(typeof(Image))]
    public class ImageColorCopier : MonoBehaviour
    {
        [SerializeField] private Image sourceImage;
        
        private Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        private void LateUpdate()
        {
            if (sourceImage == null) return;

            image.color = sourceImage.color;
        }
    }
}