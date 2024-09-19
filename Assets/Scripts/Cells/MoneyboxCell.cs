using UnityEngine;

[RequireComponent(typeof(CellControl))]

public class MoneyboxCell : MonoBehaviour
{
    [SerializeField] private GameObject _moneyboxVault;

    public GameObject MoneyboxVault {
        get { return _moneyboxVault; }
        private set {}
    }
}
