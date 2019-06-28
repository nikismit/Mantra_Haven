using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Opera.Audio
{
    public class AudioText : MonoBehaviour
    {
        private Text _text;
        private Coroutine routine;

        void Start()
        {
            _text = GetComponent<Text>();            
        }

        public void Change(string text)
        {
            _text.text = text;
            
            if( routine != null )
                StopCoroutine(routine);
            
            routine = StartCoroutine(TextAnim());
        }

        IEnumerator TextAnim()
        {
            _text.color = Color.white;

            while(_text.color.r > 0.01)
            {
                _text.color = Color.Lerp(_text.color, new Color(0,0,0,0), Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}