using UnityEngine;

namespace VarVarGamejam.Effect
{
	public class Shake : MonoBehaviour
	{
		private float _duration, _maxDuration;
		private float _shakeAmount;

		private Vector3 _orPos;

		public void Launch(float duration, float shakeAmount)
        {
			_duration = duration;
			_maxDuration = duration;
			_shakeAmount = shakeAmount;
        }

		private void Start()
		{
			_orPos = transform.localPosition;
		}

		private void Update()
		{
			if (_duration > 0)
			{
				transform.localPosition = _orPos + _shakeAmount * Mathf.Lerp(0f, 1f, _duration / _maxDuration) * Random.insideUnitSphere;
				_duration -= Time.deltaTime;
				if (_duration <= 0f)
				{
					transform.localPosition = _orPos;
				}
			}
		}
	}
}
