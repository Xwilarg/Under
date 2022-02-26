using UnityEngine;
using VarVarGamejam.Menu;

namespace VarVarGamejam.Player
{
    public class ObjectiveInit : MonoBehaviour
    {
        private void Start()
        {
            GoalManager.Instance.ObjectiveObj = gameObject;
        }
    }
}
