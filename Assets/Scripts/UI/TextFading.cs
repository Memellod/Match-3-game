using System.Collections;
using ObjectPooling;
using UnityEngine;
using UnityEngine.UI;
using static MainVars;

public class TextFading : MonoBehaviour, IPoolable
{
    Text text;
    float distanceToFly = 1f;
    float speed = 1f;
    public event EndOfAction endOfAnimation;

    public void StartAnimation(string newText, Vector3 startPosition)
    {
        if (endOfAnimation == null)
        {
            endOfAnimation = go => { };
        }
        if (text == null)
        {
            text = GetComponent<Text>();
        }
        text.text = newText;
        transform.localPosition = startPosition;
        transform.localScale = Vector3.one;
        StartCoroutine(nameof(Animate));
    }

    private IEnumerator Animate()
    {
        Vector3 end = transform.position + new Vector3(0, 1, 0) * distanceToFly;
        Vector3 start = transform.position;
        float t = 0;
        var color = text.color;
        while (text.color.a != 0)
        {
            yield return new WaitForEndOfFrame();
            t += speed * Time.deltaTime;
            transform.position = new Vector3(start.x, Mathf.Lerp(start.y, end.y, t), start.z);
            color.a = Mathf.Lerp(1, 0, t);
            text.color = color;
        }
        endOfAnimation?.Invoke(gameObject);
        endOfAnimation = null;
        yield break;
    }

    GameObject IPoolable.GetGO()
    {
        return gameObject;
    }

    void IPoolable.ResetState()
    {
        if (text != null)      
        {
            var color = text.color;
            color.a = 1;
            text.color = color;
        }
    }
}
