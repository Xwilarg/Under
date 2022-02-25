using TMPro;
using UnityEngine;

namespace VarVarGamejam.Translation
{
    [RequireComponent(typeof(TMP_Text))]
    public class TMP_TextTranslate : MonoBehaviour
    {
        private string _key;

        private void Start()
        {
            _key = GetComponent<TMP_Text>().text;
            UpdateText();
        }

        public void UpdateText()
        {
            GetComponent<TMP_Text>().text = Translate.Instance.Tr(_key);
        }
    }
}
