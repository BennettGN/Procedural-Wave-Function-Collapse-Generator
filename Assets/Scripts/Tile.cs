using UnityEngine;
using System.Collections.Generic; 

[CreateAssetMenu(fileName = "Tile", menuName = "Tiles/Tile")]
[System.Serializable]
public class Tile : ScriptableObject
{
    public string TileName;
    public GameObject Prefab;
    public TileConnection Top;
    public TileConnection Bottom;
    public TileConnection Left;
    public TileConnection Right;
    [SerializeField] private List<GameObject> optionalDecorations;

    public List<GameObject> OptionalDecorations
    {
        get { return optionalDecorations; }
    }
}

[System.Serializable]
public class TileConnection
{
    public List<Tile> connectedTiles = new List<Tile>();
}
