using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Description: This code handles the speedDebuffController;
Works similar to the speedBuffController 
*/

namespace SD {
    public class SpeedDebuffController : BuffBackend
    {

        // Variables that can be adjusted for balance.
        public float perStackBonus;
        public float buffDuration;
        public int maxBuffStacks;

        // Get the player and their collider.
        private PlayerController player;
        private GameController gameController;
        private BuffEffects buffEffects;

        void Start()
        {
            player = gameObject.GetComponent<PlayerController>();
            gameController = GameController.getInstance();
            buffEffects = BuffEffects.getInstance();

            SetBaseStat(player.GetBaseSpeed());
            SetMaxStackAmount(maxBuffStacks);
            SetMaxBuffDuration(buffDuration);
            SetBuffMechanism(BuffMechanism.MULTIPLICITIVE);
            SetBuffBonusPerStack(perStackBonus);
            SetAbsoluteMaxStatAmount(player.GetAbsoluteMaxSpeedLimit());
            buffEffects.setSlowDownDuration(buffDuration);
        }

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log("Collided with: " + other.gameObject.tag);

            // If the collision target is a buff fish, adjust stacks.
            if (other.gameObject.tag == "SlowDownEnemy")
            {
                // Add a stack of the buff, get the recalculated buff results.
                player.SetBuffAdjustedSpeedLimit(ApplyBuff());
                gameController.SetIsSlowDownActive(true);
            }
        }

        void Update()
        {
            // Update the player's base speed and inform the game controller of
            // player's speed boost buff state.
            if (Mathf.Abs(GetAdjustedStatAmount() - GetBaseStat()) > 0.1f)
            {
                player.SetBuffAdjustedSpeedLimit(GetAdjustedStatAmount());
            }
            else if (needToResetBaseStat)
            {
                player.SetBuffAdjustedSpeedLimit(GetAdjustedStatAmount());
                needToResetBaseStat = false;
                gameController.SetIsSlowDownActive(false);
            }
        }
    }
}
