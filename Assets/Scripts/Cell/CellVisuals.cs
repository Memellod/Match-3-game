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
    public class CellVisuals : MonoBehaviour
    {
        CellBase cellBase;
        public Sprite sprite;
        internal Image image;
        Animator animator;
        GameObject explosion;
        public bool isEndedAnimation;
        [SerializeField] GameObject VFX;
        public float duration { get; private set; }

        void Initialize()
        {
            cellBase = GetComponent<CellBase>();
            image = gameObject.GetComponent<Image>();

            image.sprite = sprite;
            image.transform.localScale = Vector3.one * cellBase.scale;

            animator = GetComponent<Animator>();
            AnimationClip[] VFXclips = animator.runtimeAnimatorController.animationClips;
            duration = 0;
            foreach (AnimationClip ac in VFXclips)
            {
                duration += ac.length;
            }

            CellManager cm = gameBoard.GetComponent<CellManager>();
            cm.CheckForWork += IsEnded;
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
            animator.SetTrigger("Matched");
            yield return new WaitForSeconds(duration);
            explosion = Instantiate(VFX, transform.parent);
            explosion.transform.localPosition = gameObject.transform.localPosition;
            transform.position += Vector3.one * 10000;
        }


        public void RevertIsEndedFlag()
        {
            isEndedAnimation = !isEndedAnimation;
        }

        bool IsEnded()
        {

            // check for animation ends
            if (isEndedAnimation)
            {
                //check for 
                return explosion == null;
            }
            return false;


        }

    }
}