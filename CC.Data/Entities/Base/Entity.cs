using System;

namespace CC.Data.Entities.Base;

public class Entity : ObservableObject
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; } = DateTime.Now;
}