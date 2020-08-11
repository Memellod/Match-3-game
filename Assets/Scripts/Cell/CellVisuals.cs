using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace Cell.Visuals
{
    public class CellVisuals : MonoBehaviour
    {
        CellBase cellBase;
        public Sprite sprite;
        internal Image image;

        void Initialize()
        {
            cellBase = GetComponent<CellBase>();
            image = gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.transform.localScale = Vector3.one * cellBase.scale;
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
    }
}