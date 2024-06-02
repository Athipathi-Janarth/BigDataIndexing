using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;

public class JsonValidator
{
    
    private static JSchema _schema;

    public JsonValidator(string schemaFilePath)
    {
        var schemaText = File.ReadAllText(schemaFilePath);
        _schema = JSchema.Parse(schemaText);
    }

    public  bool ValidateJson(string json, out IList<string> validationErrors)
    {
        JObject jsonObj = JObject.Parse(json);
        bool isValid = jsonObj.IsValid(_schema, out validationErrors);
        Console.WriteLine(validationErrors);
        return isValid;
    }
}
