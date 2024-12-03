using System;
using UnityEngine;

public class BankAcc : MonoBehaviour
{
    private int balance;
    public int AccountBalance { get { return balance; } }

    public Action OnAccountBalanceChanged;

    #region TRANSACTIONS
    public void SendMoney(int amount)
    {
        balance += amount;
        OnAccountBalanceChanged?.Invoke();
    }
    public void TakeMoney(int amount)
    {
        balance -= amount;
        OnAccountBalanceChanged?.Invoke();
    }
    #endregion

    #region SINGLETON
    public static BankAcc instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion
}
