using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class Utils : MonoBehaviour
{
    private static GameObject[] itemsPrefabs;
    public static GameObject GetRandomItem()
    {
        if (itemsPrefabs == null)
        {

            itemsPrefabs = Resources.LoadAll<GameObject>("Items");
        }
        int rndIndex = UnityEngine.Random.Range(0, itemsPrefabs.Length);
        Item item = itemsPrefabs[rndIndex].GetComponent<Item>();
        item.id = rndIndex;

        return itemsPrefabs[rndIndex];

    }

    public static Item InstantiateItem(int x, int y)
    {

        GameObject go = Instantiate(GetRandomItem(), new Vector3(x, y, 0), Quaternion.identity);
        Item item = go.GetComponent<Item>();
        item.OnPositionChanged(x, y);
        return item;
    }


}
