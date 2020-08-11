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

        private void Start()
        {
            cellBase = GetComponent<CellBase>();
            image = gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.transform.localScale = Vector3.one * cellBase.scale;
        }

        internal void Randomize()
        {
            sprite = MainVars.spriteList[Random.Range(0, MainVars.spriteList.Length)];
            name = sprite.name;
        }
    }
}