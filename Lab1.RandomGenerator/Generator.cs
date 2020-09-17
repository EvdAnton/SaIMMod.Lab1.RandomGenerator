using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab1.RandomGenerator
{
    public class IntervalWithCountOfNumbers
    {
        public float Range { get; set; }
        public int InnerNumbers { get; set; }
    }

    public class Generator
    {

        public int Period { get; private set; }
        public List<int> APeriod { get; private set; }
        public float Dispersion { get; private set; }

        public float Expectation { get; private set; }

        public float SigmaLimit { get; private set; }

       
        private uint From { get; set; }
        private uint To { get; set; }
        
        public List<List<float>> Result { get; }
        
        private readonly uint _startValueX0;
        private readonly uint _countOfElements;
        private readonly uint _multiplier;
        private readonly uint _mod;

        public Generator(uint startValueX0, uint multiplier, uint mod, uint from,
            uint to, uint countOfElements)
        {
            _countOfElements = countOfElements;
            _startValueX0 = startValueX0;
            _multiplier = multiplier;
            _mod = mod;

            Result = new List<List<float>>()
            {
                new List<float>()
            };
        
            From = from;
            To = to;
        }

        public List<IntervalWithCountOfNumbers> GetResult()
        {
            GenerateValue(_startValueX0);

            Expectation = GetExpectation();
            Dispersion = GetDispersion();
            SigmaLimit = GetSigmaLimit();
            Period = GetPeriod();
            APeriod = GetAllAperiods().ToList();

            return GetFrequency();
        }

        private void GenerateValue(uint currentValue)
        {
            for(var i = 0; i < _countOfElements; i++)
            {
                var nextValue = _multiplier * currentValue % _mod;
                var addedValue = (float)nextValue / _mod;

                if (Result.Last().Count > 0 && Result.Last().First().Equals(addedValue))
                {
                    Result.Add(new List<float>());
                }

                Result.Last().Add(addedValue);
                
                currentValue = nextValue;
            }
        }

        private float GetExpectation()
        {
            var count = Result.Count;
            if (count == 0)
            {
                return 0;
            }

            float expectation = 0;

            foreach (var result in Result)
            {
                expectation += GetExpectationForOneSequence(result);
            }

            expectation /=  count;

            return expectation;
        }

        private float GetExpectationForOneSequence(List<float> result)
        {
            var count = result.Count;
            if (count == 0)
            {
                return 0;
            }
            
            var sum = result.Sum();
            var expectation = sum / count;
            
            return expectation;
        }

        private float GetDispersion()
        {
            var count = Result.Count;
            if (count == 0)
            {
                return 0;
            }

            float expectation = 0;

            foreach (var result in Result)
            {
                expectation += GetDispersionForOneSequence(result);
            }

            expectation /= count;

            return expectation;
        }

        private float GetDispersionForOneSequence(List<float> result)
        {
            var expectation = Expectation;
            
            if (expectation.Equals(0))
            {
                return 0;
            }

            var count = result.Count; 
            var sum = (float)result.Sum(value => Math.Pow(value - expectation, 2));
            var dispersion = sum / count;
            
            return dispersion;
        }

        private float GetSigmaLimit()
        {
            var sigmaLimit = (float)Math.Sqrt(Dispersion);
            
            return sigmaLimit;
        }

        private List<float> GetAllNumbersFromResult(List<List<float>> results)
        {
            var resultList = new List<float>();

            foreach(var result in results)
            {
                resultList.AddRange(result);
            }

            return resultList;
        }

        private List<IntervalWithCountOfNumbers> GetFrequency()
        {
            var result = GetAllNumbersFromResult(Result);

            var countOfInterval = 20;
            float currentInterval = From;
            float previousInterval = From;
            var range = To - From;
            var step = (float)range / countOfInterval;

            var intervals = new List<IntervalWithCountOfNumbers>();

            while(currentInterval <= To)
            {
                previousInterval = currentInterval;
                currentInterval += step;

                var countOfNumersInInterval = result.Count(value => value < currentInterval && value >= previousInterval);

                intervals.Add(new IntervalWithCountOfNumbers 
                {
                    Range = currentInterval, 
                    InnerNumbers = countOfNumersInInterval 
                });
            }

            return intervals;
        }

        private int GetPeriod()
        {
            var countOfElementsInEveryPeriod = Result.Select(result => result.Count).ToList();

            int period = 0;
            int maxMatches = 0;

            foreach (var item in countOfElementsInEveryPeriod)
            {
                int matches = countOfElementsInEveryPeriod.Count(i => i == item);

                if (matches > maxMatches)
                {
                    maxMatches = matches;
                    period = item;
                }
            }

            return period;
        }

        private IEnumerable<int> GetAllAperiods()
        {
            if(Period == default)
            {
                Period = GetPeriod();
            }

            var countOfElementsInEveryPeriod = Result.Select(result => result.Count).ToList();

            var aperiods = countOfElementsInEveryPeriod.Where(period => period != Period);

            return aperiods;
        }

        public List<float> GetValuesFromPeriod(int countOfElementsInPeriod)
        {
            var period = Result.FirstOrDefault(result => result.Count == countOfElementsInPeriod);

            return period ?? new List<float>();
        }
    }
}