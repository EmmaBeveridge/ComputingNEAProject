using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace Game1.ML
{
    public class SentimentData
    {
        /// <summary>
        /// Column containing sentiment text
        /// </summary>
        [LoadColumn(0)]
        public string SentimentText;

        /// <summary>
        /// Boolean describing if sentiment is positive or negative. 
        /// </summary>
        [LoadColumn(1), ColumnName("Label")]
        public bool Sentiment;
    }

    public class SentimentPrediction : SentimentData
    {

        /// <summary>
        /// Boolean describing if the sentiment is positive or negative.
        /// </summary>
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        /// <summary>
        /// Score calibrated to the probability of the text having a positive sentiment.
        /// </summary>
        public float Probability { get; set; }


        /// <summary>
        /// Constructor for new SentimentPrediction object
        /// </summary>
        /// <param name="_prediction"></param>
        /// <param name="_probability"></param>
        public SentimentPrediction(bool _prediction, float _probability)
        {
            Prediction = _prediction;
            Probability = _probability;

        }

        public SentimentPrediction() { }


    }
}
