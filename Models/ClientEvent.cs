using System;
using System.Globalization;
using System.Linq;
using Heron.MudCalendar;
using MudBlazor;
using Tetramorph.Doctor.Components;
using Tetramorph.Doctor.Database.Models;
using Tetramorph.Doctor.Models;

public class ClientEvent : CalendarItem
{
    
    public int OriginalId { get; set; }
    public TypeofEvent EventType { get; set; }
    public DateTime Date { get; set; }
    public int ClientId { get; set; }
    public State State { get; set; }
    public string Note { get; set; } = string.Empty;
    
    public List<DrugsInput.DrugInputData> Drugs { get; set; } = new();
    public string[] Urls { get; set; } = new string[]{};


    public bool SetCalendarItem()
    {
        this.Start = Date;
        this.End = Date.AddMinutes(30);
        this.Text = EventTypeToStringFunc(EventType);

        return true;
    }

    public Color GetInfoColor()
    {
        switch (EventType)
        {
            case TypeofEvent.Consultation:
                return Color.Info;
            case TypeofEvent.StartTakingDrugs:
                return Color.Warning;
            case TypeofEvent.DosageChange:
                return Color.Warning;
            case TypeofEvent.EndTakingDrugs:
                return Color.Warning;
            case TypeofEvent.Analysis:
                return Color.Primary;
            case TypeofEvent.ProlongInjection:
                return Color.Warning;
            case TypeofEvent.Note:
                return Color.Success;
        }

        return Color.Error;
    }
    
    public Severity GetInfoSeverity()
    { 
        switch (EventType)
        {
            case TypeofEvent.Consultation:
                return Severity.Info;
            case TypeofEvent.StartTakingDrugs:
                return Severity.Warning;
            case TypeofEvent.DosageChange:
                return Severity.Warning;
            case TypeofEvent.EndTakingDrugs:
                return Severity.Warning;
            case TypeofEvent.Analysis:
                return Severity.Normal;
            case TypeofEvent.ProlongInjection:
                return Severity.Warning;
            case TypeofEvent.Note:
                return Severity.Success;
        }

        return Severity.Error;
    }
    
    public static string EventTypeToStringFunc(TypeofEvent eventType)
    {
        switch (eventType)
        {
            case TypeofEvent.Consultation:
                return "Консультация";
            case TypeofEvent.StartTakingDrugs:
                return "Начало приёма лекарств";
            case TypeofEvent.DosageChange:
                return "Смена дозировки";
            case TypeofEvent.EndTakingDrugs:
                return "Конец приёма лекарств";
            case TypeofEvent.Analysis:
                return "Анализ";
            case TypeofEvent.ProlongInjection:
                return "Инъекция пролонга";
            case TypeofEvent.Note:
                return "Записка";
            case TypeofEvent.All:
                return "Все события";
        }

        return "Нет информации";
    }
}

public enum TypeofEvent
{
    Consultation,
    StartTakingDrugs,
    DosageChange,
    EndTakingDrugs,
    Analysis,
    ProlongInjection,
    Note,
    All
}

public enum State
{
    Planned,
    Done,
    Cancelled
}