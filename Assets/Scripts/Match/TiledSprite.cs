using UnityEngine;

namespace Bejeweled.Macth
{
    /// <summary>
    /// Component for GameObjects with a local <see cref="SpriteRenderer"/> 
    /// with draw mode set to <see cref="SpriteDrawMode.Tiled"/>.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class TiledSprite : MonoBehaviour
    {
        [SerializeField, Tooltip("The local SpriteRenderer component.")]
        private SpriteRenderer spriteRenderer;

        /// <summary>
        /// The rendering Sprite.
        /// </summary>
        public Sprite Sprite
        {
            get => spriteRenderer.sprite;
            set => spriteRenderer.sprite = value;
        }

        /// <summary>
        /// Rendering color for the piece sprite.
        /// </summary>
        public Color Color
        {
            get => spriteRenderer.color;
            set => spriteRenderer.color = value;
        }

        /// <summary>
        /// The pivot in world space.
        /// </summary>
        public Vector2 NormalizedPivot => Sprite.pivot / PixelsPerUnit;

        /// <summary>
        /// The size to render.
        /// </summary>
        public Vector2 Size
        {
            get => spriteRenderer.size;
            set => spriteRenderer.size = value;
        }

        /// <summary>
        /// The world space size as a Vector2Int.
        /// </summary>
        /// <returns>Always a <see cref="Vector2Int"/> instance.</returns>
        public Vector2Int TiledSize
            => new Vector2Int(
                (int)spriteRenderer.size.x,
                (int)spriteRenderer.size.y);

        /// <summary>
        /// The unique id based on the current sprite.
        /// </summary>
        public int Id => Sprite.GetInstanceID();

        /// <summary>
        /// The number of pixels in the sprite that correspond to one unit in world space.
        /// </summary>
        public float PixelsPerUnit => Sprite.pixelsPerUnit;

        /// <summary>
        /// The width in world space.
        /// </summary>
        public float Width => Size.x;

        /// <summary>
        /// The height in world space.
        /// </summary>
        public float Height => Size.y;

        private void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
        }
    }
}