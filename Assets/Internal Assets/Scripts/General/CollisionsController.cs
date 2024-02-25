using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Tilemaps;
using UnityEngine.Rendering;

// NOTE: May need to refactor into parent class and inherit from it to separate player and AI logic
public class CollisionsController : MonoBehaviour
{
    [TitleGroup("Properties")]
    [VerticalGroup("Properties/Debug")]
    [BoxGroup("Properties/Debug/Tilemaps"), SerializeField, ListDrawerSettings(NumberOfItemsPerPage = 2)]
    private List<Tilemap> tileMaps;

    [BoxGroup("Properties/Debug/Layers"), ShowInInspector, ReadOnly, SerializeField]
    private string currentCollisionLayer;
    [BoxGroup("Properties/Debug/Layers"), ShowInInspector, ReadOnly, SerializeField]
    private string currentSortingLayer;
    [BoxGroup("Properties/Debug/Layers"), ShowInInspector, ReadOnly, SerializeField]
    private string prevCollisionLayer;
    [BoxGroup("Properties/Debug/Layers"), ShowInInspector, ReadOnly, SerializeField]
    private string prevSortingLayer;

    [BoxGroup("Properties/Debug/Values"), ShowInInspector, ReadOnly, SerializeField]
    private float floorY = 0;
    public float m_floorY { get { return floorY; } }
    [BoxGroup("Properties/Debug/Values"), ShowInInspector, ReadOnly, SerializeField]
    private float tempFloorZ = 0;
    [BoxGroup("Properties/Debug/Values"), ShowInInspector, ReadOnly, SerializeField]
    private Vector3Int entityLocation;

    private Collider myCollider;
    public Collider m_myCollider { get { return myCollider; } set { myCollider = value; } }
    private Rigidbody rb;
    public Rigidbody m_rb { get { return rb; } }

    public void SetupCollisions()
    {
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
    }

    public bool CollidedWithGround(float yPos)
    {
        // Ground Collision Check
        if (yPos < floorY)
        {
            return true;
        }
        return false;
    }

    public void IncreaseCollisionLayer(SortingGroup sortingGroup)
    {
        // whenever the entity jumps store their current sorting and collision layer into temp variables
        prevCollisionLayer = currentCollisionLayer;
        prevSortingLayer = currentSortingLayer;

        // then increase Layer and Sorting Layer from sorting group by 1
        var layer = LayerMask.NameToLayer(currentCollisionLayer) + 1;
        string sortLayer = LayerMask.LayerToName(layer);

        // set the entity's collision and sorting layer to match those layers
        gameObject.layer = layer;
        sortingGroup.sortingLayerName = sortLayer;
        tempFloorZ++;
    }

    public bool IsOnSameLayer()
    {
        if (LayerMask.LayerToName(gameObject.layer) != currentCollisionLayer)
        {
            return false;
        }
        return true;
    }

    // TODO: Fix layers not validating properly causing a bug where player ends stuck between layers and colliders on lower levels
    public void ValidateLayer(SortingGroup sortingGroup)
    {
        if (LayerMask.LayerToName(gameObject.layer) != currentCollisionLayer)
        {
            gameObject.layer = LayerMask.NameToLayer(currentCollisionLayer);
            sortingGroup.sortingLayerName = currentSortingLayer;
            tempFloorZ = floorY;
            return;
        }
        floorY = tempFloorZ;
    }

    public void DecreaseCollisionLayer(SortingGroup sortingGroup)
    {
        //prevCollisionLayer = currentCollisionLayer;
        //prevSortingLayer = currentSortingLayer;

        //// then decrease Layer and Sorting Layer to current layer at xy pos
        //var layer = LayerMask.NameToLayer(currentCollisionLayer) - 1;
        //string sortLayer = LayerMask.LayerToName(layer);

        //// set the entity's collision and sorting layer to match those layers
        //gameObject.layer = layer;
        //sortingGroup.sortingLayerName = sortLayer;
        tempFloorZ--;
    }

    // find which tile they're one based on yPos and get that tile's game object's collision and sorting layer
    public void GetTileAtPos(Vector2 vector2)
    {
        foreach (Tilemap tilemap in tileMaps)
        {
            entityLocation = tilemap.WorldToCell(vector2);
            if (tilemap.GetTile(entityLocation))
            {
                currentCollisionLayer = LayerMask.LayerToName(tilemap.gameObject.layer);
                currentSortingLayer = tilemap.gameObject.GetComponent<TilemapRenderer>().sortingLayerName;
            }
        }
    }
}
