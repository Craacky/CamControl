using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CC.Core.Devices.Impl;
using CC.Core.Models.Base;
using CC.Data.Entities.Tasks;
using JsonParser.Services;
using Attribute = CC.Data.Entities.Tasks.Attribute;

namespace CC.Core.Services.Impl;

public class NomenclatureService : ObservableObject
{
    private bool _isStartLodingNomenclatureAsync;


    private List<Nomenclature> _nomenclatures = null!;

    public List<Nomenclature> Nomenclatures
    {
        get => _nomenclatures;
        set
        {
            _nomenclatures = value;
            OnPropertyChanged(nameof(Nomenclatures));
        }
    }


    public JsonService JsonService { get; set; } = null!;
    public LocalDb LocalDbService { get; set; }


    public NomenclatureService(LocalDb localDbService)
    {
        LocalDbService = localDbService;

        _isStartLodingNomenclatureAsync = false;
        Nomenclatures = new List<Nomenclature>();

        // ensure JsonService is initialized to avoid NRE on startup
        JsonService = new JsonService();
    }


    private IEnumerable<string> GetNewJsonFileName(DirectoryInfo directoryInfo)
    {
        var inFileNamesFromDirectory = directoryInfo.GetFiles("*.in");
        foreach (var inFileName in inFileNamesFromDirectory)
        {
            var guidFormInFileName = inFileName.Name.Split(".")[0];
            var isContainInFileNameGuid = Guid.TryParse(guidFormInFileName, out Guid guid);
            if (!isContainInFileNameGuid) continue;
            {
                var jsonFileName = inFileName.FullName.Replace(".in", ".json");
                var isExistJsonFile = File.Exists(jsonFileName);
                if (isExistJsonFile)
                {
                    yield return jsonFileName;
                }
            }
        }
    }

    private IEnumerable<Nomenclature> ConvertJsonMarkingTaskToDbNomenclature(JsonParser.Objects.Read.MarkingTask task,
        string guid)
    {
        IEnumerable<Nomenclature> nomenclatures = new List<Nomenclature>();

        foreach (var jsonNomenclature in task.Nomenclatures!)
        {
            Nomenclature dbNomenclature = new()
            {
                Guid = Guid.Parse(guid),
                Code = jsonNomenclature.Code,
                ExporterCode = jsonNomenclature.ExporterCode,
                GrpCode = jsonNomenclature.GrpCode,
                Name = jsonNomenclature.Name,
                Gtin = jsonNomenclature.Gtin,
                ArtCode = jsonNomenclature.ArtCode,
                Description = jsonNomenclature.Description,
                Attributes = new List<Attribute>()
            };

            foreach (var jsonAttribute in jsonNomenclature.Attributes!)
            {
                Attribute dbAttribute = new()
                {
                    Code = (int)jsonAttribute.Code!,
                    Value = jsonAttribute.Value,
                };
                dbNomenclature.Attributes.Add(dbAttribute);
            }

            nomenclatures = nomenclatures.Append(dbNomenclature);
        }

        return nomenclatures;
    }

    private async void CheckingOnDeleteNomenclature(string directoryPath)
    {
        await Task.Run(async () =>
        {
            var inFileNamesFromDirectory = Directory.GetFiles(directoryPath, "*.in");
            for (var i = 0; i < Nomenclatures.Count; i++)
            {
                var isExistMarkingTaskInDb = inFileNamesFromDirectory.Any(a =>
                    a.ToUpper().Contains(Nomenclatures[i].Guid.ToString().ToUpper()));
                if (!isExistMarkingTaskInDb)
                {
                    var deleteDbNomenclatures =
                        LocalDbService.NomenclatureDataService.GetAllWithInclude(mt => mt.Guid == Nomenclatures[i].Guid,
                            n => n.Attributes);
                    if (deleteDbNomenclatures != null)
                    {
                        foreach (var deleteDbNomenclature in deleteDbNomenclatures)
                        {
                            await LocalDbService.NomenclatureDataService.DeleteAsync(deleteDbNomenclature.Id);
                        }
                    }

                    Nomenclatures.Remove(Nomenclatures[i]);
                    Nomenclatures = new List<Nomenclature>(Nomenclatures);
                }
            }
        });
    }


    public void StartLodingNomenclatureAsync(string directoryPath)
    {
        Task.Run(async () =>
        {
            if (_isStartLodingNomenclatureAsync)
            {
                StopLoadingNomenclatures();
            }

            _isStartLodingNomenclatureAsync = true;

            DirectoryInfo directoryInfo;
            //if(!directoryInfo.Exists)
            //{
            //    MessageBox.Show($"При попытки подключиться к папке с номенклатурами произошла ошибка.\n" +
            //                    $"{directoryPath}"+
            //                    $"Указанная папка либо отсутсутвует либо отсутвует подключение к ней.\n" +
            //                    $"Новые номенклатуры не будут загружены");
            //}

            while (_isStartLodingNomenclatureAsync)
            {
                directoryInfo = new DirectoryInfo(directoryPath);
                if (directoryInfo.Exists)
                {
                    foreach (string jsonFileName in GetNewJsonFileName(directoryInfo))
                    {
                        var jsonMarkingTask = JsonService.Read(jsonFileName)!;
                        var isFullJsonMarkingTask = !JsonService.HasEmptyObjectOrFields(jsonMarkingTask);
                        if (isFullJsonMarkingTask)
                        {
                            var guidFormJsonFileName = jsonFileName.Split(".json")[0].Split("\\")[^1];
                            var newDbNomenclatures =
                                ConvertJsonMarkingTaskToDbNomenclature(jsonMarkingTask!, guidFormJsonFileName);
                            foreach (var newDbNomenclature in newDbNomenclatures)
                            {
                                var oldObject = LocalDbService.NomenclatureDataService
                                    .GetAllWithInclude(n => n.Code == newDbNomenclature.Code, n => n.Attributes)!
                                    .FirstOrDefault();
                                if (oldObject == null)
                                {
                                    await LocalDbService.NomenclatureDataService.CreateAsync(newDbNomenclature);
                                }
                                else
                                {
                                    await LocalDbService.NomenclatureDataService.UpdateAsync(oldObject.Id,
                                        newDbNomenclature);
                                }

                                var isNewNomenclatureGtin = Nomenclatures.All(n => n.Gtin != newDbNomenclature.Gtin);
                                if (isNewNomenclatureGtin)
                                {
                                    Nomenclatures.Add(newDbNomenclature);
                                    Nomenclatures = new List<Nomenclature>(Nomenclatures);
                                }
                            }
                        }
                    }

                    CheckingOnDeleteNomenclature(directoryPath);
                }

                Thread.Sleep(100000);

                StopLoadingNomenclatures();
            }
        });
    }

    public void StopLoadingNomenclatures()
    {
        _isStartLodingNomenclatureAsync = false;
    }
}