using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


public class AStarTile {
  public Vector2Int pos;
  public int Cost;// { get; set; }
  public int Distance;// { get; set; }
  public int CostDistance => Cost + Distance;
  public AStarTile Parent;// { get; set; }

  //The distance is essentially the estimated distance, ignoring walls to our target. 
  //So how many tiles left and right, up and down, ignoring walls, to get there.
  
    /// TODO: Reimplement this to be more accurate
  public void SetDistance(Vector2Int targetPos) {
    Distance = Mathf.Abs(targetPos.x - pos.x) + Mathf.Abs(targetPos.y - pos.y);
  }
}


public class AStarAlgorithm : MonoBehaviour {

  public Vector2Int destination;


  public int DistanceToPosition(Vector2Int startPos, Vector2Int finishPos) {
    List<Vector2Int> path = CalculatePath(startPos, finishPos);
    if(path == null || path.Count == 0) {
      return 0;
    }
    return path.Count - 1;
  }

  public List<Vector2Int> CalculatePath(Vector2Int startPos, Vector2Int finishPos) {
    AStarTile start = new AStarTile { pos = startPos };
    AStarTile finish = new AStarTile { pos = finishPos };
    start.SetDistance(finish.pos);
    List<AStarTile> activeTiles = new List<AStarTile> { start };
    List<AStarTile> visitedTiles = new List<AStarTile>();
    List<Vector2Int> path = new List<Vector2Int>();


    while(activeTiles.Any()) {
      var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();
      if(checkTile.pos == finish.pos) { /// found the destination
        //Debug.Log("We have got to the destination!");
        //We found the destination and we can be sure that it's the most low cost option
        //(Because the OrderBy above)

        var tile = checkTile;
        while(tile != null) {
          path.Add(tile.pos);
          tile = tile.Parent;
        }
        path.Reverse();
        return path;
      }

      visitedTiles.Add(checkTile);
      activeTiles.Remove(checkTile);
      var walkableTiles = GetWalkableNeighbors(checkTile, finish);
      foreach(var walkableTile in walkableTiles) {
        //We have already visited this tile so we don't need to do so again!
        if(visitedTiles.Any(tile => tile.pos.x == walkableTile.pos.x && tile.pos.y == walkableTile.pos.y))
          continue;
        //It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might zigzag earlier but this is now straighter). 
        if(activeTiles.Any(tile => tile.pos.x == walkableTile.pos.x && tile.pos.y == walkableTile.pos.y)) {
          var existingTile = activeTiles.First(tile => tile.pos.x == walkableTile.pos.x && tile.pos.y == walkableTile.pos.y);
          if(existingTile.CostDistance > checkTile.CostDistance) {
            activeTiles.Remove(existingTile);
            activeTiles.Add(walkableTile);
          }
        } else {
          //We've never seen this tile before so add it to the list. 
          activeTiles.Add(walkableTile);
        }
      }
    }
    Debug.Log("No Path Found!");
    return null;
  }

  public List<AStarTile> GetWalkableNeighbors(AStarTile currentTile, AStarTile targetTile) {
    List<AStarTile> possibleTiles = new List<AStarTile>() {
      new AStarTile { pos = new Vector2Int(currentTile.pos.x, currentTile.pos.y - 1), Parent = currentTile, Cost = currentTile.Cost + 1 },
      new AStarTile { pos = new Vector2Int(currentTile.pos.x, currentTile.pos.y + 1), Parent = currentTile, Cost = currentTile.Cost + 1 },
      new AStarTile { pos = new Vector2Int(currentTile.pos.x - 1, currentTile.pos.y), Parent = currentTile, Cost = currentTile.Cost + 1 },
      new AStarTile { pos = new Vector2Int(currentTile.pos.x + 1, currentTile.pos.y), Parent = currentTile, Cost = currentTile.Cost + 1 },
    };
    possibleTiles.ForEach(tile => tile.SetDistance(targetTile.pos));

    return possibleTiles
      .Where(tile => BoardController.instance.IsPositionWalkable(tile.pos, GameController.instance.CurrentCharacter.combatTeam) || tile.pos == targetTile.pos)
      .ToList();
  }


  public void PrintPath(List<Vector2Int> path) {
    if(path == null || path.Count < 2) {
      return;
    }

    for(int i = 0; i < path.Count - 1; i++) {
      Debug.DrawLine(new Vector3(path[i].x + 0.5f, path[i].y + 0.5f, 0f),
                     new Vector3(path[i + 1].x + 0.5f, path[i + 1].y + 0.5f, 0f),
                     Color.cyan, 1f);
    }
  }
}
