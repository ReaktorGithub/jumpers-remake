using UnityEngine;

[RequireComponent(typeof(CellControl))]

public class MoneyboxCell : MonoBehaviour
{
    [SerializeField] private GameObject _moneyboxVault;

    public MoneyboxVault MoneyboxVault {
        get { return _moneyboxVault.GetComponent<MoneyboxVault>(); }
        private set {}
    }
}
