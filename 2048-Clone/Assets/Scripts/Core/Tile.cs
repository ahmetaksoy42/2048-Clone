using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public TileStateSO tileState { get; set; }
    public Cell cell { get; set; }
    public int number { get; set; }
    public bool isLocked { get; set; }

    [SerializeField] private Image backgroundImage;
    [SerializeField] private TMP_Text numberText;

    public void SetState(TileStateSO tileState, int number)
    {
        this.tileState = tileState;
        this.number = number;

        backgroundImage.color = tileState.backgroundColor;
        numberText.color = tileState.textColor;
        numberText.text = number.ToString();
    }
    public void SpawnTile(Cell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }
        this.cell = cell;
        this.cell.tile = this;
        transform.position = cell.transform.position;
    }
    public void MoveTo(Cell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }
        this.cell = cell;
        this.cell.tile = this;
        StartCoroutine(Animate(cell.transform.position, false));
    }
    private IEnumerator Animate(Vector3 to, bool isMerging)
    {
        float elapsed = 0;
        float duration = 0.1f;

        Vector3 from = transform.position;

        while(elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed/duration);
            elapsed += Time.deltaTime;
            yield return null; //for bug
        }
        transform.position = to;
        if (isMerging )
        {
            Destroy(this.gameObject);
        }
    }
    public void Merge(Cell cell)
    {
        if(this.cell != null)
        {
            this.cell.tile = null;
        }
        this.cell= null;
        cell.tile.isLocked = true;
        StartCoroutine(Animate(cell.transform.position, true));
    }
}
