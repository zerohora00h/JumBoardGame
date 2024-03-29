using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Mirror;

public class Pawn : NetworkBehaviour
{
    [Header("Pawn Status")]
    public bool isMoving;
    public int currentTileIndex = 0;
    public Transform currentTile;
    public float speed = 5.0f;
    public PlayerObjectController playerOwner;
    
    private Vector3 origPos, targetPos;
    private float timeToMoving = 0.5f;
    public float jumpHeight = 0.7f;

    [Header("Pawn Cosmetics")]
    public MeshRenderer meshRenderer;
    public Material[] colors;

    void Start()
    {
        currentTileIndex = 0;
        PlayerCosmesticsSetup();
    }

    #region  Pawn Movement
    public void MoveNext(List<GameObject> routeTile, int numMoves)
    {
        int movesDone = 1;
        if (!isMoving)
        {
            isMoving = true;

            targetPos = routeTile[(currentTileIndex + 1)].transform.position;

            float distance = Vector3.Distance(transform.position, targetPos);

            int jumps = Mathf.RoundToInt(distance / 2.5f);

            DOTween.Sequence()
                .Append(transform.DOJump(targetPos + new Vector3(0f, 1f, 0f), jumpHeight, jumps, distance / 5).SetEase(Ease.Linear))
                .Append(transform.DOMove(targetPos + new Vector3(0f, 1f, 0f), timeToMoving).SetEase(Ease.Linear))
                .OnComplete(() =>
                {
                    isMoving = false;

                    currentTileIndex = (currentTileIndex + 1);
                    currentTile = routeTile[currentTileIndex].transform;

                    if (movesDone < numMoves)
                    {
                        if (currentTile.GetComponent<WayPointData>().pointType == WayPointData.WayPointEnumerator.End)
                        {
                            Debug.Log($"Player {playerOwner.PlayerName} has reached the end and remaining {numMoves - movesDone} moves will be ignored.");
                            return;
                        }

                        movesDone++;
                        MoveNext(routeTile, numMoves - 1);
                    }
                });
        }

    }


    public void MoveBack(List<GameObject> routeTile, int numMoves)
    {
        int movesDone = 1;
        if (!isMoving)
        {

            isMoving = true;
            // Obtém o tile anterior usando o índice atual do peão na lista routeTile
            targetPos = routeTile[(currentTileIndex - 1 + routeTile.Count) % routeTile.Count].transform.position;
            //get distance between current position and target position
            float distance = Vector3.Distance(transform.position, targetPos);
            int jumps = Mathf.RoundToInt(distance / 2.5f);

            DOTween.Sequence()
                .Append(transform.DOJump(targetPos + new Vector3(0f, 1f, 0f), jumpHeight, jumps, distance / 5).SetEase(Ease.Linear))
                .Append(transform.DOMove(targetPos + new Vector3(0f, 1f, 0f), timeToMoving).SetEase(Ease.Linear))
                .OnComplete(() =>
                {
                    isMoving = false;
                    // Atualiza o índice atual do peão e o tile atual
                    currentTileIndex = (currentTileIndex - 1 + routeTile.Count) % routeTile.Count;
                    currentTile = routeTile[currentTileIndex].transform;

                    if (movesDone < numMoves)
                    {
                        movesDone++;
                        MoveBack(routeTile, numMoves - 1);
                    }
                });

        }
    }
    #endregion

    public void PlayerCosmesticsSetup()
    {
        meshRenderer.material = colors[playerOwner.PawnColor];
    }
}


