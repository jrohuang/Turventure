using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;

namespace LittleTurtle.System_{

    public class DataFileHandler{

        private string FileName;
        
        public DataFileHandler(string fileName) {
            FileName = fileName;
        }

        public bool CheckFileExists() {
            return File.Exists(Path.Combine(Application.persistentDataPath, FileName)) ? true : false;
        }

        public GameData Load() {

            string path = Path.Combine(Application.persistentDataPath, FileName);
            
            GameData loadedData = null;

            if (CheckFileExists()) {
                try {
                    string data_Json; ;
                    using (FileStream stream = new FileStream(path, FileMode.Open)) {
                        using (StreamReader reader = new StreamReader(stream)) {
                            data_Json = reader.ReadToEnd();
                        }
                    }

                    loadedData = JsonConvert.DeserializeObject<GameData>(data_Json);

                }
                catch (Exception e) {
                    Debug.Log("Error : Load data file" + e);
                    File.Delete(path);
                    return null;
                }

            }

            return loadedData;
        }

        public void DeleateDataFile() {

        }
        public void Save(GameData data) {
            Debug.Log("Save Game");

            string path = Path.Combine(Application.persistentDataPath, FileName);

            try {
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                string data_json = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings() {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                // write data to file
                using (FileStream stream = new FileStream(path, FileMode.Create)) {
                    using (StreamWriter writer = new StreamWriter(stream)) {
                        writer.Write(data_json);
                    }
                }
            }
            catch(Exception e) {
                Debug.Log("Error : can't save file " + e);
            }

        }




    }
}
