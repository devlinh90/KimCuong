using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dev90Lib.Observer;

public class Board : MonoBehaviour
{

    public int xSize = 6, ySize = 6;

    private Item[,] _items;
    private Item currentClickedItem;
    void Start()
    {
        this.RegisterListener(EventID.ItemClicked, OnMouseClickedItem);
        FillBoard();
        ClearBoard();
    }
    private void FillBoard()
    {
        _items = new Item[xSize, ySize];
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                _items[x, y] = Utils.InstantiateItem(x, y);
            }
        }
    }

    private void ClearBoard()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                MatchInfo match = GetMatchInfo(_items[x, y]);
                if (match.IsValid)
                {
                    Destroy(_items[x, y].gameObject);
                    _items[x, y] = Utils.InstantiateItem(x, y);
                    y--;
                }
            }
        }
    }
    private void OnMouseClickedItem(Component component, object senderItem)
    {
        Item item = (Item)senderItem;

        if (currentClickedItem == item)
        {
            return;
        }

        if (currentClickedItem == null)
        {
            currentClickedItem = item;
        }
        else if (IsNeighBour(currentClickedItem, item))
        {
            StartCoroutine(TryMatch(currentClickedItem, item));
            currentClickedItem = null;
        }
        else
        {

        }
    }

    private float switchDuration = 0.1f;
    private IEnumerator TryMatch(Item a, Item b)
    {
        yield return StartCoroutine(SwitchItem(a, b, switchDuration));
        MatchInfo aMatch = GetMatchInfo(a);
        MatchInfo bMatch = GetMatchInfo(b);
        if (!aMatch.IsValid && !bMatch.IsValid)
        {
            yield return StartCoroutine(SwitchItem(a, b, switchDuration));
            yield break;
        }
        if (aMatch.IsValid)
        {
            Debug.Log("a match is valid");
            yield return StartCoroutine(DestroyMatch(aMatch));
            yield return StartCoroutine(UpdateBoardAfterMatch(aMatch));
        }
        if (bMatch.IsValid)
        {
            Debug.Log("b match is valid");
            yield return StartCoroutine(DestroyMatch(bMatch));
            yield return new WaitForSeconds(0.2f); // waitting for falling item
            yield return StartCoroutine(UpdateBoardAfterMatch(bMatch));
        }

    }

    private IEnumerator UpdateBoardAfterMatch(MatchInfo match)
    {
        Debug.Log(string.Format("{0}{1}-{2}{3}", match.startX, match.endX , match.startY, match.endY));
        Debug.Log("ABS x : " + (match.endX - match.startX));
        Debug.Log("ABS y : " + (match.endY - match.startY));
        if ( match.startY == match.endY && match.endX - match.startX >=2)
        {
            Debug.Log("update match horizon");
            for (int x = match.startX; x <= match.endX; x++)
            {
                //horizontal 
                for (int y = match.startY; y < ySize - 1; y++)
                {
                    Item upperIndex = _items[x, y + 1];
                    Item current = _items[x, y];
                    if (_items[x, y])
                    {
                        _items[x, y] = upperIndex;
                        _items[x, y + 1] = current;

                        _items[x, y].OnPositionChanged(_items[x, y].x, _items[x, y].y - 1);
                    }

                }
                Debug.Log("x: " + x + "instance horizontal");
                _items[match.startX, ySize - 1] = Utils.InstantiateItem(x, ySize - 1);
            }
        }
        else if (match.startX == match.endX && match.endY - match.startY >= 2)
        {
            Debug.Log("Updat match vertical");
            int matchHeight = 1 + (match.endY - match.startY);
            for (int y = match.startY + matchHeight; y < ySize; y++)
            {
                _items[match.startX, y].OnPositionChanged(match.startX, y - matchHeight);
                _items[match.startX, y - matchHeight] = _items[match.startX, y];
            }

            for (int i = 0; i < match.items.Count; i++)
            {
                Debug.Log("y: " + ((ySize - 1) - i) + "instance horizontal");
                _items[match.startX, (ySize -1 ) - i] = Utils.InstantiateItem(match.startX, (ySize -1) - i);
            }
        }
        yield return null;
        //for (int x = 0; x < xSize; x++)
        //{
        //    for (int y = 0; y < ySize; y++)
        //    {

        //        MatchInfo matchInfo = GetMatchInfo(_items[x, y]);
        //        if (matchInfo.IsValid)
        //        {
        //            yield return StartCoroutine(DestroyMatch(matchInfo));
        //            yield return new WaitForSeconds(switchDuration);
        //            yield return StartCoroutine(UpdateBoardAfterMatch(matchInfo));
        //        }
        //    }

        //}
    }
    IEnumerator DestroyMatch(MatchInfo m)
    {
        currentClickedItem = null;
        ChangeKinematic(true);
        foreach (Item item in m.items)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
            yield return null;
        }
        ChangeKinematic(false);

    }
    private MatchInfo GetMatchInfo(Item a)
    {
        MatchInfo match = new MatchInfo();
        List<Item> horizon = GetMatchHorizontal(a);
        List<Item> vertical = GetMatchVertical(a);

        if (horizon.Count >= 3 && horizon.Count >= vertical.Count)
        {
            match.items = horizon;
            match.startX = GetMininumX(horizon);
            match.endX = GetMaxinumX(horizon);
            match.startY = a.y;
            match.endY = a.y;
        }
        else if (vertical.Count >= 3)
        {
            match.items = vertical;
            match.startY = GetMininumY(vertical);
            match.endY = GetMaxinumY(vertical);
            match.startX =a.x;
            match.endX = a.x;
        }

        return match;
    }

    int GetMininumX(List<Item> Items)
    {
        int[] indices = new int[Items.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = Items[i].x;
        }

        return Mathf.Min(indices);
    }

    int GetMaxinumX(List<Item> Items)
    {
        int[] indices = new int[Items.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = Items[i].x;
        }

        return Mathf.Max(indices);
    }

    int GetMininumY(List<Item> Items)
    {
        int[] indices = new int[Items.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = Items[i].y;
        }

        return Mathf.Min(indices);
    }

    int GetMaxinumY(List<Item> Items)
    {
        int[] indices = new int[Items.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = Items[i].y;
        }

        return Mathf.Max(indices);
    }
    private List<Item> GetMatchHorizontal(Item a)
    {
        List<Item> horizon = new List<Item> { a };
        int left = a.x - 1;
        int right = a.x + 1;

        while (left >= 0 && a.IsSame(_items[left, a.y]))
        {
            horizon.Add(_items[left, a.y]);
            left--;
        }

        while (right < xSize && a.IsSame(_items[right, a.y]))
        {
            horizon.Add(_items[right, a.y]);
            right++;
        }

        return horizon;
    }

    private List<Item> GetMatchVertical(Item a)
    {
        List<Item> vertical = new List<Item> { a };
        int top = a.y + 1;
        int bottom = a.y - 1;

        while (top < ySize && a.IsSame(_items[a.x, top]))
        {
            vertical.Add(_items[a.x, top]);
            top++;
        }

        while (bottom >= 0 && a.IsSame(_items[a.x, bottom]))
        {
            vertical.Add(_items[a.x, bottom]);
            bottom--;
        }

        return vertical;
    }
    private IEnumerator SwitchItem(Item a, Item b, float duration)
    {
        Vector3 aPos = a.transform.position;
        Vector3 bPos = b.transform.position;
        ChangeKinematic(true);
        yield return StartCoroutine(a.transform.Move(bPos, duration));
        yield return StartCoroutine(b.transform.Move(aPos, duration));
        yield return new WaitForSeconds(duration);
        SwitchPosition(a, b);
        ChangeKinematic(false);
    }

    private void SwitchPosition(Item a, Item b)
    {
        int bX = b.x, bY = b.y;
        _items[a.x, a.y] = b;
        _items[b.x, b.y] = a;
        b.OnPositionChanged(a.x, a.y);
        a.OnPositionChanged(bX, bY);
    }

    private void ChangeKinematic(bool isKinematic)
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (_items[x, y] != null)
                {
                    _items[x, y].GetComponent<Rigidbody2D>().isKinematic = isKinematic;
                }
            }
        }
    }
    private bool IsNeighBour(Item a, Item b)
    {
        int distanceX = Mathf.Abs(a.x - b.x);
        int distanceY = Mathf.Abs(a.y - b.y);

        if (distanceX + distanceY == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
