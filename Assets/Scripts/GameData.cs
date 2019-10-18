using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

namespace GameStructure
{
    public enum Level
    {
        Level1,
        Level2,
        Level3,
        Level4,
        Level5
    }

    [Serializable]
    public class GameData
    {
        int ImpratorLoyality;
        int Money;
        int PeasantsLoyality;
        int Peasants;
        GameStatment Satatment;
        GameProgress Progress;

        public static void SaveGameData(GameData data, string name)
        {
            name += ".korobeinikiData";
            if (File.Exists(name))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + name, FileMode.Open);
                bf.Serialize(file, data);
                file.Close();
            }
            else
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + name, FileMode.Create);
                bf.Serialize(file, data);
                file.Close();
            }
        }

        public static GameData LoadGameData(string name)
        {
            name += ".korobeinikiData";
            if (File.Exists(name))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + name, FileMode.Open);
                GameData data = (GameData)bf.Deserialize(file);
                file.Close();
                return data;
            }
            else
            {
                Debug.LogError("File " + Application.persistentDataPath + name + " does not exists");
                return null;
            }
        }
    }

    public class GameStatment
    {
        // buildings, enviroment
    }

    public class GameProgress
    {

    }


    public enum Item
    {
        wood,
        gold
    }

    public class ItemStorage
    {
        public Dictionary<Item, int> items;
        public Vector3 Destination; 

        public ItemStorage()
        {
            items = new Dictionary<Item, int>();
        }

        public bool IsItemExists(Item type_)
        {
            return items.ContainsKey(type_);
        }

        

        public static void ItemTransfer(Item item, ItemStorage from, ItemStorage to, int count = 1)
        {
            // Add item to list, if it doesnt exist yet
            if (!to.IsItemExists(item))
                to.items.Add(item, 0);
            to.items[item] += count;

            // Decrece item count  
            from.items[item] -= count;
                if (from.items[item] == 0)
                    from.items.Remove(item);//Delete item if it count less then zero
        }
    }
    
}
