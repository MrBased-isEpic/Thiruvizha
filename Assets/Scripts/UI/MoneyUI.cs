using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyAmount;


    private void Start()
    {
        BankAcc.instance.OnAccountBalanceChanged += OnBankBalanceChanged;
        moneyAmount.text = "0";
    }

    private void OnBankBalanceChanged()
    {
        moneyAmount.text = BankAcc.instance.AccountBalance.ToString();
    }
}
