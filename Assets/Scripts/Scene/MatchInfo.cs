using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MatchInfo
{
    public List<Item> items = new List<Item>();
    public int startX, endX, startY, endY;

    public bool IsValid
    {
        get
        {
            return items.Count > 0;
        }
    }
}

