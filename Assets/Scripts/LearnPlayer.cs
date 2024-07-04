using Unity.Services.Lobbies.Models;

public class LearnPlayer
{
    public string Name { get; set; }
    public string PlayerId { get; set; }
    public int LobbyId { get; set; }
    public int FinishedLevels { get; set; }
    public int TotalMistakes { get; set; }
    public int TotalPlayedTime { get; set; }

    public double MasteryOfSortingAlgorithm;
    public double LearnBasicSkills;
    public double LearnBehaviourOfSortingAlgorithm;

    public LearnPlayer(
        string name, 
        string playerId,
        int lobbyId, 
        int finishedLevels = 0, 
        int totalMistakes = 0,
        int totalPlayedTime = 0,
        double masteryOfSortingAlgorithm = 0.1,
        double learnBasicSkills = 0.1,
        double learnBehaviourOfSortingAlgorithm = 0.1)
    {
        Name = name;
        PlayerId = playerId;
        LobbyId = lobbyId;
        FinishedLevels = finishedLevels;
        TotalMistakes = totalMistakes;
        TotalPlayedTime = totalPlayedTime;
        MasteryOfSortingAlgorithm = masteryOfSortingAlgorithm;
        LearnBasicSkills = learnBasicSkills;
        LearnBehaviourOfSortingAlgorithm = learnBehaviourOfSortingAlgorithm;
    }

    public void UpdatePlayerData(string key, PlayerDataObject valueValue)
    {
        switch (key)
        {
            case Constants.PLAYER_FINISHED_LEVELS:
                FinishedLevels = int.Parse(valueValue.Value);
                break;
            case Constants.PLAYER_TOTAL_MISTAKES:
                TotalMistakes = int.Parse(valueValue.Value);
                break;
            case Constants.PLAYER_TOTAL_PLAYED_TIME:
                TotalPlayedTime = int.Parse(valueValue.Value);
                break;
        }
    }

    public void UpdateLearnerData(double masteryOfSortingAlgorithm,
        double learnBasicSkills,
        double learnBehaviourOfSortingAlgorithm)
    {
        MasteryOfSortingAlgorithm = masteryOfSortingAlgorithm;
        LearnBasicSkills = learnBasicSkills;
        LearnBehaviourOfSortingAlgorithm = learnBehaviourOfSortingAlgorithm;
    }
}