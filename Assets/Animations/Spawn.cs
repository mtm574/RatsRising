using UnityEngine;
using System.Collections;
// This script handles the spawning of animated rats and their damage to the boss
public class SpawnAnimatedRat : MonoBehaviour
{


    [Header("Rat Settings")]
    //adds for the inspector slots ;p
    public GameObject RatPrefab;
    public Transform SpawnPoint;
    public int ratsPerAttack = 1;
    public int ratDamage = 10;
    //Damage delay, had to add this bc I could not get the animation and colliders to work together
    public float damageDelay = 0.5f;


    [Header("Boss Manager")] 
    public BossManager bossManager;

    void Start()
    {
        Debug.Log("SpawnAnimatedRat Start: ratsPerAttack = " + ratsPerAttack);
    }

    //Handles the spawning and animation playing of the rats
    public IEnumerator Attack()
    {
        Debug.Log("Spawning " + ratsPerAttack + " rats with " + ratDamage + " damage each");

        int ratsSpawned = 0;

        for (int i = 0; i < ratsPerAttack; i++)
        {
            if (RatPrefab == null || SpawnPoint == null) continue;

            Vector3 spawnPos = SpawnPoint.position;
            //random placement of rats, so it looks a little cleaner
            spawnPos.x += Random.Range(-0.5f, 0.5f);
            spawnPos.y += Random.Range(-0.2f, 0.2f);

            GameObject ratInstance = Instantiate(RatPrefab, spawnPos, SpawnPoint.rotation);

            Animator ratAnim = ratInstance.GetComponent<Animator>();
            if (ratAnim != null)
                ratAnim.Play("RatRun");

            ratsSpawned++;
        }
        //Damage calculations and delay for animation to play
        yield return new WaitForSeconds(damageDelay);

        int totalDamage = ratsSpawned * ratDamage;

        if (bossManager != null && bossManager.CurrentBoss != null)
        {
            BossHealth bh = bossManager.CurrentBoss.GetComponent<BossHealth>();
            if (bh != null)
                bh.TakeDamage(totalDamage);
        }
        else
        {
            Debug.LogWarning("No boss found");
        }

        Debug.Log("Total Damage Dealt: " + totalDamage + " from " + ratsSpawned + " rats");
    }

    //UPGRADESSSSSS 
    public void UpgradeDamage(int extraDamage)
    {
        ratDamage += extraDamage;
    }

    public void UpgradeRatsPerAttack(int extraRats)
    {
        ratsPerAttack += extraRats;
    }
}