/* 
 * File Name: GameController.cs
 * Description: Main script for the demo.
 */ 


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace SD {
    public class GameController : MonoBehaviour {

        public GameObject Prey;
        public Vector3 spawnValue;
        public GameObject[] ingameNPCFishPrefabsArray;
        public GameObject bubbles;

        // Number of prey spawn at a game start
        public int numPrey;

        public Text scoreText;
        public Text UnscoredPointText; 
        public Text staminaText;
        public Text healthText;
        public Text opponentScoreText;

        private int score;          // Player's score
        private int unscoredPoint;
        private int opponentScore;
        public float stamina;       // Player's stamina
        private float maxStamina;
        public int health;          // Player's health
        private float maxHealth;
        private float staminaRecoveryRate = 10f;
        private float staminaRecoveryDelay;
        private float staminaBeginRecoverTime = 0.0f;

        public Boundary boundary;
        public List<Rigidbody> playerPrefabs;
        public Rigidbody opponent;
        public Rigidbody playerBase;
        public Rigidbody opponentBase;
        private Vector3 playerInitialPosition = new Vector3(-100,0,0);
        private Quaternion playerInitialRotation = Quaternion.Euler(0,0,0);
        //private Quaternion playerInitialRotation = Quaternion.Euler(0, 90, 0); // Default old values.
        private Vector3 opponentInitialPosition = new Vector3 (100, 0, 0);
        //private Quaternion opponentInitialRotation = Quaternion.Euler (0, -90, 0); // Default old values.
        private Quaternion opponentInitialRotation = Quaternion.Euler(0, 0, 0);
        private Vector3 playerBaseInitialPosition = new Vector3(-260,0,0);
        private Quaternion playerBaseInitialRotation = Quaternion.Euler(0,0,0);
        private Vector3 opponentBaseInitialPosition = new Vector3(260,0,0);
        private Quaternion opponentBaseInitialRotation = Quaternion.Euler(0,0,0);

        private static GameController gameController;
        private static GameManager sdGameManager;
        private PlayTimePlayer currentPlayer;
        private PlayTimePlayer opponentPlayer;
        private Rigidbody rbOpponent;
        private Dictionary <int, NPCFish> npcFishes = new Dictionary<int, NPCFish>();
        private Dictionary <int, GameObject> npcFishObjects = new Dictionary<int, GameObject>();
        private int maxPreyId;
        private bool hasSurrendered;
        private bool isGameTimeTicking = false;
        private bool isPlayerInBase = false;
        private bool isOpponentInBase = false;

        public GameObject surrenderPanelCanvas;
        public GameObject baseScorePanelCanvas;
        public GameObject countdownPanelCanvas;
        public GameObject foodChainPanelCanvas;
        private int foodChainPanelVisibleSeconds = 7;
        private PlayTimePlayer targetPlayer;
        public GameObject deathPanelCanvas;

        // BUFFS, BONUSES, AND POWERUPS.
        // Determines if power-ups are active.
        private bool pointBoostOn = false;
        private bool speedBoostOn = false;
        private bool evasionBoostOn = false;
        private bool slowDownOn = false;
        private int pointBonus = 0;

        // Prey fish count and max.
        private int preyFishRemaining = 0;
        private int preyFishTotal = 0;

        // The fade in and out transitions for the scene.
        public Animator fadeOutAnimator;
        public Animator fadeInAnimator;

        Rigidbody playerClone;

        void Awake () {
            gameController = this;
        }
        // Initializes the player's score, and UI texts.
        // Also spawns numbers of prey at random positions.
        void Start () {
            
            if (Constants.PLAYER_NUMBER == 2) {  // The player who joins the host will have a different position to start from.
                swapPositions();
            }
            // Spawn the appropriate player based on the character selection screen.
            if (FindObjectOfType<SDPersistentData>() != null)
            {
                Debug.Log("Player fish index: " + FindObjectOfType<SDPersistentData>().GetPlayerFishSelectionIndex());
                playerClone = Instantiate(playerPrefabs[FindObjectOfType<SDPersistentData>().GetPlayerFishSelectionIndex()], playerInitialPosition, playerInitialRotation);
            }
            else
            {
                playerClone = Instantiate(playerPrefabs[0], playerInitialPosition, playerInitialRotation);
            }
            Rigidbody playerBaseClone = Instantiate (playerBase, playerBaseInitialPosition, playerBaseInitialRotation);
            Rigidbody opponentBaseClone = Instantiate (opponentBase, opponentBaseInitialPosition, opponentBaseInitialRotation);
            score = 0;
            hasSurrendered = false;
            opponentScore = 0;
            unscoredPoint = 0; 
            UpdateScoreText ();
            UpdateOpponentScoreText ();
            UpdateStaminaText ();
            UpdateUnscoredPointText ();
            UpdateHealthText ();

            sdGameManager = SD.GameManager.getInstance ();
            currentPlayer = new PlayTimePlayer ();

            for (int i = 1; i <= numPrey; i++) {
                NPCFish npcFish = new NPCFish (i);
                npcFishes [i] = npcFish;
                maxPreyId = i;
                if (SDMain.networkManager != null) {
                    sdGameManager.FindNPCFishPosition (i); // Finds and spawns prey at the returned location.
                } else {
                    int randomFishIndex = Random.Range(0, ingameNPCFishPrefabsArray.Length);
                    //Debug.Log("Spawning fish with index id: " + randomFishIndex);
                    spawnPrey (i, randomFishIndex);
                }
                
                // After spawning the fish, get the number of prey fish.
                // Prey fish are fish that the players are able to consume.
                if(npcFishObjects[i].tag == "PlayerPrey" ||
                    npcFishObjects[i].tag == "SpeedBuffFish" ||
                    npcFishObjects[i].tag == "PointBuffFish")
                {
                    preyFishTotal++;
                }
            }
            // Initialize remaining to total.
            preyFishRemaining = preyFishTotal;

            //Debug.Log("Prey fish to start with: " + preyFishTotal);

            if (SDMain.networkManager != null) {  // We are playing multiplayer
                rbOpponent = (Rigidbody)Instantiate (opponent, opponentInitialPosition, opponentInitialRotation);
                rbOpponent.gameObject.SetActive (true);
                opponentPlayer = new PlayTimePlayer ();
                opponentPlayer.speedUpFactor = playerClone.GetComponent<PlayerController>().staminaSpeedBoostFactor;
                opponentPlayer.yRotation = opponentInitialRotation.eulerAngles.y;
                isGameTimeTicking = false; // Wait for time sync if in multiplayer mode
                gameController.countdownPanelCanvas.SetActive (true);
            } else {
                isGameTimeTicking = true; // Start the timer immediately if in offline mode
                gameController.countdownPanelCanvas.SetActive (false);
            }

            // Spawn the predator: 1 of type 8
            //spawnNpcSet(8, 1);
            //Display the food chain panel for n seconds upon game start
            StartCoroutine(showFoodChainUponStart(foodChainPanelVisibleSeconds));
            // Find the audio mixer and ask it to fade in.
            StartCoroutine(FindObjectOfType<MainMixerController>().FadeInAudio());
            // Finally, fade in the screen.
            fadeInAnimator.SetTrigger("FadeIn");
        }

      
        /// <summary>
        /// Shows the food chain upon start.
        /// </summary>
        /// <param name="seconds">Duration to show the food chain panel</param>
        IEnumerator showFoodChainUponStart(int seconds) {
            yield return new WaitForSeconds (seconds);
            hideFoodChainPanel ();
        }
        // Automatically recovers stamina, and refreshs staminaText UI every frame.
        void Update() {
            if (getIsGameTimeTicking ()) {
                if (Constants.PLAYER_NUMBER != 2)
                {  // The player who joins the host will have a different position to start from.
                    StartCoroutine(RetargetFish()); // TODO: Move coroutine out of Update() into a while loop
                }
                RecoverStamina ();
                UpdateStaminaText ();
                UpdateUnscoredPointText ();
                UpdateOpponentScoreText ();
                UpdateHealthText ();
                if (health <= 0) {
                    deathPanelCanvas.SetActive (true);
                    this.health = 0;
                    // Find the audio mixer and ask it to fade out.
                    StartCoroutine(FindObjectOfType<MainMixerController>().FadeOutAudio());
                    // Fade out the screen.
                    fadeOutAnimator.SetTrigger("FadeOut");
                    StartCoroutine (goToResultScene ());
                    playerClone.transform.localScale = new Vector3 (0, 0, 0);
                }
            }
        }

        /// <summary>
        /// This is invoked by Update() when the player's health gets 0
        /// and automaticaly ends the game
        /// </summary>
        IEnumerator goToResultScene(){
            yield return new WaitForSeconds (3);
            BtnSurrenderClick ();
        }


        public static GameController getInstance() {
            return gameController;
        }

        // Swaps the positions and rotations of the players and bases for the opponent's view.
        private void swapPositions() {
            Vector3 tempV = playerInitialPosition;
            playerInitialPosition = opponentInitialPosition;
            opponentInitialPosition = tempV;

            tempV = playerBaseInitialPosition;
            playerBaseInitialPosition = opponentBaseInitialPosition;
            opponentBaseInitialPosition = tempV;

            Quaternion tempQ = playerInitialRotation;
            playerInitialRotation = opponentInitialRotation;
            opponentInitialRotation = tempQ;

            tempQ = playerBaseInitialRotation;
            playerBaseInitialRotation = opponentBaseInitialRotation;
            opponentBaseInitialRotation = playerBaseInitialRotation;
        }

        // Spawns prey at a random position within the boundary
        public void spawnPrey(int i, int preyIndex){
            Vector3 spawnPosition;
            if (npcFishes [i].xPosition != 0 && npcFishes [i].yPosition != 0) {
                spawnPosition = new Vector3 (npcFishes [i].xPosition, npcFishes [i].yPosition, npcFishes[i].xRotationAngle);
                //Debug.Log ("Spawning NPCFish " + i + " from request result");
            } else {
                spawnPosition = new Vector3 (Random.Range(boundary.xMin, boundary.xMax), Random.Range(boundary.yMin, boundary.yMax), 0);
                //Debug.Log ("Spawning NPCFish " + i + " from local random numbers");
            }
            Quaternion spawnRotation = Quaternion.Euler(0, 90,0);
            Debug.Log("Insantiating fish from index: " + preyIndex);
            npcFishObjects [i] = Instantiate (ingameNPCFishPrefabsArray[preyIndex], spawnPosition, spawnRotation) as GameObject;
            npcFishObjects [i].name = "NPCFish_" + preyIndex + "_" + i;
            npcFishObjects [i].SetActive (true);
            // Associate the metadata of the prey with the gameobject.
            npcFishObjects[i].GetComponent<NPCFishController>().setNPCFishData(npcFishes[i]);
        }

        public void destroyPrey(int i) {

            // Displays bubbles when destroying prey
            Instantiate (bubbles, playerClone.transform.position, Quaternion.identity);

            // Modify the clone to your heart's content
            if (npcFishObjects [i] != null) {
                Destroy (npcFishObjects [i]);
                // Reduce the count of remaining prey fish.
                ReducePreyFishRemaining();
                npcFishes [i].isAlive = false;
            }
        }

        // Spawns 'num' Npc fish of type 'speciesId'
        public void spawnNpcSet(int speciesId, int num) {
            int startId = maxPreyId + 1;
            for (int i = startId; i < (startId + num); i++) {
                Vector3 spawnPosition;
                NPCFish npcFish = new NPCFish (i);
                npcFishes [i] = npcFish;
                maxPreyId = i;
                // TODO: Change random position to out-of-screen position once the movement is added.
                spawnPosition = new Vector3 (Random.Range(boundary.xMin, boundary.xMax), Random.Range(boundary.yMin, boundary.yMax), 0);
                // set the attributes of the npc fish to spawn.
                npcFish.xPosition = spawnPosition.x;
                npcFish.yPosition = spawnPosition.y;
                npcFish.target = new Vector2 (npcFish.xPosition, npcFish.yPosition);
                npcFish.speciesId = speciesId;
                spawnPrey (i, speciesId);
            }
            if (SDMain.networkManager != null) 
                sdGameManager.SendNpcFishPositions (5);  // Player 1 will send its positions to Player 2
        }

        IEnumerator RetargetFish()
        {
            targetFish();
            if (SD.SDMain.networkManager != null)
                sdGameManager.SendNpcFishPositions (5);
            yield return new WaitForSeconds(2);
            yield return null; // prevents it from hanging ?
        }

        public void targetFish()
        {
            Dictionary <int, NPCFish> npcs = getNpcFishes();
            foreach (KeyValuePair<int, NPCFish> entry in npcs) {
                if (!getNpcFishObjects ().ContainsKey (entry.Key))
                    continue;
                if (entry.Value.isAlive && getNpcFishObjects()[entry.Key].tag == "Predator") {
                    // calculate the distance to check if the player lies within the danger zone.
                    float distanceFromPlayer = Vector2.Distance (new Vector2 (getCurrentPlayer ().xPosition, getCurrentPlayer ().yPosition), new Vector2 (entry.Value.xPosition, entry.Value.yPosition));
                    float distanceFromOpponent = float.MaxValue;
                    if (sdGameManager.getIsMultiplayer ()) {
                        distanceFromOpponent = Vector2.Distance (new Vector2 (getOpponentPlayer ().xPosition, getOpponentPlayer ().yPosition), new Vector2 (entry.Value.xPosition, entry.Value.yPosition));
                    }
   
                    if (distanceFromPlayer < SD.Constants.PREDATOR_SAFE_DISTANCE || distanceFromOpponent < SD.Constants.PREDATOR_SAFE_DISTANCE) {
                        if (distanceFromPlayer > distanceFromOpponent) {
                            // If opponent is in base, do not follow it, otherwise continue to pursue.
                            if (getIsOpponentInBase ()) {
                                getNpcFishes () [entry.Key].target = new Vector2 (entry.Value.xPosition + entry.Value.targetOffset, entry.Value.yPosition);
                                getNpcFishes () [entry.Key].isAttacking = false;
                            } else {
                                getNpcFishes () [entry.Key].target = new Vector2 (getOpponentPlayer ().xPosition, getOpponentPlayer ().yPosition);
                                targetPlayer = getOpponentPlayer ();
                            }
                        } else {
                            // If player is in base, do not follow it, otherwise continue to pursue.
                            if (getIsPlayerInBase ()) {
                                getNpcFishes () [entry.Key].target = new Vector2 (entry.Value.xPosition + entry.Value.targetOffset, entry.Value.yPosition);
                                getNpcFishes () [entry.Key].isAttacking = false;
                            } else {
                                getNpcFishes () [entry.Key].target = new Vector2 (getCurrentPlayer ().xPosition, getCurrentPlayer ().yPosition);
                                targetPlayer = getCurrentPlayer ();
                            }
                        }
                        getNpcFishes () [entry.Key].isAttacking = true;
                    } else {
                        getNpcFishes () [entry.Key].target = new Vector2 (entry.Value.xPosition + entry.Value.targetOffset, entry.Value.yPosition);
                        getNpcFishes () [entry.Key].isAttacking = false;
                    }
                } else {
                    getNpcFishes () [entry.Key].target = new Vector2 (entry.Value.xPosition + entry.Value.targetOffset, entry.Value.yPosition);
                }
            }
        }

        // Increases the current score value, and pass the info to scoreText
        // by calling UpdateScore().
        public void AddScore(int newScoreValue) {
            score += newScoreValue;
            UpdateScoreText ();
            // Send the score to the opponent.
            sdGameManager.SendScoreToOpponent(score);
        }

        // Updates scoreText UI.
        void UpdateScoreText () {
            scoreText.text = "Score: " + score;
        }

        public void AddUnscoredPoint(int newScoreValue) {
            unscoredPoint += newScoreValue + pointBonus;
            UpdateUnscoredPointText ();
        }

        // Updates UnsscoreText UI.
        void UpdateUnscoredPointText() {
            UnscoredPointText.text = "Unscored Point: " + unscoredPoint;
        }

        void UpdateOpponentScoreText() {
            opponentScoreText.text = "Opponent: " + opponentScore;
        }

        public int GetUnscored(){
            return this.unscoredPoint;
        }

        public void ResetUnscored(){
            this.unscoredPoint = 0;
        }

        // Updates staminaText UI with no decimal place.
        void UpdateStaminaText() {
            staminaText.text = "Stamina: " + stamina.ToString("F0");
        }

        // Recovers the current stamina 
        void RecoverStamina(){
            if(Time.fixedTime > staminaBeginRecoverTime)
            {
                stamina = stamina + staminaRecoveryRate * Time.deltaTime;
                if (stamina >= maxStamina)
                    stamina = maxStamina;
            }
        }

        // Set the maximum amount of stamina for the player.
        public void SetMaxStamina(float maxStam)
        {
            maxStamina = maxStam;
        }

        public void SetStaminaDelay(float srd)
        {
            staminaRecoveryDelay = srd;
        }

        // Returns the current stamina
        public float GetStamina(){
            return this.stamina;
        }

        // Sets stamina
        public void SetStamina(float newStamina){
            staminaBeginRecoverTime = Time.fixedTime + staminaRecoveryDelay;
            stamina = newStamina;
            if (stamina < 0.0f)
                stamina = 0.0f;
        }

        // Sets score
        public void SetScore(int newScore){
            this.score = newScore;
        }

        // Adds unscored points to player's actual score
        public void Score(){
            this.score += this.unscoredPoint;
            UpdateScoreText ();
            // Send the score to the opponent.
            if (this.unscoredPoint != 0)  // to send the request only once.
                sdGameManager.SendScoreToOpponent(score);
        }

        public int GetHealth(){
            return this.health;
        }

        public void SetHealth(int newHealth){
            this.health = newHealth;
        }

        // Set the maximum amount of health for the player.
        public void SetMaxHealth(float maxHP)
        {
            maxHealth = maxHP;
        }

        public void UpdateHealth(int value){
            this.health = health + value;
        }

        public void UpdateHealthText(){
            healthText.text = "Health: " + health;
        }

        public void BtnSurrenderClick() {
            hasSurrendered = true;
            sdGameManager.EndGame (false, score);
        }

        public Rigidbody getOpponent() {
                return rbOpponent;
        }

        public PlayTimePlayer getCurrentPlayer() {
            return currentPlayer;
        }
        public PlayTimePlayer getOpponentPlayer() {
            return opponentPlayer;
        }

        public void setNpcFish(int i, NPCFish fish) {
            npcFishes [i] = fish;
        }

        public Dictionary<int, NPCFish> getNpcFishes() {
            return npcFishes;
        }

        public Dictionary<int, GameObject> getNpcFishObjects() {
            return npcFishObjects;
        }

        // Return the initial number of prey fish spawned.
        public int GetPreyFishTotal()
        {
            return preyFishTotal;
        }

        // Return the number of prey fish remaining in the scene.
        public int GetPreyFishRemaining()
        {
            return preyFishRemaining;
        }

        // Reduce the number of prey fish remaining by 1.
        public void ReducePreyFishRemaining()
        {
            preyFishRemaining--;
            //Debug.Log("Prey fish remaining: " + preyFishRemaining + " out of: " + preyFishTotal);
        }

        public void showCountdownPanel(){
            countdownPanelCanvas.SetActive (true);
        }

        public void hideCountdownPanel(){
            countdownPanelCanvas.SetActive (false);
        }

        public void showBaseScorePanel(){
            baseScorePanelCanvas.SetActive (true);
        }

        public void hideBaseScorePanel(){
            baseScorePanelCanvas.SetActive (false);
        }

        public void hideSurrenderPanel(){
            surrenderPanelCanvas.SetActive(false);
        }

        public void hideFoodChainPanel(){
            foodChainPanelCanvas.SetActive(false);
        }

        public int getPlayerScore() {
            return score;
        }

        public bool getHasSurrendered() {
            return hasSurrendered;
        }

        public void setOpponentScore(int opScore) {
            opponentScore = opScore;
        }

        public int getOpponentScore() {
            return opponentScore;
        }

        public bool getIsGameTimeTicking() {
            return isGameTimeTicking;
        }

        public void setIsGameTimeTicking(bool isTicking) {
            isGameTimeTicking = isTicking;
        }

        public int getMaxPreyId() {
            return maxPreyId;
        }

        public void setTargetPlayer(PlayTimePlayer p) {
            targetPlayer = p;
        }

        public PlayTimePlayer getTargetPlayer() {
            return targetPlayer;
        }

        public void setIsPlayerInBase(bool isInBase) {
            isPlayerInBase = isInBase;
        }

        public bool getIsPlayerInBase() {
            return isPlayerInBase;
        }

        public void setIsOpponentInBase(bool isInBase) {
            isOpponentInBase = isInBase;
        }

        public bool getIsOpponentInBase() {
            return isOpponentInBase;
        }

        public void SetIsPointBuffActive(bool active)
        {
            pointBoostOn = active;
        }

        public void SetIsSpeedBuffActive(bool active)
        {
            speedBoostOn = active;
        }

        public void SetIsEvasionBuffActive(bool active)
        {
            evasionBoostOn = active;
        }

        public void SetIsSlowDownActive(bool active)
        {
            slowDownOn = active;
        }

        public bool getPointBoostStatus()
        {
            return pointBoostOn;
        }

        public bool getSpeedBoostStatus()
        {
            return speedBoostOn;
        }

        public bool getSlowDownStatus()
        {
            return slowDownOn;
        }

        public bool getEvasionBoostStatus()
        {
            return evasionBoostOn;
        }

        public void SetPointBonusAmount(int bonus)
        {
            pointBonus = bonus;
        }
    } 

}
