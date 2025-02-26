using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

    public enum GameState
{
    Play,
    Pause,
    GameOver,
    LevelComplete
}


public class GameController : MonoBehaviour
{



    public Tile[] shapeTiles; // Array to hold shape references
    public Tilemap shapeTilemap; // for drawing shapes and accessing them
    public Tile[] powerUpTiles; // Array to hold power up refeferences
    public Tilemap powerUpTilemap; // for drawing power ups and accessing them

    private Vector3Int? firstTile = null;
    private Vector3Int? secondTile = null;

    private int swapsRemaining = 20;
    public TMP_Text swapsToText;

    private int score = 0;
    public TMP_Text scoreToText;

    private const int HOME = 0;
    private const int LEVEL_1 = 1;
    private const int LEVEL_2 = 2;

    public GameObject pauseMenu;
    public GameObject rulesMenu;
    public GameObject gameOverScript;
    public GameObject levelCompleteMenu;


    private GameState currentState = GameState.Play;
    private int gridLength = 5;

    int pauseState = 0;

    private Scene currentScene;

    

void InitialiseTiles() {
    Debug.Log("Filling grid with random tiles.");

    int blockerCounter = 0;

    for (int x = 0; x < gridLength; x++) {
        for (int y = 0; y < gridLength; y++) {
            bool hasBlockerAdjacent = false; // Reset for each new cell
            Tile selectedShape = shapeTiles[Random.Range(0, shapeTiles.Length)]; // Select random shape
            Vector3Int position = new Vector3Int(x, y, 0); // Grid coordinates

            // Check adjacent cells for blockers
            for (int j = y - 1; j <= y + 1; j++) {
                for (int i = x - 1; i <= x + 1; i++) {
                    if (i == x && j == y) continue; // Skip the current cell itself
                    if (i < 0 || j < 0 || i >= gridLength || j >= gridLength) continue; // Boundary check
                    Vector3Int adjacentCell = new Vector3Int(i, j, 0);
                    Tile adjacentTile = shapeTilemap.GetTile<Tile>(adjacentCell);
                    if (adjacentTile != null && adjacentTile.name == "Blocker") {
                        hasBlockerAdjacent = true; //  there is an adjacent blocker
                        break;
                    }
                }
                if (hasBlockerAdjacent) break; // Break loop if a blocker is found
            }

            // Place the selected shape if it's not a blocker or if there are less than 5 blockers already 
            // and there's no blocker adjacent to this position
            if (selectedShape.name != "Blocker" || (selectedShape.name == "Blocker" && blockerCounter < 5 && !hasBlockerAdjacent)) 
            {
                shapeTilemap.SetTile(position, selectedShape); // place if non blocker or a capable blocker 
                Debug.Log($"Placing {selectedShape.name} at position {x}, {y}");
            }

            if (selectedShape.name == "Blocker" && hasBlockerAdjacent) // if an uncapable  blocker
            {
                Tile nonBlockerShape = shapeTiles[Random.Range(0, shapeTiles.Length - 1)]; // Select random non blocker shape 
                shapeTilemap.SetTile(position, nonBlockerShape); // replace non blocker with new one 
                Debug.Log($"Placing {nonBlockerShape.name} at position {x}, {y}");

            }
            
            if (selectedShape.name != "Blocker")
            {
                    // Power-up logic should only apply to non-blocker tiles
                    int isRandom = Random.Range(1, 12);
                    if (isRandom == 6) {
                        Tile selectedPowerUp = powerUpTiles[Random.Range(0, powerUpTiles.Length)];
                        powerUpTilemap.SetTile(position, selectedPowerUp);
                        Debug.Log($"Placing {selectedPowerUp.name} power up at position {x}, {y}");
                    }
            }
                
            
        }
    }
}

private bool AreAdjacent(Vector3Int firstPosition, Vector3Int secondPosition)
{
    // Check if the two positions are next to each other in the grid
    return (Mathf.Abs(firstPosition.x - secondPosition.x) == 1 && firstPosition.y == secondPosition.y) ||
            (Mathf.Abs(firstPosition.y - secondPosition.y) == 1 && firstPosition.x == secondPosition.x);
}

private void SwapTiles(Vector3Int firstPosition, Vector3Int secondPosition)
{

    if (shapeTilemap == null) return;

    // Swap the tiles in the shape tilemap
    Tile firstTile = shapeTilemap.GetTile<Tile>(firstPosition);
    Tile secondTile = shapeTilemap.GetTile<Tile>(secondPosition);
    if (firstTile == null || secondTile == null) return;
    shapeTilemap.SetTile(firstPosition, secondTile);
    shapeTilemap.SetTile(secondPosition, firstTile);
    shapeTilemap.RefreshAllTiles();

    // Swap the tiles in the power up tilemap
    Tile firstTilePower = powerUpTilemap.GetTile<Tile>(firstPosition);
    Tile secondTilePower = powerUpTilemap.GetTile<Tile>(secondPosition);
    powerUpTilemap.SetTile(firstPosition, secondTilePower);
    powerUpTilemap.SetTile(secondPosition, firstTilePower);
    powerUpTilemap.RefreshAllTiles();

    // Debug statement for swapping
    Debug.Log($"Swapping tiles: First tile at {firstPosition} of type {firstTile.name}, Second tile at {secondPosition} of type {secondTile.name}");
    Tile postSwapFirstTile = shapeTilemap.GetTile<Tile>(firstPosition);
    Tile postSwapSecondTile = shapeTilemap.GetTile<Tile>(secondPosition);
    Debug.Log($"After swap: Tile at {firstPosition} is now {postSwapFirstTile.name}, Tile at {secondPosition} is now {postSwapSecondTile.name}");

    // Check if the swap actually occurred
    if (postSwapFirstTile == secondTile && postSwapSecondTile == firstTile)
    {
        Debug.Log("Swap done!");
        swapsRemaining--;
    }
    else
    {
        Debug.Log("Swap failed!");
    }

}

public void UpdateSwapsRemaining()
{
    if (swapsToText != null)
    {
        swapsToText.text = "Remaining Swaps: " + swapsRemaining;
    }
}

private bool OutOfBounds(Vector3Int selectedTile){
    bool result = false;

    if (selectedTile.x < 0 || selectedTile.x > gridLength - 1) {
        result = true;
    }
        if (selectedTile.y < 0 || selectedTile.y > gridLength - 1) {
        result = true;
    }

    return result;
}

private List<Vector3Int> CheckXaxis(Vector3Int targetTile)
{

    List<Vector3Int> possibleBreaks = new List<Vector3Int>{};
    Tile shape = shapeTilemap.GetTile<Tile>(targetTile);
    if (shape == null){
        return null;
    }
    possibleBreaks.Add(targetTile);

    for (int i = targetTile.x + 1; i < gridLength; i ++)
    {
        Vector3Int tileInRow = new Vector3Int(i,targetTile.y,0); // use tile for coordinates
        Tile shapeInRow = shapeTilemap.GetTile<Tile>(tileInRow); // use shape to check if they match
        
        if (shapeInRow == null || shape.name != shapeInRow.name) //if they dont match then break
        {
            break;
        }
        else {
            possibleBreaks.Add(tileInRow);
        }
    }
    for (int i = targetTile.x - 1; i >= 0; i--)
    {
        Vector3Int tileInRow = new Vector3Int(i,targetTile.y,0); 
        Tile shapeInRow = shapeTilemap.GetTile<Tile>(tileInRow);
        if (shapeInRow == null || shape.name != shapeInRow.name){
            break;
        }
        else {
            possibleBreaks.Add(tileInRow);
        }
    }    

if (possibleBreaks.Count < 3) return null;
else return possibleBreaks;

}

private List<Vector3Int> CheckYaxis(Vector3Int targetTile)
{
    List<Vector3Int> possibleBreaks = new List<Vector3Int>{};
    Tile shape = shapeTilemap.GetTile<Tile>(targetTile);
    if (shape == null){
        return null;
    }

    possibleBreaks.Add(targetTile);
    for (int i = targetTile.y + 1; i < gridLength; i ++)
    {
        Vector3Int tileInRow = new Vector3Int(targetTile.x,i,0); // use tile for coordinates
        Tile shapeInRow = shapeTilemap.GetTile<Tile>(tileInRow); // use shape to check if they match
        
        if ( shapeInRow == null || shape.name != shapeInRow.name) //if they dont match then break
        {
            break;
        }
        else {
            possibleBreaks.Add(tileInRow); // add tile to list
        }
    }
    for (int i = targetTile.y - 1; i >= 0; i--)
    {
        Vector3Int tileInRow = new Vector3Int(targetTile.x,i,0); 
        Tile shapeInRow = shapeTilemap.GetTile<Tile>(tileInRow);
        if (shapeInRow == null || shape.name != shapeInRow.name){
            break;
        }
        else {
            possibleBreaks.Add(tileInRow);
        }
    }    

if (possibleBreaks.Count < 3) return null;
else return possibleBreaks;
}

private List<Vector3Int> CheckSquare(Vector3Int targetTile)
{
    List<Vector3Int> possibleBreaks = new List<Vector3Int>{};
    Tile shape = shapeTilemap.GetTile<Tile>(targetTile);
        if (shape == null)
    {
        return null; // Return if the target tile is null
    }
    possibleBreaks.Add(targetTile);

    bool hasSquare = false;
    
    // Define all adjacent positions
    Vector3Int[] adjacentPositions = new Vector3Int[]
    {
        new Vector3Int(targetTile.x, targetTile.y + 1, 0), // up                 0
        new Vector3Int(targetTile.x + 1, targetTile.y + 1, 0), // upRight        1
        new Vector3Int(targetTile.x + 1, targetTile.y, 0), // right              2
        new Vector3Int(targetTile.x + 1, targetTile.y - 1, 0), // downRight      3
        new Vector3Int(targetTile.x, targetTile.y - 1, 0), // down               4
        new Vector3Int(targetTile.x - 1, targetTile.y - 1, 0), // downLeft       5
        new Vector3Int(targetTile.x - 1, targetTile.y, 0), // left               6
        new Vector3Int(targetTile.x - 1, targetTile.y + 1, 0) // upLeft          7
    };

    Tile[] adjacentTiles = new Tile[adjacentPositions.Length];
    for (int i = 0; i < adjacentPositions.Length; i++)
    {
        adjacentTiles[i] = shapeTilemap.GetTile<Tile>(adjacentPositions[i]);
    }


    if (adjacentTiles[0] != null && shape.name == adjacentTiles[0].name && 
    adjacentTiles[1] != null && adjacentTiles[0].name == adjacentTiles[1].name &&
    adjacentTiles[2] != null && adjacentTiles[1].name == adjacentTiles[2].name)
    {
        possibleBreaks.Add(adjacentPositions[0]); // up
        possibleBreaks.Add(adjacentPositions[1]); // upRight
        possibleBreaks.Add(adjacentPositions[2]); // right
        hasSquare = true;
    }



    else if (adjacentTiles[2] != null && shape.name == adjacentTiles[2].name && 
    adjacentTiles[3] != null && adjacentTiles[2].name == adjacentTiles[3].name &&
    adjacentTiles[4] != null && adjacentTiles[3].name == adjacentTiles[4].name)
    {
        possibleBreaks.Add(adjacentPositions[2]); // right
        possibleBreaks.Add(adjacentPositions[3]); // downRight
        possibleBreaks.Add(adjacentPositions[4]); // down
        hasSquare = true;
    }


    else if (adjacentTiles[4] != null && shape.name == adjacentTiles[4].name && 
    adjacentTiles[5] != null && adjacentTiles[4].name == adjacentTiles[5].name &&
    adjacentTiles[6] != null && adjacentTiles[5].name == adjacentTiles[6].name)
    {
        possibleBreaks.Add(adjacentPositions[4]); // down
        possibleBreaks.Add(adjacentPositions[5]); // downLeft
        possibleBreaks.Add(adjacentPositions[6]); // left
        hasSquare = true;
    }

    else if (adjacentTiles[5] != null && shape.name == adjacentTiles[5].name && 
    adjacentTiles[6] != null && adjacentTiles[5].name == adjacentTiles[6].name &&
    adjacentTiles[7] != null && adjacentTiles[6].name == adjacentTiles[7].name)
    {
        possibleBreaks.Add(adjacentPositions[6]); // left
        possibleBreaks.Add(adjacentPositions[7]); // upLeft
        possibleBreaks.Add(adjacentPositions[0]); // up
        hasSquare = true;
    }

    if (hasSquare) return possibleBreaks;
    else return null;

}


private List<Vector3Int> CheckMoves(Vector3Int targetTile)
{
    var xAxisBreakables = CheckXaxis(targetTile);
    var yAxisBreakables = CheckYaxis(targetTile);
    var squareBreakables = CheckSquare(targetTile);

    List<Vector3Int> largestBreakableSet = null;
    int largestCount = 0;

    if (xAxisBreakables != null && xAxisBreakables.Count > largestCount)
    {
        largestBreakableSet = xAxisBreakables;
        largestCount = xAxisBreakables.Count;
    }

    if (yAxisBreakables != null && yAxisBreakables.Count > largestCount)
    {
        largestBreakableSet = yAxisBreakables;
        largestCount = yAxisBreakables.Count;
    }

    if (squareBreakables != null && squareBreakables.Count > largestCount)
    {
        largestBreakableSet = squareBreakables;
    }

    return largestBreakableSet;
}

public void UpdateScore()
{
    if (scoreToText != null)
    {
        scoreToText.text = "Score: " + score;
    }
}


private List<Vector3Int> BombTiles(Vector3Int targetTile){

    bool isBombable = (CheckMoves(targetTile) != null); // is a breakable piece

    if (!isBombable) return null;

    List<Vector3Int> possibleBreaks = new List<Vector3Int>{};
    Tile shape = shapeTilemap.GetTile<Tile>(targetTile);
    Tile powerUp = powerUpTilemap.GetTile<Tile>(targetTile);

    if (shape == null || powerUp == null || powerUp.name == null || powerUp.name != "Bomb")
    {
        return null; // Return if the target tile is null
    }

    for (int yDiff = -1; yDiff <= 1; yDiff++){
        for (int xDiff = -1; xDiff <= 1; xDiff++){
            Vector3Int position = new Vector3Int(targetTile.x + xDiff, targetTile.y + yDiff, 0);
            if (position.x < 0 || position.x > 5 || position.y < 0 || position.y > 5) continue;
            Tile positionShape = shapeTilemap.GetTile<Tile>(position);
            if( positionShape!= null && positionShape.name != null && positionShape.name != "Blocker")
            {
                possibleBreaks.Add(position);
            }
        }
    }

    if (possibleBreaks.Count > 1) return possibleBreaks;
    else return null;
}

private IEnumerator BombLogic(Vector3Int targetTile){

    
    List<Vector3Int> bombableTiles = BombTiles(targetTile);
    if (bombableTiles != null)
    {
        score += (15 * bombableTiles.Count); // increase score
        for (int i = 0; i<bombableTiles.Count; i++){
            Vector3Int position = bombableTiles[i];
            yield return new WaitForSeconds(0.08f); //tiny delay
            Tile currentPowerUp = powerUpTilemap.GetTile<Tile>(position); 
            if(currentPowerUp == null)
            {
                shapeTilemap.SetTile(bombableTiles[i],null);    
            }
            else if (currentPowerUp.name == "Bomb" && currentPowerUp.name != null)
             {
                shapeTilemap.SetTile(bombableTiles[i],null);
                powerUpTilemap.SetTile(bombableTiles[i],null);
                
                StartCoroutine(BombLogic(position));
             }
            else if (currentPowerUp.name == "ColourBomb" && currentPowerUp.name != null)
            {
                shapeTilemap.SetTile(bombableTiles[i],null);
                powerUpTilemap.SetTile(bombableTiles[i],null);
                ColourBombLogic(position);
            }
            else if (currentPowerUp.name == "Concretion" && currentPowerUp.name != null) ConcretionLogic(position);
            
        }
    }

        shapeTilemap.RefreshAllTiles();
        powerUpTilemap.RefreshAllTiles();
}

private void ConcretionLogic(Vector3Int position){
    Debug.Log("Concretion starting");
    Tile concreteTile = powerUpTilemap.GetTile<Tile>(position);
    Debug.Log($" power up is: {concreteTile.name}");

    Tile blocker = shapeTiles[shapeTiles.Length - 1]; // get blocker tile
    Debug.Log($" name of new tile is: {blocker.name}");
    if (concreteTile != null && concreteTile.name != null && concreteTile.name == "Concretion")
    {

            powerUpTilemap.SetTile(position, null); // set concretion power up to null
            shapeTilemap.SetTile(position, null); // set tile on shape tilemap to null
            shapeTilemap.SetTile(position, blocker); // set tile on shape tilemap to blocker
            shapeTilemap.RefreshAllTiles();
            Debug.Log("Concretion complete"); 

    }
}

private List<Vector3Int> TilesOfColour(Vector3Int targetTile) 
{
    Tile targetColour = shapeTilemap.GetTile<Tile>(targetTile);
    Tile powerUp = powerUpTilemap.GetTile<Tile>(targetTile);
    List<Vector3Int> sameColours = new List<Vector3Int>{};
    if (targetColour == null || powerUp == null || powerUp.name == null || powerUp.name != "ColourBomb")
    {
        return null;
    }
    else
    {
    for (int j = 0; j < gridLength; j++)
    {
        for (int i = 0; i < gridLength; i++)
        {
            Vector3Int currentPos = new Vector3Int(i, j, 0);
            Tile currentColour = shapeTilemap.GetTile<Tile>(currentPos);

            if (currentColour != null && targetColour.name == currentColour.name) {
                 sameColours.Add(currentPos);
            }
        }
    }
    return sameColours;
    }
}

private void ColourBombLogic(Vector3Int targetTile)
{
    List<Vector3Int> allOfColour = TilesOfColour(targetTile);
    if (allOfColour == null) {
        Debug.Log("TilesOfColour is not working or this does not have a colour bomb");
        return;
    }
    else
    {

        score += (15 * allOfColour.Count); // increase score
        for (int i = 0; i < allOfColour.Count; i++)
        {
            Vector3Int currentPos = allOfColour[i];
            Tile currentPowerUp = powerUpTilemap.GetTile<Tile>(currentPos);
            Tile currentColour = shapeTilemap.GetTile<Tile>(currentPos);
            if(currentPowerUp == null)
            {
                shapeTilemap.SetTile(currentPos,null);    
            }
            
            else if (currentPowerUp.name == "Bomb" && currentPowerUp.name != null){
                shapeTilemap.SetTile(currentPos,null);
                powerUpTilemap.SetTile(allOfColour[i],null); // Remove the power-up to avoid recursion
                StartCoroutine(BombLogic(currentPos)); 
            }            
            else if (currentPowerUp.name == "ColourBomb" && currentPowerUp.name != null){
                powerUpTilemap.SetTile(allOfColour[i],null);
                ColourBombLogic(currentPos);
            } 
            else if (currentPowerUp.name == "Concretion" && currentPowerUp.name != null) ConcretionLogic(currentPos);
            
        }
    }

}


private IEnumerator RemoveTiles(Vector3Int targetTile)
{
    List<Vector3Int> possibleBreaks = new List<Vector3Int>{};

    possibleBreaks = CheckMoves(targetTile);

    // I need bombs to chain react so need to cross check all possible breaks before bombLogic

    yield return new WaitForSeconds(0.08f);

    if (possibleBreaks != null){

        for(int i = 0; i < possibleBreaks.Count; i++){
            Vector3Int position = possibleBreaks[i];
            Tile currentTile = powerUpTilemap.GetTile<Tile>(position);
            if (currentTile != null && currentTile.name == "Bomb") StartCoroutine(BombLogic(position));
            if (currentTile != null && currentTile.name == "ColourBomb") ColourBombLogic(position);
            if (currentTile != null && currentTile.name == "Concretion") ConcretionLogic(position);

        }
        score += possibleBreaks.Count * 15;

        for(int i = 0; i < possibleBreaks.Count; i++){
            shapeTilemap.SetTile(possibleBreaks[i],null);
            powerUpTilemap.SetTile(possibleBreaks[i],null);
        }
    firstTile = null;
    }

    UpdateScore();
}


private void refillTopLayer(){
    for (int x = 0; x< gridLength; x++)
    {
        for (int y = gridLength - 1; y < gridLength; y++)
        {
        Vector3Int position = new Vector3Int(x,y,0);
        Tile currentTile = shapeTilemap.GetTile<Tile>(position);
        if (currentTile == null) 
        {
            Tile randomShape = this.shapeTiles[Random.Range(0, shapeTiles.Length-1)]; // Select random tile from palette except blocker
            shapeTilemap.SetTile(position, randomShape);
            if (randomShape.name == "Blocker") continue;
            int isRandom = Random.Range(1,12); // generate a random number between 1 and 12.  /* 6 means it isRandom */   
            if( isRandom == 6)
            {
               Tile randomPowerUp = this.powerUpTiles[Random.Range(0,powerUpTiles.Length)];
               powerUpTilemap.SetTile(position, randomPowerUp);
            }        
            
        }
    }
}
}

private bool isFragile(Vector3Int selectedTile)
{
    Tile fragileTile = powerUpTilemap.GetTile<Tile>(selectedTile);
    Tile fragileShape = shapeTilemap.GetTile<Tile>(selectedTile);

    bool hasFragility = false;

    if (fragileShape == null || fragileTile == null || fragileShape.name == null || fragileTile.name == null)
    {
        return false;
    }

    if (fragileTile.name == "Fragile")
    {
        hasFragility = true;
    }


    return hasFragility;
}

private IEnumerator SandMechanics(){
    bool tilesMoved;
    do{
        tilesMoved = false;
        for(int y = 1; y < gridLength; y++){ // 1 because we don't need to shift the bottom row
            for (int x = 0; x < gridLength; x++) 
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                yield return new WaitForSeconds(0.08f);
                if (isBlocker(position)) continue; // continue to next tile

                Vector3Int sandD = new Vector3Int(x, y - 1, 0);
                Tile currentTile = shapeTilemap.GetTile<Tile>(position);
                Tile currentTilePowerUp = powerUpTilemap.GetTile<Tile>(position);

                if (currentTile != null && shapeTilemap.GetTile<Tile>(sandD) == null 
                    && sandD.y >= 0)
                {
                    shapeTilemap.SetTile(sandD, currentTile);
                    powerUpTilemap.SetTile(sandD, currentTilePowerUp);
                    shapeTilemap.SetTile(position, null);
                    powerUpTilemap.SetTile(position, null);

                    // Check if the moved tile is fragile and should break
                    if(isFragile(sandD))
                    {
                        shapeTilemap.SetTile(sandD, null);  
                        powerUpTilemap.SetTile(sandD, null);
                    }

                    tilesMoved = true;
                }  
            }
        }
    } while(tilesMoved); // repeat if any tile moved

