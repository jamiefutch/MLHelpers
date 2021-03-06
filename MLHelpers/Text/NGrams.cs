﻿using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;

// ReSharper disable IdentifierTypo

namespace MLHelpers.Text
{
    public class NGrams
    {
        MLContext _mlContext;
        private EstimatorChain<NgramExtractingTransformer> _textPipeline;
        

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="ngramLength"></param>
        public NGrams(int ngramLength = 3)
        {
            _mlContext = new MLContext();
            _textPipeline = _mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "Text")
                // 'ProduceNgrams' takes key type as input. Converting the tokens into key type using 'MapValueToKey'.
                .Append(_mlContext.Transforms.Conversion.MapValueToKey("Tokens"))
                .Append(_mlContext.Transforms.Text.ProduceNgrams("NgramFeatures", "Tokens",
                    ngramLength: ngramLength,
                    useAllLengths: false,
                    weighting: NgramExtractingEstimator.WeightingCriteria.Tf));
            
        }

        #region FullNgramModel
        public List<NGramModel> GenerateNGrams(string inputString)
        {

            var ngList = new List<string>
            {
                inputString
            };
            return GenerateNGrams(ngList.ToArray());
        }
        
        public List<NGramModel> GenerateNGrams(List<string> inputStrings)
        {   
            return GenerateNGrams(inputStrings.ToArray());
        }
        
        public List<NGramModel> GenerateNGrams(string[] inputStrings, int ngramLength = 3)
        {
            var retList = new List<NGramModel>();

            MLContext mlContext = new MLContext();
            EstimatorChain<NgramExtractingTransformer> textPipeline = mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "Text")
                // 'ProduceNgrams' takes key type as input. Converting the tokens into key type using 'MapValueToKey'.
                .Append(mlContext.Transforms.Conversion.MapValueToKey("Tokens"))
                .Append(mlContext.Transforms.Text.ProduceNgrams("NgramFeatures", "Tokens",
                    ngramLength: ngramLength,
                    useAllLengths: false,
                    weighting: NgramExtractingEstimator.WeightingCriteria.Tf));

            var strings = StringsToTextDataList(inputStrings);
            var dataview = mlContext.Data.LoadFromEnumerable(strings);

            var textTransformer = textPipeline.Fit(dataview);
            var transformedDataView = textTransformer.Transform(dataview);
            var predictionEngine = mlContext.Model.CreatePredictionEngine<TextData, TransformedTextData>(textTransformer);
            var prediction = predictionEngine.Predict(strings[0]);
            VBuffer<ReadOnlyMemory<char>> slotNames = default;
            transformedDataView.Schema["NgramFeatures"].GetSlotNames(ref slotNames);
            // ReSharper disable once InconsistentNaming
            var NgramFeaturesColumn = transformedDataView.GetColumn<VBuffer<float>>(transformedDataView.Schema["NgramFeatures"]);
            var slots = slotNames.GetValues();
            
            foreach (var featureRow in NgramFeaturesColumn)
            {
                //Console.Write($"row:{rowCount}\t");
                foreach (var item in featureRow.Items())
                {
                    var ng = new NGramModel
                    {
                        NGramString = slots[item.Key].ToString()
                        , NGramArray = slots[item.Key].ToString().Split('|')
                    };
                    retList.Add(ng);
                }
            }

            return retList;
        }
        #endregion


        #region Ngrams - just strings
        public List<string> GenerateNGramsStrings(string inputString)
        {

            var ngList = new List<string>
            {
                inputString
            };
            return GenerateNGramsStrings(ngList.ToArray());
        }
        
        public List<string> GenerateNGramsStrings(List<string> inputStrings)
        {   
            return GenerateNGramsStrings(inputStrings.ToArray());
        }
        
        public List<string> GenerateNGramsStrings(string[] inputStrings, int ngramLength = 3)
        {
            //MLContext mlContext = new MLContext();
            // EstimatorChain<NgramExtractingTransformer> textPipeline = mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "Text")
            //     // 'ProduceNgrams' takes key type as input. Converting the tokens into key type using 'MapValueToKey'.
            //     .Append(mlContext.Transforms.Conversion.MapValueToKey("Tokens"))
            //     .Append(mlContext.Transforms.Text.ProduceNgrams("NgramFeatures", "Tokens",
            //         ngramLength: ngramLength,
            //         useAllLengths: false,
            //         weighting: NgramExtractingEstimator.WeightingCriteria.Tf));

            var strings = StringsToTextDataList(inputStrings);
            var dataview = _mlContext.Data.LoadFromEnumerable(strings);

            var textTransformer = _textPipeline.Fit(dataview);
            var transformedDataView = textTransformer.Transform(dataview);
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<TextData, TransformedTextData>(textTransformer);
            var prediction = predictionEngine.Predict(strings[0]);
            VBuffer<ReadOnlyMemory<char>> slotNames = default;
            transformedDataView.Schema["NgramFeatures"].GetSlotNames(ref slotNames);
            
            // ReSharper disable once InconsistentNaming
            var NgramFeaturesColumn = transformedDataView.GetColumn<VBuffer<float>>(transformedDataView.Schema["NgramFeatures"]);
            var slots = slotNames.GetValues();
            
            var retList = new List<string>();
            foreach (var featureRow in NgramFeaturesColumn)
            {
                //Console.Write($"row:{rowCount}\t");
                foreach (var item in featureRow.Items())
                {
                    retList.Add(slots[item.Key].ToString());
                }
            }
            return retList;
        }
        #endregion
        
        
        
        
        
        private TextData StringToTextData(string inputString)
        {
            var td = new TextData
            {
                Text = inputString
            };
            return td;
        }

        private List<TextData> StringsToTextDataList(List<string> inputStrings)
        {
            return StringsToTextDataList(inputStrings.ToArray());
        }

        private List<TextData> StringsToTextDataList(string[] inputStrings)
        {
            var tdList = new List<TextData>();
            foreach (var inputString in inputStrings)
            {
                tdList.Add(StringToTextData(inputString));
            }

            return tdList;
        }

        private class TextData
        {
            public string Text { get; set; }
        }

        private class TransformedTextData : TextData
        {
            public float[] NgramFeatures { get; set; }
        }
    }
    public class NGramModel
    {
        public string NGramString { get; set; }

        public string[] NGramArray { get; set; }
                
        public override string ToString()
        {
            return NGramString;
        }
    }
}
