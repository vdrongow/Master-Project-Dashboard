using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Config
{
    public GlobalWeights GlobalWeights { get; set; }
    public List<string> ActivityNames { get; set; }
    public Dictionary<string, Dictionary<string, double>> ActivityObservationWeights { get; set; }
    public List<string> SimulatedLearnerBehaviorTypes { get; set; }
    public Dictionary<string, Dictionary<string, ScalarBelief>> InitialScalarBeliefs { get; set; }
    public Dictionary<string, List<string>> ActivityRecommenderTargets { get; set; }
    public Dictionary<string, CompetenceModel> CompetenceModel { get; set; }
}

public class GlobalWeights
{
    public double GeneralEvidenceWeight { get; set; }
    public double RecommendationCompetenceWeaknessScoreWeight { get; set; }
    public double RecommendationActivityRepetitionScoreWeight { get; set; }
    public double RecommendationActivityCorrectIncorrectRatioScore { get; set; }
    public double RecommendationDifficultyFlowModifier { get; set; }
    public double RecommendationDifficultyConstantAddition { get; set; }
}

public class CompetenceModel
{
    public string ID { get; set; }
    public List<string> States { get; set; }
    public List<double> Cpt { get; set; }
    public List<string> Parents { get; set; }
}

public class ScalarBelief
{
    public double Value { get; set; }
    public double Certainty { get; set; }
}

public class ScalarBeliefs
{
    [JsonProperty("masteryOfSortingAlgorithm")]
    public ScalarBelief MasteryOfSortingAlgorithm { get; set; }

    [JsonProperty("learnBehaviourOfSortingAlgorithms")]
    public ScalarBelief LearnBehaviourOfSortingAlgorithms { get; set; }

    [JsonProperty("learnBasicSkills")]
    public ScalarBelief LearnBasicSkills { get; set; }

    [JsonProperty("bubbleSort")]
    public ScalarBelief BubbleSort { get; set; }

    [JsonProperty("selectionSort")]
    public ScalarBelief SelectionSort { get; set; }

    [JsonProperty("insertionSort")]
    public ScalarBelief InsertionSort { get; set; }

    [JsonProperty("swapElements")]
    public ScalarBelief SwapElements { get; set; }

    [JsonProperty("stepOver")]
    public ScalarBelief StepOver { get; set; }

    [JsonProperty("foundNewMin")]
    public ScalarBelief FoundNewMin { get; set; }

    [JsonProperty("noNewMin")]
    public ScalarBelief NoNewMin { get; set; }

    [JsonProperty("swapFurtherForwards")]
    public ScalarBelief SwapFurtherForwards { get; set; }

    [JsonProperty("insertElement")]
    public ScalarBelief InsertElement { get; set; }

    [JsonProperty("identifySmallerNumber")]
    public ScalarBelief IdentifySmallerNumber { get; set; }

    [JsonProperty("identifySmallestElement")]
    public ScalarBelief IdentifySmallestElement { get; set; }

    [JsonProperty("numberComparison")]
    public ScalarBelief NumberComparison { get; set; }

    [JsonProperty("extremeValues")]
    public ScalarBelief ExtremeValues { get; set; }

    [JsonProperty("identifyLargerNumber")]
    public ScalarBelief IdentifyLargerNumber { get; set; }

    [JsonProperty("identifyLargestElement")]
    public ScalarBelief IdentifyLargestElement { get; set; }
}

public class ProbabilisticBelief
{
    public double Good { get; set; }
    public double Bad { get; set; }
}

public class ProbabilisticBeliefs
{
    [JsonProperty("masteryOfSortingAlgorithm")]
    public ProbabilisticBelief MasteryOfSortingAlgorithm { get; set; }

    [JsonProperty("learnBehaviourOfSortingAlgorithms")]
    public ProbabilisticBelief LearnBehaviourOfSortingAlgorithms { get; set; }

    [JsonProperty("learnBasicSkills")]
    public ProbabilisticBelief LearnBasicSkills { get; set; }

    [JsonProperty("bubbleSort")]
    public ProbabilisticBelief BubbleSort { get; set; }

    [JsonProperty("selectionSort")]
    public ProbabilisticBelief SelectionSort { get; set; }

    [JsonProperty("insertionSort")]
    public ProbabilisticBelief InsertionSort { get; set; }

    [JsonProperty("swapElements")]
    public ProbabilisticBelief SwapElements { get; set; }

    [JsonProperty("stepOver")]
    public ProbabilisticBelief StepOver { get; set; }

    [JsonProperty("foundNewMin")]
    public ProbabilisticBelief FoundNewMin { get; set; }

    [JsonProperty("noNewMin")]
    public ProbabilisticBelief NoNewMin { get; set; }

    [JsonProperty("swapFurtherForwards")]
    public ProbabilisticBelief SwapFurtherForwards { get; set; }

    [JsonProperty("insertElement")]
    public ProbabilisticBelief InsertElement { get; set; }

    [JsonProperty("identifySmallerNumber")]
    public ProbabilisticBelief IdentifySmallerNumber { get; set; }

    [JsonProperty("identifySmallestElement")]
    public ProbabilisticBelief IdentifySmallestElement { get; set; }

    [JsonProperty("numberComparison")]
    public ProbabilisticBelief NumberComparison { get; set; }

    [JsonProperty("extremeValues")]
    public ProbabilisticBelief ExtremeBeliefs { get; set; }

    [JsonProperty("identifyLargerNumber")]
    public ProbabilisticBelief IdentifyLargerNumber { get; set; }

    [JsonProperty("identifyLargestElement")]
    public ProbabilisticBelief IdentifyLargestElement { get; set; }
}

public class ProbabilisticValueConverter : JsonConverter<ProbabilisticBelief>
{
    public override ProbabilisticBelief ReadJson(JsonReader reader, Type objectType, ProbabilisticBelief existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var array = JArray.Load(reader);
        return new ProbabilisticBelief
        {
            Good = (double)array[0],
            Bad = (double)array[1]
        };
    }

    public override void WriteJson(JsonWriter writer, ProbabilisticBelief value, JsonSerializer serializer)
    {
        var array = new JArray { value.Good, value.Bad };
        array.WriteTo(writer);
    }
}