    refillTopLayer();
}



private bool isBlocker(Vector3Int selectedCell)
{
    Tile selectedTile = shapeTilemap.GetTile<Tile>(selectedCell);
    if (selectedTile == null) 
    {
        return false; 
    }
    else
    {
        return selectedTile.name == "Blocker";
    }    
}

private bool hasSwaps()
{
    if (swapsRemaining < 1) return false;
    else return true;
}

private bool hasWon()
{
    if(hasSwaps() && score >= 500){
        return true;
    }
    else return false;
}

private void playStateLogic(){

    pauseMenu.SetActive(false);

    if (!hasSwaps())
    {
        currentState = GameState.GameOver;
    }

    if(hasWon())
    {
        currentState = GameState.LevelComplete;
    }

    StartCoroutine(SandMechanics());

    if (Input.GetMouseButtonDown(0)) // Left mouse button
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // takes x and y coordinates of the mouse position
        Vector3Int clickedShape = shapeTilemap.WorldToCell(mousePosition); //converts 2d vector into vector 3 
        
        // if button then pause
            
        if (!OutOfBounds(clickedShape) && isBlocker(clickedShape) == false ) // this becomes an else if
        {
            if (firstTile == null)
            {
                firstTile = clickedShape;
                Debug.Log($"First Tile Selected at {firstTile}");
                Debug.Log($"{CheckMoves(clickedShape)}");
                StartCoroutine(RemoveTiles(clickedShape));
            }
            else
            {
                secondTile = clickedShape;
                Debug.Log($"Second Tile Selected at {secondTile}");
                    

                if (AreAdjacent(firstTile.Value, secondTile.Value))
                {
                    SwapTiles(firstTile.Value, secondTile.Value);
                    UpdateSwapsRemaining();
                    firstTile = null; // Reset the positions
                    secondTile = null;
                    Debug.Log("Tiles have swapped!");
                }
                else
                {
                    // Not adjacent
                    firstTile = null;
                    secondTile = null;
                    Debug.Log("Tiles are not adjacent!");
    
                }
            }
        }
    }
}

