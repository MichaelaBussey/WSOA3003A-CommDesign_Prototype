using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    START, PLAYERTURN, ENEMYTURN, WON, LOST
}

public class battleSystem : MonoBehaviour
{
    public BattleState State;
    public Text CombatLog;
    public GameObject playerPrefab, enemyPrefab, WON, LOST, critHit, winBox, loseBox, End;
    Stats PlayerUnit, EnemyUnit;
    public Transform playerLocation, enemyLocation;
    public BattleHUD playerHUD, enemyHUD;
    public AudioClip Crit, Heal, Hit, Miss;


    void Start()
    {
        State = BattleState.START;
        playerPrefab.SetActive(true);
        enemyPrefab.SetActive(true);
        StartCoroutine(SetupBattle());
    }
    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerLocation);
        PlayerUnit = playerGO.GetComponent<Stats>();
        GameObject enemyGO = Instantiate(enemyPrefab, enemyLocation);
        EnemyUnit = enemyGO.GetComponent<Stats>();
        //Retrieves stats about the two
        playerHUD.SetHUD(PlayerUnit);
        enemyHUD.SetHUD(EnemyUnit);

        yield return new WaitForSeconds(0);

        State = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAtk1()
    {
        if (State == BattleState.PLAYERTURN)
        {
            int atk1 = Random.Range(3, 6);
            bool isDead = EnemyUnit.TakeDamage(atk1);
            //ACHIEVED MULTIPLE SOUNDS ON ONE SCRIPT!!
            GetComponent<AudioSource>().clip = Hit;
            GetComponent<AudioSource>().Play();
            enemyHUD.HP.text = EnemyUnit.currentHP.ToString();
            State = BattleState.ENEMYTURN;
            CombatLog.text +=  atk1.ToString() + "dmg dealt to enemy." + "\n";
            CombatLog.color = Color.blue;
            yield return new WaitForSeconds(2);

            if (isDead)
            {
                State = BattleState.WON;
                EndBattle();
            }

            else
            {
                StartCoroutine(EnemyTurn());
            }
        }

    }

    IEnumerator PlayerAtk2()
    {
        if (State == BattleState.PLAYERTURN)
        {
            int hitChance = 75;
            bool isDead = false;
            int atk2;
            if (Random.Range(0, 100) <= hitChance)
            {
            atk2 = Random.Range(5, 10);
            isDead = EnemyUnit.TakeDamage(atk2);
            GetComponent<AudioSource>().clip = Hit;
            GetComponent<AudioSource>().Play();
            enemyHUD.HP.text = EnemyUnit.currentHP.ToString();
            CombatLog.text += atk2.ToString() + "dmg dealt to enemy." + "\n";
            CombatLog.color = Color.blue;            
                if (atk2 >= 7)
                {
                    GetComponent<AudioSource>().clip = Crit;
                    GetComponent<AudioSource>().Play();
                    Instantiate(critHit, enemyLocation);
                    CombatLog.text += "It was a critical hit!" + "\n";
                }
            }
            else
            {
                GetComponent<AudioSource>().clip = Miss;
                GetComponent<AudioSource>().Play();
                CombatLog.text += ("Attack missed!" + "\n");
                CombatLog.color = Color.red;
            }



            State = BattleState.ENEMYTURN;
            yield return new WaitForSeconds(2);
            if (isDead)
            {
                State = BattleState.WON;
                EndBattle();
            }

            else
            {
                StartCoroutine(EnemyTurn());
            }
        }

    }

    IEnumerator PlayerHeal()
    {
        if( State == BattleState.PLAYERTURN)
        {
        int healAmt = Random.Range(3, 7) ;
        PlayerUnit.Heal(healAmt);
        GetComponent<AudioSource>().clip = Heal;
        GetComponent<AudioSource>().Play();
        CombatLog.text += "+" + healAmt.ToString() + " HP healed!" + "\n";
        CombatLog.color = Color.green;
        playerHUD.HP.text = PlayerUnit.currentHP.ToString();
        State = BattleState.ENEMYTURN;
        yield return new WaitForSeconds(2);
        StartCoroutine(EnemyTurn());
        }

    }


    void EndBattle()
    {
        if (State == BattleState.WON)
        {            
            winBox.SetActive(true);
            WON.SetActive(true);
            End.SetActive(true);
        }

        else if (State == BattleState.LOST)
        {
            loseBox.SetActive(true);            
            LOST.SetActive(true);
            End.SetActive(true);
        }

    }

    IEnumerator EnemyTurn()
    {
        int hitChance = 85;
        bool isDead = false;
        if (Random.Range(0, 100) <= hitChance)
        {
            int EnemyAtk = Random.Range(3, 8);
            isDead = PlayerUnit.TakeDamage(EnemyAtk);
            GetComponent<AudioSource>().clip = Hit;
            GetComponent<AudioSource>().Play();
            CombatLog.text += EnemyAtk.ToString() + "dmg dealt to you." + "\n";
            CombatLog.color = Color.red;
            playerHUD.HP.text = PlayerUnit.currentHP.ToString();
            yield return new WaitForSeconds(1);
        }
        else
        {
            GetComponent<AudioSource>().clip = Miss;
            GetComponent<AudioSource>().Play();
            CombatLog.text += ("Enemy attack missed!" + "\n");
            CombatLog.color = Color.red;
        }

        if (isDead)
        {
            State = BattleState.LOST;
            EndBattle();
        }
        else
        {
            State = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    void PlayerTurn()
    {
        //Choose an action
    }

    public void OnAttack1()
    {
        if (State != BattleState.PLAYERTURN)
        {
            return;
        }
        else
        {
            StartCoroutine(PlayerAtk1());
        }
    }

    public void OnAttack2()
    {
        if (State != BattleState.PLAYERTURN)
        {
            return;
        }
        else
        {
            StartCoroutine(PlayerAtk2());
        }
    }

    public void OnHeal()
    {
        if (State != BattleState.PLAYERTURN)
        {
            return;
        }
        else
        {
            StartCoroutine(PlayerHeal());
        }
    }
}
