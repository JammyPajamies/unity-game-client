using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Description: The code handles the visual effects for the player buffs.
*/

namespace SD
{
    public class BuffEffects : MonoBehaviour
    {
        private Renderer meshRenderer;
        private GameController gameController;
        
        // Use this for initialization
        void Start()
        {
            meshRenderer = GetComponent<Renderer>();
            gameController = GameController.getInstance();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            StartCoroutine(activateEffect());
        }

        IEnumerator activateEffect()
        {
            //speed power-up
            if (gameController.getSpeedBoostStatus() == true)
            {
                //changes player model's color when power-up is picked up
                meshRenderer.material.SetColor("_Color", Color.blue);

                // Get trails and enable them.
                Component[] buffTrails = this.GetComponentsInChildren<TrailRenderer>();
                foreach(TrailRenderer trail in buffTrails)
                {
                    if(!trail.isVisible)
                    {
                        trail.enabled = true;
                    }
                }

                //duration of power-up's effects
                yield return new WaitForSeconds(3f);

                // Disable the extra trails.
                foreach (TrailRenderer trail in buffTrails)
                {
                    trail.enabled = false;
                }

                //returns player model color back to original
                //sets power-up status to false
                meshRenderer.material.SetColor("_Color", Color.white);
                gameController.SetIsSpeedBuffActive(false);
            }

            //point power-up
            if (gameController.getPointBoostStatus() == true)
            {
                //changes player model's color when power-up is picked up
                meshRenderer.material.SetColor("_Color", Color.yellow);

                //duration of power-up's effects
                yield return new WaitForSeconds(3f);

                //returns player model color back to original
                //sets power-up status to false
                meshRenderer.material.SetColor("_Color", Color.white);
                gameController.SetIsPointBuffActive(false);
            }
        }
    }
}
