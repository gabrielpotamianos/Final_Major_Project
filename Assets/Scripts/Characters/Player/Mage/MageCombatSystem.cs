using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MageCombatSystem : PlayerCombat
{
    public static bool InteruptCast = false;
    public delegate void SpellToCast();
    private delegate IEnumerator IEnumeratorDelegate();


    [Space(20)]
    [Header("Mage")]

    public Slider CastBar;
    public CanvasGroup CastBarCanvasGroup;

    [Space(10)]
    public Sprite FireballSprite;
    public GameObject FireballPrefab;
    public float FireballDamageMultiplier;
    public int FireballManaCost;
    public float FireballCooldownTime;
    public float FireballRange;
    public float FireballCastTime;
    public bool FireballOnCooldown = false;
    GameObject FireballGameObjectRef;
    [TextArea]
    public string FireballDescription;





    [Space(15)]
    public Color SpellIndicatorForbiddenColor;
    public Color SpellIndicatorColor;
    [Space(7)]
    public Texture ForbiddenTexture;
    public Texture SpellIndicatorTexture;
    public GameObject SpellIndicator;
    public GameObject IceMissle;
    public float NumberOfMissiles;
    public LayerMask IgnoreSpellIndicator;
    public Sprite BlizzardSprite;
    public float BlizzardCastTime;
    public float BlizzardCastRange;
    public float BlizzardAreaAffected;
    public float BlizzardDamageMultiplier;
    public float BlizzardManaCost;
    public float BlizzardCooldownTime;
    public bool BlizzardOnCooldown = false;

    [TextArea]
    public string BlizzardDescription;

    Projector BlizzardProjector;
    IEnumerator BlizzardCoroutine;
    GameObject SpellIndicatorGameObject;
    public static float activeMissiles;

    List<GameObject> BlizzardMissiles;






    [Space(15)]

    public Sprite DeathsBreathSprite;
    public GameObject DeathsBreathPrefab;

    public float DeathsBreathDamageMultiplier;

    public float DeathsBreathDuration;
    public float DeathsBreathHeight;

    public float DeathsBreathFlameRadius;
    public float DeathsBreathMaxDistance;
    public LayerMask DeathsBreathLayers;

    public float DeathsBreathCooldownTime;
    public bool DeathsBreathOnCooldown;

    GameObject DeathsBreathGameObject;

    [TextArea]
    public string DeathsBreathDescription;

    protected override void Start()
    {
        base.Start();
        CastBar = GameObject.Find("CastBar").gameObject.GetComponent<Slider>();
        CastBarCanvasGroup = CastBar.GetComponent<CanvasGroup>();
        BlizzardMissiles = new List<GameObject>();

        // Replace Keywords with values for Damage and Cost
        FireballDescription = FireballDescription.Replace(Constants.DAMAGE_DESCRIP_KEYWORD, ((int)FireballDamageMultiplier).ToString());
        FireballDescription = FireballDescription.Replace(Constants.COST_DESCRIP_KEYWORD, FireballManaCost.ToString());

        BlizzardDescription = BlizzardDescription.Replace(Constants.DAMAGE_DESCRIP_KEYWORD, ((int)BlizzardDamageMultiplier).ToString());
        BlizzardDescription = BlizzardDescription.Replace(Constants.COST_DESCRIP_KEYWORD, BlizzardManaCost.ToString());

        DeathsBreathDescription = DeathsBreathDescription.Replace(Constants.DAMAGE_DESCRIP_KEYWORD, ((int)DeathsBreathDamageMultiplier).ToString());
        DeathsBreathDescription = DeathsBreathDescription.Replace(Constants.COST_DESCRIP_KEYWORD, ((int)(playerData.statistics.CurrentSpellResource * 4 / 100)).ToString());

        //Sets the Images for Abilities
        playerData.SetSpellsUI(FireballSprite, BlizzardSprite, DeathsBreathSprite);

        //Asign the spell to buttons so they can be interacted from mouse
        AssignSpellsOnButtons(Fireball, FireballDescription, Blizzard, BlizzardDescription, DeathsBreath, DeathsBreathDescription);


    }

    protected override void Update()
    {

        //If player is alive check for input and pass the ability for further check if needed
        if (playerData.Alive)
            GetInput(Fireball, Blizzard, DeathsBreath);
        base.Update();
    }





    #region Abilities Checks

    void Fireball()
    {
        //Checks if the spell can be casted
        if (SpellChecks.CheckSpell(Target.instance.GetCurrentTarget(), playerData, FireballRange, FireballOnCooldown,
        FireballManaCost))
        {
            //Assigns the target in case of target changing while ability running
            Missiles.CurrTarger = Target.instance.getCurrEnemy();

            //Starts the casting
            StartCoroutine(CastSpell(FireballStart, FireballStop, FireballCastTime, CastBar, "Fireball"));
        }
    }

    void Blizzard()
    {
        //Checks the spell conditions
        if (SpellChecks.CheckSpell(playerData, BlizzardOnCooldown, BlizzardManaCost))
            
            //Starts the ability
            StartCoroutine(BlizzardStart());
    }

    void DeathsBreath()
    {
        //Checks the spell conditions using SpellChecks Class
        if (SpellChecks.CheckSpell(playerData, DeathsBreathOnCooldown, playerData.statistics.CurrentSpellResource * 4 / 100))
        {
            //Starts the cooldown for the ability;
            StartCoroutine(SpellCooldown(playerData.Spell3, DeathsBreathCooldownTime, (x) => { DeathsBreathOnCooldown = x; }));
            
            //Starts the ability
            StartCoroutine(DeathsBreathStart(DeathsBreathDuration));
        }

    }

    #endregion




    IEnumerator CastSpell(SpellToCast spell, SpellToCast StopSpell, float CastTime, Slider Castbar, string AnimTransitionBool)
    {
        //If spell was interupted before starting the cast exit
        if (!InteruptCast)
        {
            //SpellCheckAssigned makes sure no other ability is casted in the same time
            SpellCheckAssigned = true;

            //Enable aniamtions
            playerData.animator.SetBool("Casting", true);

            //Set bar active
            CastBarCanvasGroup.alpha = 1;
            CastBar.value = 0;

            //Reset the time
            float timeLeft = 0;

            //Cast
            while (Castbar.value < 1)
            {

                //If interupted while casting quit
                if (InteruptCast)
                {
                    //Turn the bar off and reset the values
                    CastBar.value = 0;
                    CastBarCanvasGroup.alpha = 0;

                    //disable animation
                    playerData.animator.SetBool("Casting", false);

                    //Call the end actions of the spell 
                    StopSpell.Invoke();

                    //Stops
                    yield break;
                }
                
                //Increase the time by framerate
                timeLeft += Time.deltaTime;

                //Divide Timeleft by total Time
                Castbar.value = timeLeft / CastTime;

                yield return null;
            }

            //Set the correct animation 
            playerData.animator.SetBool(AnimTransitionBool, true);

            //Call the callback
            spell.Invoke();

            //disable the bar
            CastBarCanvasGroup.alpha = 0;
        }
    }

    IEnumerator CastSpell(IEnumeratorDelegate spell, SpellToCast StopSpell, float CastTime, Slider Castbar, string AnimTransitionBool)
    {

        //If spell was interupted before starting the cast exit
        if (!InteruptCast)
        {
            //Initialise coroutine 
            IEnumeratorDelegate Spell;

            //SpellCheckAssigned makes sure no other ability is casted in the same time
            SpellCheckAssigned = true;

            //Enable animation
            playerData.animator.SetBool("Casting", true);

            //Enable bar and reset value
            CastBarCanvasGroup.alpha = 1;
            CastBar.value = 0;

            //Reset time
            float timeLeft = 0;
            
            //Assign the coroutine so it can be stopped properly if needed
            Spell = spell;

            //Start the corotuine
            StartCoroutine(Spell.Invoke());

            while (Castbar.value < 1)
            {

                //If ablity has been interrupted
                if (InteruptCast)
                {
                    //Reset bar value and disable it
                    CastBar.value = 0;
                    CastBarCanvasGroup.alpha = 0;

                    //Disable animation
                    playerData.animator.SetBool("Casting", false);
                    
                    //Call the ending function of the spell
                    StopSpell.Invoke();

                    yield break;
                }

                //Add time by the time frame
                timeLeft += Time.deltaTime;

                //get the value 
                Castbar.value = timeLeft / CastTime;
                yield return null;
            }

            //enable animation
            playerData.animator.SetBool(AnimTransitionBool, true);


            //disable bar
            CastBarCanvasGroup.alpha = 0;
        }
    }


    #region Fireball - Body Functions


    /// <summary>
    /// Fireball Start Function - Triggers the cooldown of the ability
    /// </summary>
    void FireballStart()
    {
        ResetCombatCoroutine();
        
        //Starts the cooldown of the spell
        StartCoroutine(SpellCooldown(playerData.Spell1, FireballCooldownTime, (x) => { FireballOnCooldown = x; ; }));
    }


    /// <summary>
    /// Instantiate the fireball or reposition the fireball
    /// </summary>
    void InitFireball()
    {
        //Gets the hands references
        GameObject rightHand = transform.Find("Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_R/Shoulder_R/Elbow_R/Hand_R").gameObject;
        GameObject leftHand = transform.Find("Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_L/Shoulder_L/Elbow_L/Hand_L").gameObject;

        //Get the middle point between the hands
        Vector3 FireballInitPos = (rightHand.transform.position + leftHand.transform.position) / 2;

        //If has not been instantiated
        if (!FireballGameObjectRef)
            FireballGameObjectRef = Instantiate(FireballPrefab, FireballInitPos, Quaternion.identity);
        else // if has been instantiated
        {
            FireballGameObjectRef.SetActive(true);
            FireballGameObjectRef.transform.position = FireballInitPos;
        }

        //attach to the right hand
        FireballGameObjectRef.transform.parent = rightHand.gameObject.transform;

    }


    /// <summary>
    /// Launches Fireball (Enables the Missile Script)
    /// </summary>
    void LaunchFireball()
    {
        //Deattach it from the hand 
        FireballGameObjectRef.transform.parent = null;

        //Enable the miisile script
        FireballGameObjectRef.GetComponent<Missiles>().enabled = true;

        //Assign damage multiplier for the script
        FireballGameObjectRef.GetComponent<Missiles>().FireballMultiplier = FireballDamageMultiplier;
        FireballGameObjectRef.GetComponent<Collider>().enabled = true;
        playerData.UpdateSpellResource(-FireballManaCost);
        
        //Enables spell resource regen
        SpellResourceRegen = true;

    }


    /// <summary>
    /// Sets all the Animator Transition to false
    /// </summary>
    void FireballStop()
    {
        playerData.animator.SetBool("Fireball", false);
        playerData.animator.SetBool("Casting", false);
        SpellCheckAssigned = false;
    }

    #endregion


    #region Blizzard - Body Functions

    /// <summary>
    /// Blizzard Start - Starts the Projector and places the Spell Marker
    /// </summary>
    /// <returns></returns>
    IEnumerator BlizzardStart()
    {
        if (!InteruptCast)
        {
            Ray dir = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(dir, out hit, Mathf.Infinity, IgnoreSpellIndicator);

            //Create a spell marker if is not created and assign the blizzard projector for it
            if (!SpellIndicatorGameObject)
            {
                SpellIndicatorGameObject = Instantiate(SpellIndicator, new Vector3(hit.point.x, 0, hit.point.z), Quaternion.identity);
                BlizzardProjector = SpellIndicatorGameObject.transform.GetChild(0).GetComponent<Projector>();
            }
            else
            {
                SpellIndicatorGameObject.transform.GetChild(0).gameObject.SetActive(true);
                BlizzardProjector.enabled = true;

            }

            while (true)
            {
                //Raycast from the mouse position on the world space
                dir = Camera.main.ScreenPointToRay(Input.mousePosition);
                Physics.Raycast(dir, out hit, Mathf.Infinity, IgnoreSpellIndicator);

                //Move spell marker if you hit a collider
                if (hit.collider)
                    SpellIndicatorGameObject.transform.position = new Vector3(hit.point.x, SpellIndicator.transform.position.y, hit.point.z);


                //If the distance is too far from the pre-set range
                if (Vector3.Distance(gameObject.transform.position, SpellIndicatorGameObject.transform.position) > BlizzardCastRange)
                {
                    //Display message out of range
                    MessageManager.instance.DisplayMessage(Constants.OUT_OF_RANGE, 0.1f);

                    //Change the projector colour
                    BlizzardProjector.material.color = SpellIndicatorForbiddenColor;

                    //Change the texture to forbidden sign
                    BlizzardProjector.material.SetTexture("_ShadowTex", ForbiddenTexture);
                }

                //if distance meet requirements
                else if (Vector3.Distance(gameObject.transform.position, SpellIndicatorGameObject.transform.position) < BlizzardCastRange)
                {
                    //Set the coloour to green
                    BlizzardProjector.material.color = SpellIndicatorColor;

                    //Set the indicator to the normal spell marker sign
                    BlizzardProjector.material.SetTexture("_ShadowTex", SpellIndicatorTexture);

                    if (Input.GetMouseButtonDown(0))
                    {
                        //Deactivate the projector gameobject
                        SpellIndicatorGameObject.transform.GetChild(0).gameObject.SetActive(false);

                        //Cast the spell
                        StartCoroutine(CastSpell(BlizzardLaunch, BlizzardStop, BlizzardCastTime, CastBar, "Blizzard"));
                        
                        playerData.UpdateSpellResource(-BlizzardManaCost);

                        yield break;
                    }
                }
                //if right click has been pressed
                if (Input.GetMouseButtonDown(1))
                {
                    //Cancel
                    SpellIndicatorGameObject.transform.GetChild(0).gameObject.SetActive(false);
                    yield break;
                }
                yield return null;
            }
        }
        else //if spell has been interrupted
        {
            //Call stop function
            BlizzardStop();

            //disable spellmarker if it is enabled
            if (SpellIndicatorGameObject)
                SpellIndicatorGameObject.transform.GetChild(0).gameObject.SetActive(false);
            yield break;
        }

    }

    /// <summary>
    /// Blizzard Launch - Instantiate the Missiles or Reposition them
    /// <para>Object Pooling</para>
    /// </summary>
    /// <returns></returns>
    IEnumerator BlizzardLaunch()
    {
        if (!InteruptCast)
        {
            //Start the cooldown
            StartCoroutine(SpellCooldown(playerData.Spell2, BlizzardCooldownTime, (x) => { BlizzardOnCooldown = x; }));
            
            //Disable projector
            BlizzardProjector.enabled = false;
            ResetCombatCoroutine();

            for (int i = 0; i < NumberOfMissiles; i++)
            {
                //stop spawning ice projectiles if spell was interrupted
                if (InteruptCast)
                {
                    BlizzardStop();
                    break;
                }
                //Get random position within a given radius
                Vector2 pos = new Vector2(SpellIndicatorGameObject.transform.position.x, SpellIndicatorGameObject.transform.position.z) + UnityEngine.Random.insideUnitCircle * BlizzardAreaAffected;

                //If projectiles have not been instantiated
                if (BlizzardMissiles.Count < NumberOfMissiles)
                {
                    var temp = Instantiate(IceMissle, new Vector3(pos.x, SpellIndicatorGameObject.transform.GetChild(0).transform.position.y, pos.y), Quaternion.identity);
                    activeMissiles++;
                    temp.GetComponent<Missiles>().enabled = true;
                    temp.GetComponent<Missiles>().BlizzardDamage = BlizzardDamageMultiplier;
                    
                    //Assign the AOE object
                    temp.GetComponent<Missiles>().AOEDamage = SpellIndicatorGameObject.transform.GetChild(1).gameObject.GetComponent<AOEDamageScript>();
                    BlizzardMissiles.Add(temp);
                }
                else //if have been instantiated, reuse them
                {
                    BlizzardMissiles[i].transform.position = new Vector3(pos.x, BlizzardProjector.transform.position.y, pos.y);
                    BlizzardMissiles[i].transform.rotation = Quaternion.identity;
                    BlizzardMissiles[i].GetComponent<Missiles>().enabled = true;
                    BlizzardMissiles[i].SetActive(true);
                }

                yield return new WaitForSeconds(BlizzardCastTime / NumberOfMissiles);
            }

            //Wait for the projectiles to be destroyed to disable the spell marker indicator
            while (activeMissiles > 0)
            {
                yield return null;
            }
            SpellIndicatorGameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
        else //if spell was interrupted
        {
            BlizzardStop();
            yield break;
        }
    }


    /// <summary>
    /// Blizzard Stop - Stops the animations
    /// </summary>
    void BlizzardStop()
    {
        playerData.animator.SetBool("Casting", false);
        playerData.animator.SetBool("Blizzard", false);
        SpellCheckAssigned = false;
    }

    #endregion


    #region Death's Breath - Body Function

    /// <summary>
    /// Death's Breath - Flame thrower
    /// <para> Creates a Flame - Thrower Skull </para>
    /// </summary>
    /// <param name="TimeToBePausedFor"></param>
    /// <returns></returns>
    IEnumerator DeathsBreathStart(float TimeToBePausedFor)
    {
        //Disables the Spell Assignment 
        SpellCheckAssigned = true;
        ResetCombatCoroutine();

        //Consume the 4% of the Player's Mana
        playerData.UpdateSpellResource(-(playerData.statistics.CurrentSpellResource * 4 / 100));
        SpellResourceRegen = true;


        //Checks if the Object was instantiated
        if (!DeathsBreathGameObject)
            DeathsBreathGameObject = Instantiate(DeathsBreathPrefab, transform.position + new Vector3(0, DeathsBreathHeight, 0), transform.rotation);
        else
        {
            //Reposition Object, Rotates Object, Activates Object, Reset Initial Setup of the Children
            DeathsBreathGameObject.SetActive(true);
            DeathsBreathGameObject.transform.GetChild(0).gameObject.SetActive(false);
            DeathsBreathGameObject.transform.GetChild(1).gameObject.SetActive(false);

            DeathsBreathGameObject.transform.position = transform.position + new Vector3(0, DeathsBreathHeight, 0);
            DeathsBreathGameObject.transform.rotation = transform.rotation;
        }

        //Make it a Child of the Player GameObject
        DeathsBreathGameObject.transform.parent = gameObject.transform;

        //Data Gather
        var FireRaisingParticle = DeathsBreathGameObject.transform.GetChild(0).transform.GetChild(1).GetComponent<ParticleSystem>().main;
        var SkullParticle = DeathsBreathGameObject.transform.GetChild(0).transform.GetChild(2).GetComponent<ParticleSystem>();
        var JawParticle = DeathsBreathGameObject.transform.GetChild(0).transform.GetChild(3).GetComponent<ParticleSystem>();
        var EndSmokeParticle = DeathsBreathGameObject.transform.GetChild(0).transform.GetChild(5).GetComponent<ParticleSystem>().main;


        //Updates the Duration and the Start Delay Time to the Time we want the skull to be paused for
        FireRaisingParticle.duration += TimeToBePausedFor;
        EndSmokeParticle.startDelay = EndSmokeParticle.startDelay.constant + TimeToBePausedFor;


        //Enables the FireFlame and Skull Particle Effects
        DeathsBreathGameObject.transform.GetChild(0).gameObject.SetActive(true);
        DeathsBreathGameObject.transform.GetChild(1).gameObject.SetActive(true);

        //Gathers all Objects in from of the player 
        RaycastHit[] hitss;
        hitss = Physics.SphereCastAll(transform.position + new Vector3(0, DeathsBreathHeight, 0), DeathsBreathFlameRadius, transform.forward, DeathsBreathMaxDistance, DeathsBreathLayers, QueryTriggerInteraction.UseGlobal);

        foreach (RaycastHit hit in hitss)
        {
            //If any of the objects on that Layer Mask are Enemies then Hit them if they are alive
            if (hit.collider.GetComponent<EnemyCombat>() && hit.collider.GetComponent<EnemyCombat>().enemyData.Alive)
                DealDamage(hit.collider.GetComponent<EnemyCombat>(), DeathsBreathDamageMultiplier);
        }

        //Wait for Half of the Skull Particle Effect to play
        yield return new WaitForSeconds(SkullParticle.main.startLifetime.constant / 2);


        //Pause Skull and Jaw for TimeToBePausedFor
        SkullParticle.Pause(true);
        JawParticle.Pause(true);

        yield return new WaitForSeconds(TimeToBePausedFor);

        //Play Skull and Jaw Particle Effect
        SkullParticle.Play(true);
        JawParticle.Play(true);

        //Wait for the remaining time of the skull effect
        yield return new WaitForSeconds(SkullParticle.main.duration - SkullParticle.time);

        //Disable the FlameThrower Particle
        DeathsBreathGameObject.transform.GetChild(1).gameObject.SetActive(false);

        //Release Spell Assignment
        SpellCheckAssigned = false;

        //Wait for the remaining time of the EndSmokeParticle Effect
        yield return new WaitForSeconds(EndSmokeParticle.duration - DeathsBreathGameObject.transform.GetChild(0).transform.GetChild(5).GetComponent<ParticleSystem>().time);

        //Disbale Skull GameObject
        DeathsBreathGameObject.transform.GetChild(0).gameObject.SetActive(false);

        //Disable Entire Object
        DeathsBreathGameObject.SetActive(false);

        //Reset  
        FireRaisingParticle.duration -= TimeToBePausedFor;
        EndSmokeParticle.startDelay = EndSmokeParticle.startDelay.constant - TimeToBePausedFor;



    }

    #endregion
}