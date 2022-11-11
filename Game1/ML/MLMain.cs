using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using static Microsoft.ML.DataOperationsCatalog;
using System.Text.RegularExpressions;

namespace Game1.ML
{
    internal static class MLMain

    //https://learn.microsoft.com/en-us/dotnet/machine-learning/tutorials/sentiment-analysis

    {
        static string _dataPath = Path.Combine(Environment.CurrentDirectory, "MLData", "yelp_labelled.txt");
        static MLContext mlContext;
        static ITransformer model;



        public static void BuildML()
        {
            mlContext = new MLContext();
            TrainTestData splitDataView = LoadData(mlContext);
            model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);
            Evaluate(mlContext, model, splitDataView.TestSet);

        }

        static TrainTestData  LoadData(MLContext mlContext)
        {
            //loads data, splits dataset into train and test data and returns train and test datasets

            IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(_dataPath, hasHeader: false);
            TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            return splitDataView;

        }

        public static ITransformer BuildAndTrainModel (MLContext mlContext, IDataView splitTrainSet)
        {
            var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText)).Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
            var model = estimator.Fit(splitTrainSet);
            return model;


        }

        static void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        {
            IDataView predictions = model.Transform(splitTestSet);
            CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");
            Console.WriteLine();
            Console.WriteLine("Model quality metrics evaluation");
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
            Console.WriteLine("=============== End of model evaluation ===============");
        }

        public static SentimentPrediction Predict(SentimentData sentimentData)
        {
            PredictionEngine<SentimentData, SentimentPrediction> predictionFunc = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            var prediction = predictionFunc.Predict(sentimentData);
            Console.WriteLine($"sentiment: {sentimentData.SentimentText} prob:{prediction.Probability}");
            return new SentimentPrediction(_prediction: Convert.ToBoolean(prediction.Prediction), _probability: prediction.Probability);



        }
        
        public static SentimentPrediction PredictWithSubject(SentimentData sentimentData)
        {
            SentimentPrediction prediction = Predict(sentimentData);

            Regex regex = new Regex(@"((You)|(you)|(U)|(u))");
            if (!regex.IsMatch(sentimentData.SentimentText))
            {
                prediction.Prediction = true; //does not penalise a negative opinion if not directed at the player
            }

            return prediction;



        }





    }
}
