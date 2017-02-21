using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dev90Lib.Observer;

public class Item : MonoBehaviour {

    public int x = 0;
    public int y = 0;

    public int id = 0;

    public void OnPositionChanged(int newX, int newY)
    {
        x = newX;
        y = newY;
        //gameObject.name = string.Format("[{0},{1}]", x, y);
    }

    void OnMouseDown()
    {
        this.PostEvent(EventID.ItemClicked, this);
    }

    public bool IsSame( Item other){
        if (id == other.id)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
