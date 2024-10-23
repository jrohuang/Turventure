using LittleTurtle.System_;

namespace LittleTurtle{
    public interface IDatapersistence{
        void LoadData(GameData data);

        void SaveData(ref GameData data);
    }
}
