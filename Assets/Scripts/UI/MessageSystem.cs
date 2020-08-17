using System.Collections;
using UnityEngine;

namespace UI
{
    public class MessageSystem
    {


        /// <summary>
        /// shows message for user for time seconds
        /// </summary>
        /// <param name="message"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        IEnumerator ShowMessage(string message, float time)
        {
            yield return new WaitForEndOfFrame();
            // show message

        }
    }
}