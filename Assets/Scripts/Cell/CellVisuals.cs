using GameBoards.CellManagement;
using ObjectPooling;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static MainVars;
using Random = UnityEngine.Random;

namespace Cell.Visuals
{
    [RequireComponent(typeof(CellBase))]
    public class CellVisuals : MonoBehaviour, IPoolable
    {

        CellBase cellBase;                                      // base component
        public Sprite sprite;                                   // sprite for image
        internal Image image;                                   // ref to image component
        Animator animator;                                      // ref to animator
        GameObject explosion;                                   // ref to explosion after moving object to pool
        public bool isEndedAnimation;                           // bool indicating is animation is NOT playing
        [SerializeField] private GameObject VFX;                // ref to VFX gamObject (actually its a particle system)
        [SerializeField] private AudioClip SFX;                 // ref to audioCLip
        private static AudioSource audioSource;
        public float duration { get; private set; }             // duration of all VFX effects (currently using only one)


        CellManager cm;

        void Initialize()
        {
            // get refs to components
            cellBase = GetComponent<CellBase>();
            image = gameObject.GetComponent<Image>();
            cm = gameBoard.GetComponent<CellManager>();

            image.sprite = sprite;
            image.transform.localScale = Vector3.one * gameBoard.gemScale;

            // get duration of animation
            animator = GetComponent<Animator>();
            AnimationClip[] VFXclips = animator.runtimeAnimatorController.animationClips;
            duration = 0;
            foreach (AnimationClip ac in VFXclips)
            {
                duration += ac.length;
            }

            // TODO: very bad, change
            if (audioSource == null)
            {
                audioSource = Camera.main.gameObject.AddComponent<AudioSource>();
                audioSource.clip = SFX;
            }


            isEndedAnimation = true;

        }

        internal void Randomize()
        {
            if (cellBase == null)
            {
                Initialize();
            }
            sprite = spriteList[Random.Range(0, spriteList.Length)];
            image.sprite = sprite;
            name = sprite.name;
        }

        internal IEnumerator PlayVFX()
        {
            cm.playingAnimationsList.Add(this);
            // play animation
            animator.SetTrigger("Matched");
            //wait for it to end
            yield return new WaitForSeconds(duration);
            // instantiate VFX gameObject
            explosion = Instantiate(VFX, transform.parent);
            explosion.transform.localPosition = gameObject.transform.localPosition;
            image.enabled = false;

            if (!SoundManager.Instance.explosionSFX.isPlaying)
            {
                SoundManager.Instance.explosionSFX.Play(); 
            }

            yield return new WaitForSeconds(explosion.GetComponent<ParticleSystem>().main.duration);
            cm.playingAnimationsList.Remove(this);
        }

        public void ResetState()
        {
            if (image != null)
            {
                image.enabled = true;
            }
        }

        public GameObject GetGO()
        {
            return ((IPoolable)cellBase).GetGO();
        }
    }
}