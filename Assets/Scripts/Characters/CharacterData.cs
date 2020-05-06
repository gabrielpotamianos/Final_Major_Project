using UnityEngine;
using UnityEngine.UI;

public class CharacterData : MonoBehaviour
{
    public bool Alive;
    public string Name;
    public Animator animator;

    protected Slider HealthBar;
    protected RawImage avatar;
    protected Canvas CanvasRoot;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected void UpdateBar(Slider bar, float value)
    {
        bar.value = value;
    }

    public bool IsItAlive(float health, float MaxHealth)
    {
        // if (MaxHealth > health && !InCombat) IsRegenHealth = true;
        return Alive = health > 0;
    }

    public void IsHealthRegenNeeded(ref bool healtRegen, float health, float MaxHealth)
    {
        healtRegen = health <= MaxHealth;
    }



    public Canvas GetCanvasRoot()
    {
        CanvasRoot = GameObject.Find("CanvasHUD").GetComponent<Canvas>();
        return CanvasRoot;
    }
    public Slider GetHealthBar()
    {
        return HealthBar;
    }
    public void SetHealthBar(Slider bar)
    {
        HealthBar = bar;
    }

}