using System.Collections.Generic;
using UnityEngine;

namespace LittleTurtle{

    [System.Serializable]
    public class GameData{
        public TurtleData turleData = new TurtleData();
        public _MonsterData[] monsters;
        public PlayerData playerData = new PlayerData();
        public InventoryData inventoryData = new InventoryData();
        public List<PickableItemData> itemGameobjects = new List<PickableItemData>();
    }

    #region = ItemGameobject =
    public class PickableItemData {
        public int ID;
        public int Quantity;
        public Vector3 position;
    }
    #endregion

    #region = Turtle =
    [System.Serializable]
    public class TurtleData {
        public string Name;
        public float moveSpeed;
        public float HP;
        public float Growth;

        public int TurtleState;
        public Vector3 position;
        public Quaternion rotation;

        public float MaxHP;//
        public float MaxGrowth;//
    }
    #endregion

    #region = Monster =
    [System.Serializable]
    public class _MonsterData {
        public int ID;
        public Vector3 position;
        public float health;
        public Quaternion quaternion;
    }
    #endregion

    #region = Inventory =
    [System.Serializable]
    public class InventoryData {
        public int[][] BagItems;
        public int[][] EquipmentItems;
    }
    #endregion

    #region = Player =
    [System.Serializable]
    public class PlayerData {
        public float Health;
        public Vector3 position;
        public int CurrenetWeaponID;

        public int[] weaponList;
    }
    #endregion

}
