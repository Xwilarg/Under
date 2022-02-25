using TMPro;
using UnityEngine;

namespace VarVarGamejam.Translation
{
    [RequireComponent(typeof(TMP_Text))]
    public class TMP_TextTranslate : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<TMP_Text>().text = Translate.Instance.Tr(GetComponent<TMP_Text>().text);
        }
    }
}
