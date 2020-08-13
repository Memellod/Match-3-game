using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using ObjectPooling;

namespace Cell.Visuals
{
    public class CellVisuals : MonoBehaviour, IPoolable
    {
        CellBase cellBase;
        public Sprite sprite;
        internal Image image;
        Animator animator;
        public float duration { get; private set; }

        void Initialize()
        {
            cellBase = GetComponent<CellBase>();
            image = gameObject.GetComponent<Image>();

            image.sprite = sprite;
            image.transform.localScale = Vector3.one * cellBase.scale;

            animator = GetComponent<Animator>();
            AnimationClip[] VFXclips = animator.runtimeAnimatorController.animationClips;
            duration = VFXclips[0].length;
        }

        internal void Randomize()
        {
            if (image == null)
            {
                Initialize();
            }
            sprite = MainVars.spriteList[Random.Range(0, MainVars.spriteList.Length)];
            image.sprite = sprite;
            name = sprite.name;
        }

        internal void PlayVFX()
        {
            animator.SetTrigger("Matched");
           // var go = Instantiate(VFX, transform.parent);
           // go.transform.localPosition = gameObject.transform.localPosition;
        }

        public void ResetState()
        {

        }

        public GameObject GetGO()
        {
            return ((IPoolable)cellBase).GetGO();
        }
    }
}