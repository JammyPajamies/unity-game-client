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
        private float speedBoostDuration = 0f;
        private float slowDownDuration = 0f;
        private float pointBoostDuration = 0f;
        private float evasionBoostDuration = 0f;
        private static BuffEffects buffEffects;

        void Awake()
        {
            buffEffects = this;
        }

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
            if (gameController.getSpeedBoostStatus() == true)
            {
                //changes player model's color when power-up is picked up
                meshRenderer.material.SetColor("_Color", Color.blue);

                // Get trails and enable them.
                Component[] buffTrails = this.GetComponentsInChildren<TrailRenderer>();
                foreach (TrailRenderer trail in buffTrails)
                {
                    if (!trail.isVisible)
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
                yield return new WaitForSeconds(pointBoostDuration);

                //returns player model color back to original
                //sets power-up status to false
                meshRenderer.material.SetColor("_Color", Color.white);
                gameController.SetIsPointBuffActive(false);
            }

            //evasion power-up
            if (gameController.getEvasionBoostStatus() == true)
            {
                //changes player model's color when power-up is picked up
                meshRenderer.material.SetColor("_Color", Color.gray);

                //duration of power-up's effects
                yield return new WaitForSeconds(evasionBoostDuration);

                //returns player model color back to original
                //sets power-up status to false
                meshRenderer.material.SetColor("_Color", Color.white);
                gameController.SetIsEvasionBuffActive(false);
            }

            //slow down
            if (gameController.getSlowDownStatus() == true)
            {
                //changes player model's color when power-up is picked up
                meshRenderer.material.SetColor("_Color", Color.green);

                //duration of power-up's effects
                yield return new WaitForSeconds(slowDownDuration);

                //returns player model color back to original
                //sets power-up status to false
                meshRenderer.material.SetColor("_Color", Color.white);
                gameController.SetIsSlowDownActive(false);
            }
        }

        #region Getters & Setters
        public void setSpeedBoostDuration(float duration)
        {
            speedBoostDuration = duration;
        }

        public void setPointBoostDuration(float duration)
        {
            pointBoostDuration = duration;
        }

        public void setEvasionBoostDuration(float duration)
        {
            evasionBoostDuration = duration;
        }

        public void setSlowDownDuration(float duration)
        {
            slowDownDuration = duration;
        }

        public static BuffEffects getInstance()
        {
            return buffEffects;
        }

        #endregion
    }
}
