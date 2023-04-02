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

        /// <summary>
        /// Object of MLContext class. Initialisation creates a new ML.NET environment and acts as a start point for ML.NET operations. 
        /// </summary>
        static MLContext mlContext;
        static ITransformer model;


        /// <summary>
        /// Loads and splits data using LoadData method. Calls BuildAndTrainModel and Evaluate methods to prepare and evaluate model. 
        /// </summary>
        public static void BuildML()
        {
            mlContext = new MLContext();
            TrainTestData splitDataView = LoadData(mlContext);
            model = BuildAndTrainModel(mlContext, splitDataView.TrainSet);
            Evaluate(mlContext, model, splitDataView.TestSet);

        }

        /// <summary>
        /// Loads dataset and splits into training and testing data. Returns the train and test datasets.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns></returns>
        static TrainTestData  LoadData(MLContext mlContext)
        {
            //loads data, splits dataset into train and test data and returns train and test datasets

            IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(_dataPath, hasHeader: false);
            TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            return splitDataView;

        }


        /// <summary>
        /// Extracts and transforms the data by converting sentiment text data to numeric key type Features column. The SDCA logistic regression binary trainer is used as the classification training algorithm to categorise sentiment text data as either positive or negative. The model is then trained and returned.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="splitTrainSet"></param>
        /// <returns></returns>
        public static ITransformer BuildAndTrainModel (MLContext mlContext, IDataView splitTrainSet)
        {
            var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.SentimentText)).Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
            var model = estimator.Fit(splitTrainSet);
            return model;


        }


        /// <summary>
        /// Quality check method. Uses the test dataset and a binary classification evaluator to evaluate the accuracy of the model’s predictions.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="model"></param>
        /// <param name="splitTestSet"></param>
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

        /// <summary>
        /// Returns new SentimentPrediction object containing prediction for supplied sentiment data. Creates a new PredictionEngine (convenience API to perform prediction on single data instance) object to evaluate sentiment of supplied data.
        /// </summary>
        /// <param name="sentimentData"></param>
        /// <returns></returns>
        public static SentimentPrediction Predict(SentimentData sentimentData)
        {
            PredictionEngine<SentimentData, SentimentPrediction> predictionFunc = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            var prediction = predictionFunc.Predict(sentimentData);
            Console.WriteLine($"sentiment: {sentimentData.SentimentText} prob:{prediction.Probability}");
            return new SentimentPrediction(_prediction: Convert.ToBoolean(prediction.Prediction), _probability: prediction.Probability);



        }

        /// <summary>
        /// Imperfect data set means the model has a tendency to penalise negative opinions even if they are not directed towards the subject. This method contextualises the sentiment text by checking for the subject of the sentence. If the negative sentiment is directed towards the person, then the interaction sentiment is evaluated as negative.
        /// </summary>
        /// <param name="sentimentData"></param>
        /// <returns></returns>
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
