using TMPro;
using UnityEngine;

public class TileInformation : MonoBehaviour
{
    #region Tile variables

    [SerializeField] Transform PiecePosition;

    [SerializeField] int CurrentSpaceIndex;
    // public Material sectionColor;

    // public string locationName;

    // public string currentOwner;

    // public float purchasePrice;

    [SerializeField] TextMeshPro locationNameText;

    [SerializeField] TextMeshPro currentOwnerText;

    [SerializeField] TextMeshPro purchasePriceText;

    #endregion

    private void OnEnable()
    {
        //BoardManager.RequestTileTransform += GetPiecePosition();
    }

    private void OnDisable()
    {
        //BoardManager.RequestTileTransform -= GetPiecePosition();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public Transform GetPiecePosition()
    {
        return PiecePosition;
    }
    public int GetSpaceIndex()
    {
        return CurrentSpaceIndex;
    }
}