public void pauseGame()
{
    CloseMenus();
    currentState = GameState.Pause;
}

public void resumeGame()
{
    CloseMenus();
    currentState = GameState.Play;
}

private void levelCompleteLogic(){

    CloseMenus();
    levelCompleteMenu.SetActive(true);
    //Display Level Complete and swaps remaining +
    // button for next level or home page
}

private void pauseStateLogic(){
CloseMenus();
pauseMenu.SetActive(true);

}

public void rulesLogic()
{
    
    CloseMenus();
    rulesMenu.SetActive(true);
    pauseState = 1;
}


public void returnToPause(){
    
    CloseMenus();
    pauseMenu.SetActive(true);
    pauseState = 0;
    
}



public void QuitGame(){
    Application.Quit();
}

private void gameOverLogic(){
    CloseMenus();
    gameOverScript.SetActive(true);
}

    public void loadScene(int scene)
    {
        score = 0;
        swapsRemaining = 20;
        gameOverScript.SetActive(false); // get rid of game over menu if restarting level
        Debug.Log("Button pressed");
        if (scene == HOME) 
        { 
            SceneManager.LoadSceneAsync(HOME); // load the home menu

        }
        if (scene == LEVEL_1)
        {
            gridLength = 6;
            SceneManager.LoadSceneAsync(LEVEL_1); // load the first level
            InitialiseTiles(); 
        }
        if (scene == LEVEL_2) 
        {
            gridLength = 8;
            SceneManager.LoadSceneAsync(LEVEL_2); // load the second level
            InitialiseTiles(); 
        }
    }

    public void CloseMenus()
    {
        pauseMenu.SetActive(false);
        rulesMenu.SetActive(false);
        gameOverScript.SetActive(false);
        levelCompleteMenu.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        CloseMenus();
        Debug.Log("Start called. ");
        currentScene = SceneManager.GetActiveScene(); //get current scene
        if (currentScene.name == "Level1"){
            gridLength = 5;
            InitialiseTiles();
        }
        if (currentScene.name == "Level2"){
            gridLength = 7;
            InitialiseTiles();
        }          
    }

    // Update is called once per frame
    public void Update()
    {
    
        if (currentState == GameState.Play) playStateLogic();
        if (currentState == GameState.Pause) {
            if (pauseState == 0) pauseStateLogic();
            if (pauseState == 1) rulesLogic();
            
        }
        if (currentState == GameState.LevelComplete) levelCompleteLogic();
        if (currentState == GameState.GameOver) gameOverLogic();
    }
}
