using UnityEngine;
using VarVarGamejam.Map;
using VarVarGamejam.SO;

namespace VarVarGamejam.Tablet
{
    public class Radar : MonoBehaviour
    {
        [SerializeField]
        private GameObject _line;

        [SerializeField]
        private RadarInfo _info;

        public GameObject Player { set; private get; }
        public SpriteRenderer PlayerRadarIcon { set; private get; }

        private Vector2 ToVector2(Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        private void Update()
        {
            var playerPos = ToVector2(Player.transform.position);
            var middle = Vector2.one * MapGeneration.Instance.Middle;

            var prevRot = _line.transform.rotation.eulerAngles.z;
            _line.transform.Rotate(0f, 0f, Time.deltaTime * _info.LineRotSpeed);
            var nextRot = _line.transform.rotation.eulerAngles.z;

            var angle = playerPos.x < middle.x ? Vector2.Angle(playerPos - middle, Vector2.up) : Vector2.Angle(middle - playerPos, Vector2.up) + 180;
            if (angle < prevRot && angle >= nextRot) // TODO: A small part isn't covered
            {
                PlayerRadarIcon.color = new Color(PlayerRadarIcon.color.r, PlayerRadarIcon.color.g, PlayerRadarIcon.color.b, 1f);
            }
            else
            {
                PlayerRadarIcon.color = new Color(PlayerRadarIcon.color.r, PlayerRadarIcon.color.g, PlayerRadarIcon.color.b, PlayerRadarIcon.color.a - _info.FadeSpeed * Time.deltaTime);
            }
        }
    }
}
