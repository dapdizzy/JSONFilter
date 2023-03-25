namespace JSONFilter
{
    public class Filter
    {
        private const string fieldPath = "fieldPath";
        private const string op = "op";
        private const string value = "value";

        public static bool DoesSatisfyCriteria(string json, string jsonCriteria, out string? errorMessage)
        {
            if (json == null)
            {
                throw new ArgumentNullException("json");
            }

            if (jsonCriteria == null)
            {
                throw new ArgumentNullException(nameof(jsonCriteria));
            }

            errorMessage = null;

            var result = false;

            try
            {
                var jObject = Newtonsoft.Json.Linq.JObject.Parse(json);
                var criteriaJToken = Newtonsoft.Json.Linq.JToken.Parse(jsonCriteria);
                var criteriaJArray = criteriaJToken as Newtonsoft.Json.Linq.JArray;
                if (criteriaJArray == null)
                {
                    errorMessage = $"{nameof(jsonCriteria)} must be JSON Array";
                    return false;
                }

                foreach (var jToken in criteriaJArray)
                {
                    var criteriaJObject = jToken as Newtonsoft.Json.Linq.JObject;
                    if (criteriaJObject == null)
                    {
                        errorMessage = $"Criterions must passed in {nameof(jsonCriteria)} array must be JSON objects";
                        return false;
                    }

                    if (criteriaJObject.TryGetValue(fieldPath, out var fieldValueJToken) &&
                        criteriaJObject.TryGetValue(op, out var operatorValueJToken) &&
                        criteriaJObject.TryGetValue(value, out var valueValueJObject))
                    {
                        var jsonPath = fieldValueJToken.ToString();
                        if (string.IsNullOrEmpty(jsonPath.ToString()))
                        {
                            errorMessage = $"{fieldPath} specied in criterion entry must be present";
                            return false;
                        }

                        var matchingJToken = jObject.SelectToken(jsonPath);
                        if (matchingJToken == null)
                        {
                            errorMessage = $"JSON does not contain path {jsonPath}";
                            return false;
                        }

                        var comparisonOperator = operatorValueJToken.ToString();
                        switch (comparisonOperator)
                        {
                            case "EQ": // Равно
                                break;
                            case "LT": // Меньше
                                break;
                            case "GT": // Больше
                                break;
                            case "Contains": // Содержит
                                switch (matchingJToken.Type)
                                {
                                    case Newtonsoft.Json.Linq.JTokenType.String:
                                        switch (valueValueJObject.Type)
                                        {
                                            case Newtonsoft.Json.Linq.JTokenType.Array:
                                                var allowedValuesHashSet = valueValueJObject.ToArray().Select(val => val.ToString()).ToHashSet();
                                                result = allowedValuesHashSet.Contains(matchingJToken.ToString());
                                                return result;
                                            default:
                                                errorMessage = $"Value type for criterion with path {jsonPath} must have Array type. Actually it has {valueValueJObject.Type} type";
                                                return false;
                                        }
                                    case Newtonsoft.Json.Linq.JTokenType.Array:
                                        switch (valueValueJObject.Type)
                                        {
                                            case Newtonsoft.Json.Linq.JTokenType.Array:
                                                var requiredValues = valueValueJObject.ToArray().Select(val => val.ToString()).ToHashSet();
                                                var actualValues = matchingJToken.ToArray().Select(val => val.ToString()).ToHashSet();
                                                // All values passed in as values should be containe within the actual values
                                                result = requiredValues.All(value => actualValues.Contains(value));
                                                return result;
                                            default:
                                                errorMessage = $"Path {jsonPath}: Value for Contains comparison operator for checked value of type Array, should also be Array. Instead type {valueValueJObject.Type} was found";
                                                return false;
                                        }
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
                return false;
            }

            return result;
        }
    }
}