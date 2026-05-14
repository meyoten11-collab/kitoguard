namespace API.ServerAddon;

public enum GameServerActionResult : short
{
    Unknown = 0,
    Success = 1,
    ActionUndefined = 2,
    UnexpectedException = 3,
    ParamsNotSupplied = 4,
    CharNameNotFound = 5,
    FunctionError = 6
}
