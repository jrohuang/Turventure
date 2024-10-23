using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using LittleTurtle.System_;

namespace LittleTurtle.UI {
    public class TextLang : MonoBehaviour {

        public TMP_Text TMPText;
        private string currentText;

        public Dictionary<int, string> Texts = new Dictionary<int, string>();  // text keyWord and order

        private void Awake() {
            if (!Texts.ContainsKey(0)) {
                if (!string.IsNullOrEmpty(TMPText.text)) {
                    Texts.Add(0, TMPText.text);
                }
            }

            LanguageMgr.TextLangs.Add(this);
            UpdateText();
        }
        private void OnDestroy() {
            if (LanguageMgr.TextLangs.Contains(this)) LanguageMgr.TextLangs.Remove(this);
        }


        public void SetMainText(string s) {
            Texts.Clear();
            Texts.Add(0, s);

            UpdateText();
        }
        public void SetAdditionalText(string s, int order) {
            Texts.Add(order, s);
            UpdateText();
        }

        public void UpdateText() {
            if (Texts.Count == 0) return;

            currentText = null;
            List<string> result = new List<string>();
            
            Texts.OrderBy(kv => kv.Key).Select(kv => kv.Value).ToList().ForEach(s => currentText += (LanguageMgr.TryGetWord(s)));

            currentText = currentText.Replace("\r", "");
            
            TMPText.text = currentText;
        }
    }
}