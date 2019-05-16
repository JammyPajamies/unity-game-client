using UnityEngine;
using System.Collections;
using System;

namespace SD
{
    public class OpponentController : MonoBehaviour
    {


        private GameManager sdGameManager;
        private GameController sdGameController;
        private Rigidbody rbOpponent;
        /*
        private float xPosition;
        private float yPosition;
        private float xRotation;
        private float yAngle;
        private float yRotation;
        private float turnSpeed = 10f;
        private Vector3 turn;
        */

        // Base values. Should match values from the player versions.
        public float forwardAcceleration;
        public float baseMaxSpeed;
        public float absoluteMaxSpeedLimit;

        // The max rotation rate of the opponent.
        // Should be modified match the values of the player equivalent value.
        public float maxRotationSpeed;
        public float maxRotationRadiansPerSecond;
        public float minimumSpeedToTurningRatio;
        public float currentSpeedLimit;

        private Quaternion targetQuatRotation;
        private float playerToMouseAngle;
        private float currentToMaxSpeedRatio;
        private Vector3 newPosition;
        private bool facingRight;
        private bool isMoving;

        private GameObject opponentModel;
        private Vector3 opponentToNewVector;

        // Use this for initialization
        void Start()
        {
            sdGameController = GameController.getInstance();
            sdGameManager = GameManager.getInstance();
            rbOpponent = sdGameController.getOpponent().GetComponent<Rigidbody>();
            //xPosition = yPosition = 0.0f;
            //yAngle = yRotation = 0f;

            // If we are players1 aka host, then the enemy will face the left.
            // Otherwise, they face right.
            if (Constants.PLAYER_NUMBER % 2 == 1)
            {
                facingRight = false;
            }
            else
            {
                facingRight = true;
            }

            opponentModel = transform.Find("Model").gameObject;
            isMoving = false;
            // TODO: Get current speed limit from opponent.
            currentSpeedLimit = 100f;

            //turn = new Vector3 (0f, turnSpeed, 0f);
        }

        void FixedUpdate()
        {
            #region Old code
            /*
             * if (sdGameManager.getIsMultiplayer ())
             * {
                xPosition = sdGameController.getOpponentPlayer ().xPosition;
                yPosition = sdGameController.getOpponentPlayer ().yPosition;
                yRotation = sdGameController.getOpponentPlayer ().yRotation;
                rbOpponent.MovePosition (new Vector3(xPosition, yPosition, 0));

                xRotation = sdGameController.getOpponentPlayer ().xRotation;
                yAngle = -90;
                if (xRotation >= -90 && xRotation <= 90) {
                    xRotation = 180 - xRotation;
                    yAngle = 90;
                }
                rbOpponent.MoveRotation (Quaternion.Euler (xRotation - 180, yAngle, 0));
            }
            */
            #endregion

            // Only perform position updates while the game time is ticking.
            if(sdGameManager.getIsMultiplayer() && sdGameController.getIsGameTimeTicking())
            {
                newPosition = new Vector3(sdGameController.getOpponentPlayer().xPosition,
                    sdGameController.getOpponentPlayer().yPosition, 0);

                // Check to see if we still need to update the player's position.
                if (Vector3.Distance(newPosition, rbOpponent.position) < 0.1)
                {
                    isMoving = false;
                }
                else
                {
                    isMoving = true;
                }

                // This bit is tricky. We are not guaranteed to have precise enough tick rates to accurately
                // show the opponent's actual position if we use physics, since deltaTime between our updates,
                // theirs, and the server latency will always introduce inconsistencies.
                // Therefore, we fall back to setting the position per-frame and
                // and hope that the framerate is high enough to compensate.
                //TODO: Implement a way to lerp/slerp the position of the opponent so that movement looks more natural.
                if (isMoving)
                {
                    rbOpponent.position.Set(newPosition.x, newPosition.y, newPosition.z);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (sdGameManager.getIsMultiplayer() && sdGameController.getIsGameTimeTicking())
            {
                // Get the new target player position.
                newPosition = new Vector3(sdGameController.getOpponentPlayer().xPosition,
                    sdGameController.getOpponentPlayer().yPosition, 1);


                // Handle player model rotations.
                HandleRotations();

                // Calculate the rotation in angles between the opponent's current position and the new values.
                // We will use this data both to continue in-progress rotations and to initiate new rotations.
                newPosition = new Vector3(sdGameController.getOpponentPlayer().xPosition,
                    sdGameController.getOpponentPlayer().yPosition, 1);
                newPosition.z = 0;
                // Now normalize the vector.
                opponentToNewVector = (newPosition - new Vector3(transform.position.x, transform.position.y, 0));

                // Clamp the rotation rate.
                // Need to get the enemy player's speed limit from them.
                currentToMaxSpeedRatio = Mathf.Clamp(rbOpponent.velocity.magnitude / currentSpeedLimit, minimumSpeedToTurningRatio, maxRotationRadiansPerSecond);

                // Calculate and clamp the playerToMouseAngle angle to limit slerp amount.
                playerToMouseAngle = Mathf.Atan2(opponentToNewVector.y, opponentToNewVector.x) * Mathf.Rad2Deg;
                //Debug.Log("ptma: " + playerToMouseAngle + " zang: " + rb.rotation.eulerAngles.z);
                float maxAngle = Math.Abs(Mathf.Rad2Deg * maxRotationRadiansPerSecond * currentToMaxSpeedRatio);
                Mathf.Clamp(playerToMouseAngle, -maxAngle, maxAngle);

                // Convert the target rotation angle to a quaternion.
                targetQuatRotation = Quaternion.Euler(new Vector3(0f, 0f, playerToMouseAngle));


                // Rotate if angle between mouse vector and player direction vector is significant.
                if (Math.Abs(playerToMouseAngle) > .001)
                {
                    // Now apply the rotation slerp.
                    rbOpponent.rotation = Quaternion.Slerp(rbOpponent.rotation, targetQuatRotation, maxRotationSpeed * Time.deltaTime);
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Opponent")
            {
                sdGameController.setIsOpponentInBase(true);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.tag == "Opponent")
            {
                sdGameController.setIsOpponentInBase(false);
            }
        }

        private void HandleRotations()
        {
            // Invert oppnent model if they rotate beyond a vertical angle.
            // Do this by flagging for a slerp.
            if (facingRight && (transform.eulerAngles.z > 90f && transform.eulerAngles.z < 270f))
            {
                facingRight = false;
            }
            else if (!facingRight && (transform.eulerAngles.z <= 90f || transform.eulerAngles.z >= 270f))
            {
                facingRight = true;
            }

            // Continue slerping if transition is not complete or facing direction has changed.
            if (!facingRight && (Mathf.Abs(transform.eulerAngles.x - 180f) > 0.01 || Mathf.Abs(transform.eulerAngles.y - 90f) > 0.01))
            {
                opponentModel.transform.localRotation = Quaternion.Slerp(
                    opponentModel.transform.localRotation,
                    Quaternion.Euler(new Vector3(-180f, -90f, 0f)),
                    Time.deltaTime * maxRotationSpeed * 2);
            }
            else if (facingRight && (Mathf.Abs(transform.eulerAngles.x) > 0.01 || Mathf.Abs(transform.eulerAngles.y + 90f) > 0.01))
            {
                opponentModel.transform.localRotation = Quaternion.Slerp(
                    opponentModel.transform.localRotation,
                    Quaternion.Euler(new Vector3(0f, 90f, 0f)),
                    Time.deltaTime * maxRotationSpeed * 2);
            }
        }
    }
}
