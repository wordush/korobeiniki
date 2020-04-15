using UnityEngine;
using UnityEngine.Serialization;

namespace Menu
{
    public class MenuSoundHandler : MonoBehaviour
    {
        public AudioClip click;

        private AudioSource _source;

        private void Start()
        {
            _source = GetComponent<AudioSource>();
        }

        public void OnPointerEnterSound()
        {
            _source.clip = click;
            _source.Play();
        }
    }
}
