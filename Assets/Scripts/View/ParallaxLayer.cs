using UnityEngine;

namespace Platformer.View
{
    /// <summary>
    /// Used to move a transform relative to the main camera position with a scale factor applied.
    /// This is used to implement parallax scrolling effects on different branches of gameobjects.
    /// </summary>
    public class ParallaxLayer : MonoBehaviour
    {
        public Vector3 movementScale = Vector3.one;
        public float spriteWidth;  // Breite des Sprites

        private Transform _camera;
        private Vector3 _startPosition;

        void Awake()
        {
            _camera = Camera.main.transform;
            _startPosition = transform.position;

            // Berechne die Breite des Sprites, wenn nicht gesetzt
            if (spriteWidth <= 0)
            {
                SpriteRenderer renderer = GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    spriteWidth = renderer.bounds.size.x;
                }
            }
        }

        void LateUpdate()
        {
            // Bewege das Layer entsprechend der Kamerabewegung
            Vector3 offset = Vector3.Scale(_camera.position - _startPosition, movementScale);
            //offset.y -= 2.5f; // Adjust the y position down by 0.5 units
            transform.position = _startPosition + offset;

            // Wenn die Kamera die Sprite-Breite überschreitet, springe das Layer zurück
            if (Mathf.Abs(_camera.position.x - transform.position.x) >= spriteWidth)
            {
                float offsetAmount = (_camera.position.x - transform.position.x) % spriteWidth;
                transform.position += new Vector3(offsetAmount, 0, 0);
            }
        }
    }
}