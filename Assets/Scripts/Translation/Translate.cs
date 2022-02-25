using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

namespace VarVarGamejam.Translation
{
    public class Translate
    {
        private string[] _languages =
        {
            "english",
            "french",
            "russian",
            "chinese",
            "german",
            "spanish"
        };

        private Translate()
        {
            Assert.IsTrue(File.Exists("Assets/Plugins/Newtonsoft.Json.dll"), "Missing Newtonsoft.Json plugin, check the README inside the Plugins/ folder");
            foreach (var lang in _languages)
            {
                _translationData.Add(lang, JsonConvert.DeserializeObject<Dictionary<string, string>>(Resources.Load<TextAsset>(lang).text));
            }
        }

        private static Translate _instance;
        public static Translate Instance
        {
            private set => _instance = value;
            get
            {
                _instance ??= new Translate();
                return _instance;
            }
        }

        public string Tr(string key)
        {
            var langData = _translationData[_currentLanguage];
            if (langData.ContainsKey(key))
            {
                return langData[key];
            }
            return _translationData["english"][key];
        }

        private string _currentLanguage = "english";

        private Dictionary<string, Dictionary<string, string>> _translationData = new();
    }
}
