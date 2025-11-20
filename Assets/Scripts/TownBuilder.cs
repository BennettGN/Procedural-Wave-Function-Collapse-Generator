using UnityEngine;
using System.Collections.Generic;

public class TownBuilder : MonoBehaviour
{
    [SerializeField] private int Width;
    [SerializeField] private int Height;
    
    //Grid of tiles 
    private Tile[,] TownGrid;

    //Contains all possible tiles
    public List<Tile> allTiles;

    private List<Vector2Int> TilesToCollapse;

    void Start()
    {
        TownGrid = new Tile[Width, Height];
        TilesToCollapse = new List<Vector2Int>();
        allTiles = new List<Tile>(Resources.LoadAll<Tile>("Tiles"));
    }

    // Loop through TilesToCollapse, each time we come across uncollapsed neighbor

    // Update is called once per frame
    void Update()
    {
        
    }
}
