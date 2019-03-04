using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(menuName="soundSettings")]
    public class SoundSettings : ScriptableObject
    {
        [Header("mic settings")]
        public float minFreq = 150;
        public float maxFreq = 400 - 150;        
        public float minVolume;
        public float maxVolume = 20;

        [Header("in-game")]
        public float minY = -5;
        public float maxY = 5;
        public Color[] colors;
    }
}