using System;
using System.IO;
using JsonParser.Objects.Read;
using Newtonsoft.Json;

namespace JsonParser.Services;

public class JsonService
{
      public static MarkingTask? Read(string fileName)
    {
        string textFromJsonFile = File.ReadAllText(fileName);
        try
        {
            MarkingTask? task = JsonConvert.DeserializeObject<MarkingTask>(textFromJsonFile);
            return task;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static void Write(Objects.Write.ReportTask reportTask, string fileName)
    {
        string textFromObject = JsonConvert.SerializeObject(reportTask, Formatting.Indented);
        File.WriteAllText(fileName, textFromObject);
    }

    public static bool HasEmptyObjectOrFields(MarkingTask? markingTask)
    {
        if (markingTask == null)
        {
            return true;
        }
        else
        {
            if (markingTask.Nomenclatures == null)
            {
                return true;
            }
            else
            {
                foreach (var nomenclature in markingTask.Nomenclatures)
                {
                    if (string.IsNullOrEmpty(nomenclature.Name) ||
                        string.IsNullOrEmpty(nomenclature.Gtin) ||
                        nomenclature.ArtCode == null ||
                        string.IsNullOrEmpty(nomenclature.Description) ||
                        nomenclature.Attributes == null)
                    {
                        return true;
                    }
                    else
                    {
                        foreach (Objects.Read.Attribute attribute in nomenclature.Attributes)
                        {
                            if (attribute.Code == null ||
                                string.IsNullOrEmpty(attribute.Value))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }
}