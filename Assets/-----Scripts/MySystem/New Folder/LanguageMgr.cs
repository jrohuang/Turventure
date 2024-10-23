using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LittleTurtle.UI;

namespace LittleTurtle.System_ {
    public class LanguageMgr : MonoBehaviour {

        public string CSVFileName;
        private static Dictionary<string, string> strings = new Dictionary<string, string>();
        public static Language currentLanguage;
        public static LanguageMgr Instance;

        private void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start() {
#if UNITY_EDITOR
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {
                if (EditorPrefs.GetString("Language") == string.Empty) {
                    currentLanguage = Language.English;
                }
                else {
                    Enum.TryParse(EditorPrefs.GetString("Language"), out Language l);
                    currentLanguage = l;
                }
            }
#endif
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
                if (PlayerPrefs.GetString("Language") == string.Empty) {
                    currentLanguage = Language.English;
                }
                else {
                    Enum.TryParse(PlayerPrefs.GetString("Language"), out Language l);
                    currentLanguage = l;
                }
            }
            ChangeLanguage();
        }

        public void ChangeLanguage() {

            Language nextLanguage;
            Language l;

#if UNITY_EDITOR
            Enum.TryParse(EditorPrefs.GetString("Language"), out l);
            nextLanguage = (Language)(((int)l + 1) % Enum.GetValues(typeof(Language)).Length);
            LanguageMgr.currentLanguage = nextLanguage;
#endif

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
                Enum.TryParse(PlayerPrefs.GetString("Language"), out l);
                nextLanguage = (Language)(((int)l + 1) % Enum.GetValues(typeof(Language)).Length);
                LanguageMgr.currentLanguage = nextLanguage;
            }


#if UNITY_EDITOR

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {
                EditorPrefs.SetString("Language", currentLanguage.ToString());
            }
#endif
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
                PlayerPrefs.SetString("Language", currentLanguage.ToString());
            }

            strings.Clear();
            string[] lines = Resources.Load<TextAsset>(CSVFileName).text.Split("\n");
            int languageIndex = (int)currentLanguage + 1;

            for (int i = 1; i < lines.Length - 1; i++) {
                string[] parts = lines[i].Split(',');
                strings.Add(parts[0], parts[languageIndex]);
            }

            ChangeAllText();
        }

        private void ChangeAllText() {
            List<TextLang> toRemove = new List<TextLang>();
            foreach (var t in TextLangs) {
                if (t) {
                    t.UpdateText();
                }
                else {
                    toRemove.Add(t);
                }
            }

            toRemove.ForEach(t => TextLangs.Remove(t));
        }

        public static List<TextLang> TextLangs = new List<TextLang>();

        public static string TryGetWord(string word) {
            return strings.TryGetValue(word, out string s) ? s : word;
        }
    }

    public enum Language {
        English,
        Chinese
    }
}