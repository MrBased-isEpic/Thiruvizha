using UnityEngine;

public class BankAcc : MonoBehaviour
{
    private int balance;
    public int AccountBalance { get { return balance; } }



    #region TRANSACTIONS
    public void SendMoney(int amount)
    {
        balance += amount;
    }
    public void TakeMoney(int amount)
    {
        balance -= amount;
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
