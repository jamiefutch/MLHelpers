using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Transforms.Text;

namespace MLHelpers.Text
{
    public class TextNormalizer : IDisposable
    {
        private readonly MLContext _mlContext;
        private readonly List<TextData> _emptySamplesList;
        private readonly IDataView _emptyDataView;
        
        // text normalization
        private readonly TextNormalizingEstimator _normTextPipeline;
        private readonly TextNormalizingTransformer _normTextTransformer;
        private readonly PredictionEngine<TextData, TransformedTextData> _predictionEngine;
        
        public TextNormalizer()
        {
            _mlContext = new MLContext();
            _emptySamplesList = new List<TextData>();
            _emptyDataView = _mlContext.Data.LoadFromEnumerable(_emptySamplesList);
            
            // text normalizer
            _normTextPipeline = _mlContext.Transforms.Text.NormalizeText("NormalizedText", "Text",
                TextNormalizingEstimator.CaseMode.Lower,
                keepDiacritics: false,
                keepPunctuations: false,
                keepNumbers: false);
            _normTextTransformer = _normTextPipeline.Fit(_emptyDataView);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<TextData, TransformedTextData>(_normTextTransformer);
        }

        public string NormalizeText(string text)
        {                               
            var data = new TextData() { Text = text };
            var prediction = _predictionEngine.Predict(data);
            return prediction.NormalizedText;
        }


        private class TextData
        {
            public string Text { get; set; }
        }

        private class TransformedTextData : TextData
        {
            public string NormalizedText { get; set; }
        }


        public void Dispose()
        {
            _predictionEngine?.Dispose();
        }
    }    
}
