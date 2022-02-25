using UnityEngine;

namespace VarVarGamejam.Menu
{
    public class GoalManager : MonoBehaviour
    {
        public static GoalManager Instance { get; private set; }

        [SerializeField]
        private GameObject _takeHelp;

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
    }
}
