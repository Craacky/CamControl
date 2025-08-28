using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CC.Data.Entities.Base;

namespace CC.Data.Entities.Tasks;

public class Nomenclature : Entity
{
    public Guid Guid { get; set; }


    //Целое, уникальное в рамках экспортёра, код номенклатуры
    public int? Code { get; set; }

    //Целое, код экспортёра, сообщается из Маркировки
    public int? ExporterCode { get; set; }

    //Целое, код группы товаров
    public int? GrpCode { get; set; }
    //Текст, наименование товара


    private string? _name;

    [MaxLength(255)]
    public string? Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    //Текст 14 символов, GTIN товара
    private string? _gtin;

    [MaxLength(15)]
    public string? Gtin
    {
        get => _gtin;
        set
        {
            _gtin = value;
            OnPropertyChanged(nameof(Gtin));
        }
    }


    //Текст, может быть пусто, артикул товара 
    public int? ArtCode { get; set; }

    //Текст, может быть пусто, описание
    [MaxLength(255)] public string? Description { get; set; }


    //Список объектов, значения атрибута товара
    public virtual List<Attribute> Attributes { get; set; } = new List<Attribute>();


    public override string ToString()
    {
        return Name + " " + Attributes.FirstOrDefault(c => c.Code == 15)?.Value + " " +
               Attributes.FirstOrDefault(c => c.Code == 14)?.Value + "\n" + Gtin;
    }
}