using UnityEngine;
using System.Collections.Generic;

public class TownBuilder : MonoBehaviour
{
    [SerializeField] private int Width;
    [SerializeField] private int Height;
    [SerializeField] private Tile defaultTile;
    //Contains all possible tiles
    [SerializeField] private List<Tile> allTiles;
    [SerializeField] private float tileSizeOffset = 1.0f;
    [SerializeField ] private bool animateCollapse = false;
    [SerializeField] private int animateDelay = 0;
    [SerializeField] private bool startAtCenter = false;
    [SerializeField] private bool startAtBottomLeftCorner = false;
    [SerializeField] private bool startAtTopRightCorner = false;
    [SerializeField] private bool startAtBottomRightCorner = false;
    [SerializeField] private bool startAtTopLeftCorner = false;

    //Grid of tiles 
    private Tile[,] TownGrid;

    private List<Vector2Int> TilesToCollapse;

    private List<Vector2Int> offsets;

    private HashSet<Vector2Int> visited;

    public enum Direction
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }
    void Start()
    {
        visited = new HashSet<Vector2Int>();
        TownGrid = new Tile[Width, Height];
        TilesToCollapse = new List<Vector2Int>();
        TilesToCollapse.Clear();

        // Add starting positions based on toggles
        if (startAtCenter)
        {
            TilesToCollapse.Add(new Vector2Int(Width / 2, Height / 2));
        }

        if (startAtBottomLeftCorner)
        {
            TilesToCollapse.Add(new Vector2Int(2, 2));
        }

        if (startAtTopRightCorner)
        {
            TilesToCollapse.Add(new Vector2Int(Width - 2, Height - 2));
        }

        if (startAtBottomRightCorner)
        {
            TilesToCollapse.Add(new Vector2Int(Width - 2, 2));
        }

        if (startAtTopLeftCorner)
        {
            TilesToCollapse.Add(new Vector2Int(1, Height - 2));
        }

        offsets = new List<Vector2Int>
        {
            new Vector2Int(0, 1),   // Up
            new Vector2Int(1, 0),   // Right
            new Vector2Int(0, -1),  // Down
            new Vector2Int(-1, 0)   // Left
        };

        if (!animateCollapse)
        {
            CollapseWord();
        }
    }




    public void CollapseWord()
    {

        while (TilesToCollapse.Count > 0)
        {
            Collapse();
        }

    }

    public void Collapse()
    {
        int x = TilesToCollapse[0].x;
        int y = TilesToCollapse[0].y;

        if (TownGrid[x, y] != null)
        {
            TilesToCollapse.RemoveAt(0);
            return;
        }

        List<Tile> possibleTiles = new List<Tile>(allTiles);

        for (int i = 0; i < offsets.Count; i++)
        {
            Vector2Int neighborPos = new Vector2Int(x + offsets[i].x, y + offsets[i].y);

            if (IsInBounds(neighborPos))
            {
                Tile neighborConnection = TownGrid[neighborPos.x, neighborPos.y];

                //Remove nodes from possible tiles based on the possible tiles of the neighbor's adjacent connection
                if (neighborConnection != null)
                {
                    switch (i)
                    {
                        case (int)Direction.Up:
                            Intersect(possibleTiles, neighborConnection.Bottom.connectedTiles);
                            break;
                        case (int)Direction.Right:
                            Intersect(possibleTiles, neighborConnection.Left.connectedTiles);
                            break;
                        case (int)Direction.Down:
                            Intersect(possibleTiles, neighborConnection.Top.connectedTiles);
                            break;
                        case (int)Direction.Left:
                            Intersect(possibleTiles, neighborConnection.Right.connectedTiles);
                            break;
                        default:
                            Debug.LogError("Invalid direction parsing");
                            break;
                    }
                }
                else
                {
                    //Add neighbor to TilesToCollapse and assume all tiles are possible until collapsed
                    if (!TilesToCollapse.Contains(neighborPos))
                    {
                        TilesToCollapse.Add(neighborPos);
                    }
                }
            }
        }

        if (possibleTiles.Count > 0 && IsInBounds(new Vector2Int(x+1,y+1)) && IsInBounds(new Vector2Int(x - 1, y - 1)))
        {
            Tile tilePicked = possibleTiles[Random.Range(0, possibleTiles.Count)];
            TownGrid[x, y] = tilePicked;
            TilesToCollapse.RemoveAt(0);

        }
        else
        {
            if (visited.Contains(TilesToCollapse[0]))
            {
                //Render Default Tile if no possible tiles
                TownGrid[x, y] = defaultTile;
                Debug.Log("No possible tiles found, placing default tile at: " + x + ", " + y);
                TilesToCollapse.RemoveAt(0);
            }
            else
            {
                visited.Add(TilesToCollapse[0]);
                TilesToCollapse.Add(TilesToCollapse[0]);
                TilesToCollapse.RemoveAt(0);
                return;
            }
        }

        GameObject tileObject = Instantiate(TownGrid[x, y].Prefab, new Vector3(x * tileSizeOffset, 0, y * tileSizeOffset), TownGrid[x, y].Prefab.transform.rotation);
        if ((TownGrid[x, y].OptionalDecorations.Count > 0))
        {
            bool generateRandomDecoration = Random.Range(0, 2) == 0;
            if (generateRandomDecoration)
            {
                Instantiate(TownGrid[x, y].OptionalDecorations[Random.Range(0, TownGrid[x, y].OptionalDecorations.Count)], new Vector3(x * tileSizeOffset + Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), y * tileSizeOffset), Quaternion.identity);
            }
        }

    }

    //Updates current tiles to remove all current tiles that are not in allowed tiles (currentTiles = currentTiles intersect allowedTiles)
    private void Intersect(List<Tile> currentTiles, List<Tile> allowedTiles)
    {
        for (int i = currentTiles.Count - 1; i >= 0; i--)
        {
            if (!allowedTiles.Contains(currentTiles[i]))
            {
                currentTiles.RemoveAt(i);
            }
        }
    }


    private bool IsInBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height;
    }

    // Update is called once per frame
    void Update()
    {
        if (animateCollapse && TilesToCollapse.Count > 0)
        {
            Collapse();
            System.Threading.Thread.Sleep(animateDelay);
        }
    }

}
