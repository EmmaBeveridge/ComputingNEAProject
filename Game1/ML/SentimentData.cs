﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace Game1.ML
{
    public class SentimentData
    {
        [LoadColumn(0)]
        public string SentimentText;

        [LoadColumn(1), ColumnName("Label")]
        public bool Sentiment;
    }

    public class SentimentPrediction : SentimentData
    {

        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        public float Probability { get; set; }

        public float Score { get; set; }

        public SentimentPrediction(bool _prediction, float _probability)
        {
            Prediction = _prediction;
            Probability = _probability;

        }

        public SentimentPrediction() { }


    }
}