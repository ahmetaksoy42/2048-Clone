using DG.Tweening;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private TileStateSO[] tileStates;

    private float animDuration = 0.1f;
    private bool isWaiting;

    private List<Tile> tiles = new();

    private TileGrid tileGrid;

    private void Awake()
    {
        tileGrid = GetComponentInChildren<TileGrid>();
    }
    private void Start()
    {
       
    }
    private void Update()
    {
        if (!isWaiting)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveTiles(Vector2Int.down, 0, 1, tileGrid.GetHeight() - 2, -1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveTiles(Vector2Int.right, tileGrid.GetWidth() - 2, -1, 0, 1);
            }
        }
    }
    private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool isChanged = false;
        for (int x = startX; x >= 0 && x < tileGrid.GetWidth(); x += incrementX)
        {
            for (int y = startY; y >= 0 && y < tileGrid.GetHeight(); y += incrementY)
            {
                Cell cell = tileGrid.GetCell(x, y);
                if (cell.isOccupied)
                {
                    isChanged |= MoveTile(cell.tile, direction);
                }
            }
        }
        if (isChanged)
        {
            StartCoroutine(WaitForChanges());
        }
    }
    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        Cell newCell = null;
        Cell adjacentCell = tileGrid.GetAdjacentCell(tile.cell, direction);

        while (adjacentCell != null)
        {
            if (adjacentCell.isOccupied)
            {
                if (CanMerge(tile, adjacentCell.tile))
                {
                    Merge(tile, adjacentCell.tile);
                    return true;
                }
                break;
            }
            newCell = adjacentCell;
            adjacentCell = tileGrid.GetAdjacentCell(adjacentCell, direction);
        }
        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }
        return false;
    }
    private void Merge(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);
        int index = Mathf.Clamp(IndexOf(b.tileState) + 1, 0, tileStates.Length - 1);
        int number = b.number * 2;
        b.SetState(tileStates[index], number);

        AnimateTiles(b, animDuration);

        GameManager.Instance.IncreaseScore(number);
        AudioManager.Instance.Play(Consts.Audio.MERGE_SOUND);
    }
    private void AnimateTiles(Tile tileToAnimate, float animDuration)
    {
        tileToAnimate.gameObject.transform.DOScale(1.25f, animDuration).OnComplete(() =>
        {
            tileToAnimate.gameObject.transform.DOScale(1f, animDuration);
        });
    }
    private bool CanMerge(Tile a, Tile b)
    {
        return a.number == b.number && !b.isLocked;
    }
    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, tileGrid.transform);
        tile.SetState(tileStates[0], tileStates[0].number);
        tile.SpawnTile(tileGrid.GetRandomEmptyCell());
        tiles.Add(tile);
    }
    private int IndexOf(TileStateSO tileState)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (tileState == tileStates[i])
            {
                return i;
            }
        }
        return -1;
    }
    private IEnumerator WaitForChanges() //for bug fix
    {
        isWaiting = true;
        float waitingDuration = 0.1f;
        yield return new WaitForSeconds(waitingDuration);
        isWaiting = false;

        foreach (Tile tile in tiles)
        {
            tile.isLocked = false;
        }

        if (tiles.Count != tileGrid.GetSize())
        {
            CreateTile();
        }
        if (CheckForGameOver())
        {
            GameManager.Instance.GameOver();
        }
    }
    private bool CheckForGameOver()
    {
        if (tiles.Count != tileGrid.GetSize())
        {
            return false;
        }
        foreach (Tile tile in tiles)
        {
            Cell upCell = tileGrid.GetAdjacentCell(tile.cell, Vector2Int.up);
            Cell downCell = tileGrid.GetAdjacentCell(tile.cell, Vector2Int.down);
            Cell rightCell = tileGrid.GetAdjacentCell(tile.cell, Vector2Int.right);
            Cell leftCell = tileGrid.GetAdjacentCell(tile.cell, Vector2Int.left);
            if (upCell && CanMerge(tile, upCell.tile)) // upCell != null = upCell
            {
                return false;
            }
            else if (downCell && CanMerge(tile, downCell.tile))
            {
                return false;
            }
            else if (rightCell && CanMerge(tile, rightCell.tile))
            {
                return false;
            }
            else if (leftCell && CanMerge(tile, leftCell.tile))
            {
                return false;
            }
        }
        return true;
    }
    public void ClearBoard()
    {
        foreach(Cell cell in tileGrid.cells)
        {
            cell.tile = null;
        }
        foreach (Tile tile in tiles)
        {
            Destroy(tile.gameObject);
        }
        tiles.Clear();
    }
}

