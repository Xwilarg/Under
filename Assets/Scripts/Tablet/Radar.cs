using UnityEngine;

namespace VarVarGamejam.Tablet
{
    public class Radar : MonoBehaviour
    {
        [SerializeField]
        private GameObject _line;

        [SerializeField]
        private float _speed;

        private void Update()
        {
            _line.transform.Rotate(0f, 0f, Time.deltaTime * _speed);
        }
    }
}
