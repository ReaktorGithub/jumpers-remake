using System.ComponentModel;

public enum EAiBranchTypes
{
    [Description("Обычный бранч, ai не меняет кол-во очков")]
    Normal,
    [Description("Приятный вариант для рисокового ai")]
    Risky,
    [Description("Приятный вариант для осторожного ai")]
    Careful,
    [Description("Приятный вариант для любого ai")]
    Tasty,
    [Description("Неприятный вариант для любого ai")]
    Dirty,
}
