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

    public enum State
    {
        Rest,
        Work,
        Delivery1St,
        Delivery2Nd,
        Consum1St,
        Consum2Nd,
        Work1C,
        Work2C,
        Work3C
    }

    public enum Work
    {
        Rest,
        Work,

    }

    [Serializable]
    public class GameData
    {
        int _impratorLoyality;
        int _money;
        int _peasantsLoyality;
        int _peasants;
        GameStatment _satatment;
        GameProgress _progress;

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
        Wood,
        Gold,
        Floor,
        Corn,
        Firewood,
        Fiber,
        Plank,
        Potato
    }

    public class ItemStorage
    {
        public Dictionary<Item, int> items;
        public GameObject Destination;
        public bool ConsunationStarted;

        public ItemStorage(GameObject dest)
        {
            items = new Dictionary<Item, int>();
            Destination = dest;
            ConsunationStarted = false;
        }

        public bool IsItemExists(Item type)
        {
            return items.ContainsKey(type);
        }

        

        public static void ItemTransfer(Item item, ItemStorage from, ItemStorage to, int count = 0)
        {
            // Add item to list, if it doesnt exist yet
            
            if (!to.IsItemExists(item))
                if (count == 0)
                    to.items.Add(item, from.items[item]);
                else
                    to.items.Add(item, count);
            else 
                if (count == 0)
                    to.items[item] += from.items[item];
                else
                    to.items[item] += count;

            // Decrece item count
            if (count == 0)
                from.items[item] = 0;
            else
                from.items[item] -= count;

            if (from.items[item] <= 0)
                from.items.Remove(item);//Delete item if it count less then zero
        }
    }
    
}
