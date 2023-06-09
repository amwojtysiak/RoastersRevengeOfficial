using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public static GameMaster gm;

    public Transform playerPrefab;
    public Transform spawnPoint;
    public float spawnDelay = 2;
    public Transform spawnPrefab;
    public Transform enemyPrefab;
    public Transform enemySpawnPoint;

    private StatusIndicator statusIndicator;


    //public Transform boxBreakPoint;
    public Transform boxBreakPrefab;


    public class GameStateStats
    {
        public int playerLives = 5;
        public int playerDeathCount = 0;
        public int enemyKillCount = 1;

    }

    public GameStateStats gameStateStats = new();


    private void Start()
    {
        if (gm == null)
        {
            gm = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
        }
    }

    public static void KillPlayer(Player player)
    {
        Destroy(player.gameObject);
        gm.gameStateStats.playerDeathCount++;
        Debug.Log("Player Died");
        gm.StartCoroutine(gm.RespawnPlayer());
    }

    public static void KillObject(InteractiveObject intObj)
    {
        
        //Transform clone = Instantiate(gm.boxBreakPrefab, gm.boxBreakPoint.position, gm.boxBreakPoint.rotation) as Transform;
        Transform clone = Instantiate(gm.boxBreakPrefab, intObj.transform.position, intObj.transform.rotation) as Transform;
        Destroy(clone.gameObject, 3f);
        Destroy(intObj.gameObject);
        Debug.Log("object Destroyed");
    }

    public static void KillEnemy(SpaceShip_Enemy intObj)
    {

        //Transform clone = Instantiate(gm.boxBreakPrefab, gm.boxBreakPoint.position, gm.boxBreakPoint.rotation) as Transform;
        Transform clone = Instantiate(gm.boxBreakPrefab, intObj.transform.position, intObj.transform.rotation) as Transform;
        Destroy(clone.gameObject, 3f);
        Destroy(intObj.gameObject);
        gm.gameStateStats.enemyKillCount++;
        Debug.Log("object Destroyed");
        for (int i = 0; i < gm.gameStateStats.enemyKillCount/2; i++)
        {
            if (i > 5)
            {
                return;
            }
            gm.StartCoroutine(gm.RespawnEnemy());
        }
        //gm.StartCoroutine(gm.RespawnEnemy());
    }

    public IEnumerator RespawnPlayer()
    {
        //Debug.Log("TODO: Waiting for spawn sound");
        Debug.Log("DeathCount = " + gameStateStats.playerDeathCount);
        yield return new WaitForSeconds(spawnDelay);
        if (gm.gameStateStats.playerDeathCount <gm.gameStateStats.playerLives)
        {
            Transform player_clone = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation) as Transform;
            //statusIndicator.

            Transform clone = Instantiate(spawnPrefab, spawnPoint.position, spawnPoint.rotation) as Transform;
            Destroy(clone.gameObject, 3f);
        }
        
        
    }

    public IEnumerator RespawnEnemy()
    {
        //Debug.Log("TODO: Waiting for spawn sound");
        Debug.Log("KillCount = " + gameStateStats.enemyKillCount);
        yield return new WaitForSeconds(spawnDelay);
        Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);
        Transform clone = Instantiate(spawnPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation) as Transform;
        Destroy(clone.gameObject, 3f);

    }
}
