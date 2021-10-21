/// <summary>
/// Utility class for simplifying all commands generated during a turn.
/// </summary>
public class TurnCommands
{
    public const int COMMANDS_PER_TURN = 3;

    public Command this[int index]
    {
        get => m_commands[index];
    }

    Command[] m_commands = new Command[COMMANDS_PER_TURN];

    public TurnCommands(Player player, Enemy enemy, MazeCellEdges dir)
    {
        m_commands[0] = new MovePlayerCommand(player, dir);
        m_commands[1] = new MoveEnemyCommand(enemy, player);
        m_commands[2] = new MoveEnemyCommand(enemy, player);
    }
}
