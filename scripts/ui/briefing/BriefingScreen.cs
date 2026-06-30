using Godot;
using Godot.Collections;
using System.Text;

public partial class BriefingScreen : Control
{
    public override void _Ready()
    {
        var levelData = LevelManager.Instance.PendingLevelData;
        if (levelData == null)
        {
            GetNode<Label>("VBox/TitleLabel").Text = "MISSION";
            GetNode<Label>("VBox/GoldLabel").Text = $"Gold: {EconomyManager.Instance.CurrentMoney}  Lives: {GameManager.Instance.CurrentLives}";
            GetNode<Button>("VBox/StartButton").Pressed += () =>
            {
                UIManager.Instance.PopScreen();
                LevelManager.Instance.LoadPendingLevel();
            };
            return;
        }

        GetNode<Label>("VBox/TitleLabel").Text = levelData.LevelName.ToUpper();
        GetNode<Label>("VBox/GoldLabel").Text = $"Gold: {EconomyManager.Instance.CurrentMoney}  Lives: {GameManager.Instance.CurrentLives}";

        var sb = new StringBuilder();
        var waves = levelData.Waves;
        int waveNum = 1;
        foreach (var wave in waves)
        {
            if (wave?.Enemies == null || wave.Enemies.Count == 0) continue;
            var enemyNames = new StringBuilder();
            foreach (var enemy in wave.Enemies)
                enemyNames.Append(enemy?.EnemyName ?? "?").Append(" ");
            sb.AppendLine($"Wave {waveNum}: {wave.EnemyCount}x {enemyNames.ToString().Trim()}");
            waveNum++;
        }

        GetNode<Label>("VBox/WaveListLabel").Text = sb.ToString().TrimEnd();

        GetNode<Button>("VBox/StartButton").Pressed += () =>
        {
            UIManager.Instance.PopScreen();
            LevelManager.Instance.LoadPendingLevel();
        };
    }
}
