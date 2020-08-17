using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using ObjectPooling;
using GameBoards.CellManagement;
using static MainVars;

namespace Cell.Visuals
{
    [RequireComponent(typeof(CellBase))]
    public class CellVisuals : MonoBehaviour, IPoolable
    {
        // base component
        CellBase cellBase;
        // sprite for image
        public Sprite sprite;
        // ref to image component
        internal Image image;
        // ref to animator
        Animator animator;
        // ref to explosion after moving object to pool
        GameObject explosion;
        // bool indicating is animation is NOT playing
        public bool isEndedAnimation;
        // ref to VFX gamObject (actually its a particle system)
        [SerializeField] GameObject VFX;
        // duration of all VFX effects (currently using only one)
        public float duration { get; private set; }


        CellManager cm;

        void Initialize()
        {
            // get refs to components
            cellBase = GetComponent<CellBase>();
            image = gameObject.GetComponent<Image>();
            cm = gameBoard.GetComponent<CellManager>();

            image.sprite = sprite;
            image.transform.localScale = Vector3.one * cellBase.scale;

            // get duration of animation
            animator = GetComponent<Animator>();
            AnimationClip[] VFXclips = animator.runtimeAnimatorController.animationClips;
            duration = 0;
            foreach (AnimationClip ac in VFXclips)
            {
                duration += ac.length;
            }

            isEndedAnimation = true;

        }

        internal void Randomize()
        {
            if (image == null)
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