using UnityEngine;

namespace Bejeweled.Macth
{
    /// <summary>
    /// Board Sound component. Plays all the board sounds.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    public sealed class BoardSounds : MonoBehaviour
    {
        [SerializeField, Tooltip("The local AudioSource component.")]
        private AudioSource audioSource;

        [Header("Audio Clips")]
        [SerializeField, Tooltip("The AudioClip played when the piece is selected.")]
        private AudioClip selectPiece;
        [SerializeField, Tooltip("The AudioClip played when the piece is spawned.")]
        private AudioClip spawnPiece;
        [SerializeField, Tooltip("The AudioClip played when the piece is swapped.")]
        private AudioClip swapPiece;
        [SerializeField, Tooltip("The AudioClip played when a match is success.")]
        private AudioClip matchSuccess;
        [SerializeField, Tooltip("The AudioClip played when a invalid move is done.")]
        private AudioClip invalidPieceMove;

        private void Reset()
        {
            audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Plays the <see cref="selectPiece"/> audio.
        /// </summary>
        public void PlayPieceSelection() => audioSource.PlayOneShot(selectPiece);

        /// <summary>
        /// Plays the <see cref="spawnPiece"/> audio.
        /// </summary>
        public void PlayPieceSpawn() => audioSource.PlayOneShot(spawnPiece);

        /// <summary>
        /// Plays the <see cref="swapPiece"/> audio.
        /// </summary>
        public void PlayPieceSwap() => audioSource.PlayOneShot(swapPiece);

        /// <summary>
        /// Plays the <see cref="matchSuccess"/> audio.
        /// </summary>
        public void PlayMatchSucess() => audioSource.PlayOneShot(matchSuccess);

        /// <summary>
        /// Plays the <see cref="invalidPieceMove"/> audio.
        /// </summary>
        public void PlayInvalidPieceMove() => audioSource.PlayOneShot(invalidPieceMove);
    }
}