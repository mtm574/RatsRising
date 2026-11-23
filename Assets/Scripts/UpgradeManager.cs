using UnityEngine;
using TMPro;
//Manages the upgrade box!! That includes the drop rate, rate of attack, and the damage of the rats. It also handles the UI for the upgrade box.

public class UpgradeManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI AttackNum;
    public TextMeshProUGUI AttackCostNum;
    public TextMeshProUGUI AttackRateNum;
    public TextMeshProUGUI AttackRateCostNum;
    public TextMeshProUGUI DropRateNum;
    public TextMeshProUGUI DropRateCostNum;
    public TextMeshProUGUI FeatherNum;

    [Header("Rat Spawner")]
    public SpawnAnimatedRat ratSpawner; 

    //STATS!! 
    [Header("Stats")]
    public int attack = 1;
    public int attackCost = 10;
    public int attackRate = 0;
    public int attackRateCost = 20;
    public int dropRate = 0;
    public int dropRateCost = 30;
    public int feathers = 0;

    [Header("Auto Spawn Settings")]
    public float baseInterval = 1f;
    private float autoSpawnTimer = 0f;

    // Updates the UI after an upgrade is purchased or when the game starts
    void Start()
    {
        UpdateUI();
    }

    //Gets checked every frame to see if the player has pressed the space bar to attack or if the auto spawn timer has reached the interval for automatic attacks
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && ratSpawner != null)
        {
            ratSpawner.StartCoroutine(ratSpawner.Attack());
        }
        //passive rat attacks. 
        if (attackRate > 0 && ratSpawner != null)
        {
            autoSpawnTimer += Time.deltaTime;
            float interval = baseInterval / attackRate;
            interval = Mathf.Max(interval, 0.1f);
            //resets the auto spawn timer and calls the rat spawner to attack if the interval has been reached.
            if (autoSpawnTimer >= interval)
            {
                ratSpawner.StartCoroutine(ratSpawner.Attack());
                autoSpawnTimer = 0f;
            }
        }
    }
    //handles the purchase of an attack upgrade. It checks if the player has enough feathers, deducts the cost, increases the attack stat, and updates the UI. It also informs the rat spawner to upgrade the number of rats per attack.
    public void BuyAttackUpgrade()
    {
        if (feathers >= attackCost)
        {
            feathers -= attackCost;
            attack += 1;
            attackCost = Mathf.RoundToInt(attackCost * 1.5f);

            if (ratSpawner != null)
                ratSpawner.UpgradeRatsPerAttack(1);

            UpdateUI();
        }
    }
    //handles the purchase of an attack rate upgrade. It checks if the player has enough feathers, deducts the cost, increases the attack rate stat, and updates the UI. 
    public void BuyAttackRateUpgrade()
    {
        if (feathers >= attackRateCost)
        {
            feathers -= attackRateCost;
            attackRate += 1;
            attackRateCost = Mathf.RoundToInt(attackRateCost * 1.5f);
            UpdateUI();
        }
    }
    //handles the purchase of a drop rate upgrade. It checks if the player has enough feathers, deducts the cost, increases the drop rate stat, and updates the UI. (literally just like the other two functions)
    public void BuyDropRateUpgrade()
    {
        if (feathers >= dropRateCost)
        {
            feathers -= dropRateCost;
            dropRate += 1;
            dropRateCost = Mathf.RoundToInt(dropRateCost * 1.5f);
            UpdateUI();
        }

    }
    //refreshing of UI
    public void UpdateUI()
    {
        if (AttackNum != null) AttackNum.text = attack.ToString();
        if (AttackCostNum != null) AttackCostNum.text = attackCost.ToString();
        if (AttackRateNum != null) AttackRateNum.text = attackRate.ToString("0");
        if (AttackRateCostNum != null) AttackRateCostNum.text = attackRateCost.ToString();
        if (DropRateNum != null) DropRateNum.text = dropRate.ToString("0");
        if (DropRateCostNum != null) DropRateCostNum.text = dropRateCost.ToString();
        if (FeatherNum != null) FeatherNum.text = feathers.ToString();
    }
    //controls the 10 increase to the drop rate
    public int GetModifiedDrop(int bossBaseDrop)
    {
        return Mathf.RoundToInt(bossBaseDrop * (1f + dropRate * 0.10f));
    }
    //adds feathers to the player's total and updates the UI. This function can be called when the player earns feathers from defeating bosses or other in-game actions.(upgrades by 10% per purchase)
    public void AddFeathers(int amount)
    {
        feathers += amount;
        UpdateUI();
    }


}