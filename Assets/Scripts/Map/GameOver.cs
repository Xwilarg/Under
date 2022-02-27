using UnityEngine;
using UnityEngine.UI;

namespace VarVarGamejam.Map
{
    public class GameOver : MonoBehaviour
    {
        public static GameOver Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private Image _fade;
        private AudioSource _deathAudio;

        private void Start()
        {
            _fade = GetComponent<Image>();
            _deathAudio = GetComponent<AudioSource>();
        }

        public void Loose()
        {
            _deathAudio.Play();
        }
    }
}
