using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the speed buff controller script.
/// Inherits from the BuffTemplate class.
/// </summary>
namespace SD
{
    public class SpeedBuffController : BuffBackend
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
            buffEffects.setSpeedBoostDuration(buffDuration);
        }

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log("Collided with: " + other.gameObject.tag);

            // If the collision target is a buff fish, adjust stacks.
            if (other.gameObject.tag == "SpeedBuffFish")
            {
                // Add a stack of the buff, get the recalculated buff results.
                player.SetBuffAdjustedSpeedLimit(ApplyBuff());
                gameController.SetIsSpeedBuffActive(true);
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
            else if(needToResetBaseStat)
            {
                player.SetBuffAdjustedSpeedLimit(GetAdjustedStatAmount());
                needToResetBaseStat = false;
                gameController.SetIsSpeedBuffActive(false);
            }
        }
    }
}