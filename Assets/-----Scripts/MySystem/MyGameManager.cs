using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using LittleTurtle.UI;
using System.Threading.Tasks;
using TMPro;

namespace LittleTurtle.System_ {

    public class MyGameManager : MonoBehaviour {

        public static MyGameManager Instance;

        public GameObject loadingPanelPrefab;

        public static bool isLoadingSaved;
        public Curves _curves;

        // save system
        private List<IDatapersistence> dataPersistenceObjects;
        private DataFileHandler dataHandler;
        private GameData gameData;
        private List<IDatapersistence> FindAllDataPersistenceOjects() {
            IEnumerable<IDatapersistence> dataPersistenceObject = FindObjectsOfType<MonoBehaviour>(true).OfType<IDatapersistence>();
            return new List<IDatapersistence>(dataPersistenceObject);
        }

        void Awake() {
            if (Instance == null) {
                Instance = this;
            }
            else {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            dataHandler = new DataFileHandler("GameData");
            

            // environment setting
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 61;
        }

        public async void LoadScene(string sceneName, bool loadingSaved) {

            GameObject obj = Instantiate(loadingPanelPrefab, FindObjectOfType<Canvas>().transform);
            obj.transform.SetAsLastSibling();

            // optimize...
            TextLang progressText = obj.GetComponentInChildren<TextLang>();
            progressText.SetMainText("0%");
            progressText.SetAdditionalText("Loading", -2);
            progressText.SetAdditionalText("...", -1);

            isLoadingSaved = loadingSaved;
            var scene = SceneManager.LoadSceneAsync(sceneName);

            scene.allowSceneActivation = false;
            do {
                await Task.Delay(1000);
                progressText.SetMainText(scene.progress.ToString("0.0"));
                progressText.SetAdditionalText("Loading", -2);
                progressText.SetAdditionalText("...", -1);

            } while (scene.progress < 0.9f);

            progressText.SetMainText("100%");
            progressText.SetAdditionalText("Loading", -2);
            progressText.SetAdditionalText("...", -1);
            await Task.Delay(500);
            scene.allowSceneActivation = true;
        }

        public void SaveGame() {
            gameData = new GameData();

            dataPersistenceObjects = FindAllDataPersistenceOjects();
            foreach (IDatapersistence datapersistence in dataPersistenceObjects) {
                datapersistence.SaveData(ref gameData);
            }

            dataHandler.Save(gameData);
        }
        IEnumerator LoadGame() {
            yield return null;
            print("load game data");
            gameData = dataHandler.Load();
            
            if (gameData == null) {
                print("data file is null");
                SceneManager.LoadScene("Menu");  //optimate
            }
            else {
                dataPersistenceObjects = FindAllDataPersistenceOjects();
                foreach (IDatapersistence datapersistence in dataPersistenceObjects) {
                    print("try to load data");
                    datapersistence.LoadData(gameData);
                }
            }
        }
        private void OnApplicationQuit() {
            if (SceneManager.GetActiveScene().name == "Game") {
                SaveGame();
            }
        }


        private void OnDestroy() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode1) {
            if (scene.name == "Game") {
                Time.timeScale = 1;

                if (isLoadingSaved) {
                    StartCoroutine(LoadGame());
                }
                else {
                    SaveDataMgr.CreateNewGame();
                }
            }
        }

        public void StopGame() {
            Time.timeScale = 0;
        }
        public void ResumeGame() {
            Time.timeScale = 1;
        }
        public void ExitGame() {
            SceneManager.LoadScene("Menu");
            Time.timeScale = 1;
        }

    }
}
