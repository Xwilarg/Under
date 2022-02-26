using UnityEngine;

namespace VarVarGamejam.Menu
{
    public class GoalManager : MonoBehaviour
    {
        public static GoalManager Instance { get; private set; }

        [SerializeField]
        private GameObject _takeHelp;

        [SerializeField]
        private Camera _minimapCamera;

        public GameObject ObjectiveObj { private get; set; }

        private void Awake()
        {
            Instance = this;
        }

        public void ToggleTakeHelp(bool value)
        {
            _takeHelp.SetActive(value);
        }

        public void TakeObjective()
        {
            ObjectiveObj.SetActive(false);
        }

        public void SetMinimapCamera(float x, float y, float size)
        {
            _minimapCamera.transform.position = new Vector3(x, 10f, y);
            _minimapCamera.orthographicSize = size;
        }
    }
}
