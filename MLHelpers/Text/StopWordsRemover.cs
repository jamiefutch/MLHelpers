﻿using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Transforms.Text;

namespace MLHelpers.Text
{
    public class StopWordsRemover : IDisposable
    {

        private readonly MLContext _mlContext;
        private readonly List<TextData> _emptySamplesList;
        private readonly IDataView _emptyDataView;
        
        // stop words
        private Microsoft.ML.Data.EstimatorChain<StopWordsRemovingTransformer> _stopWordsTextPipeline;
        private readonly Microsoft.ML.Data.TransformerChain<StopWordsRemovingTransformer> _textPipeline;
        private readonly PredictionEngine<TextData, TransformedTextData> _predictionEngine;


        public StopWordsRemover()
        {
            _mlContext = new MLContext();
            _emptySamplesList = new List<TextData>();
            _emptyDataView = _mlContext.Data.LoadFromEnumerable(_emptySamplesList);

            // stop words
            _stopWordsTextPipeline = _mlContext.Transforms.Text.TokenizeIntoWords("Words", "Text")
                .Append(_mlContext.Transforms.Text.RemoveDefaultStopWords("WordsWithoutStopWords", "Words", language: StopWordsRemovingEstimator.Language.English));
            _textPipeline = _stopWordsTextPipeline.Fit(_emptyDataView);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<TextData, TransformedTextData>(_textPipeline);
        }

        public string RemoveStopWords(string comment)
        {
            var data = new TextData() { Text = comment };
            var prediction = _predictionEngine.Predict(data);

            string retComment;
            if (prediction.WordsWithoutStopWords != null)
            {
                retComment = string.Join(" ", prediction.WordsWithoutStopWords);
            }
            else
            {
                retComment = comment;
            }   
            return retComment;
        }


        private class TextData
        {
            public string Text { get; set; }
        }

        private class TransformedTextData : TextData
        {
            public string[] WordsWithoutStopWords { get; set; }
        }

        public void Dispose()
        {
            _predictionEngine?.Dispose();
        }
    }
}
