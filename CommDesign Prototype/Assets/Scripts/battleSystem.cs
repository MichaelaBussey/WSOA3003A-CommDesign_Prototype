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
    public GameObject playerPrefab, enemyPrefab, WON, LOST;
    Stats PlayerUnit, EnemyUnit;
    public Transform playerLocation, enemyLocation;
    public BattleHUD playerHUD, enemyHUD;


    void Start()
    {
        State = BattleState.START;
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
            enemyHUD.HP.text = EnemyUnit.currentHP.ToString();
            State = BattleState.ENEMYTURN;
            CombatLog.text += atk1.ToString() + "dmg dealt to enemy." + "\n";
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
            int hitChance = 45;
            bool isDead = false;
            if (Random.Range(0, 100) <= hitChance)
            {
            int atk2 = Random.Range(5, 9);
            isDead = EnemyUnit.TakeDamage(atk2);
            //CombatLog.text = currentHP + "-" + dmg; idk tryna figure out the log
            enemyHUD.HP.text = EnemyUnit.currentHP.ToString();
            CombatLog.text += atk2.ToString() + "dmg dealt to enemy." + "\n";
            }
            else
            {
                CombatLog.text += ("Attack missed!" + "\n");
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
        int healAmt = Random.Range(2, 6) ;
        PlayerUnit.Heal(healAmt);
        CombatLog.text += "+" + healAmt.ToString() + " HP healed!" + "\n"; 
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
            WON.SetActive(true);
            //enemyPrefab.SetActive(false);
        }

        else if (State == BattleState.LOST)
        {
            LOST.SetActive(true);
            //playerPrefab.SetActive(false);

        }

    }

    IEnumerator EnemyTurn()
    {
        int EnemyAtk = Random.Range(4, 8);
        bool isDead = PlayerUnit.TakeDamage(EnemyAtk);
        CombatLog.text += EnemyAtk.ToString() + "dmg dealt to you." + "\n";
        playerHUD.HP.text = PlayerUnit.currentHP.ToString();
        yield return new WaitForSeconds(1);

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
