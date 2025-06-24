using BlazorDexie.Database;
using BlazorDexie.JsModule;
using Bogus;
using Microsoft.JSInterop;
using Tetramorph.Doctor.Database.Models;

namespace Tetramorph.Doctor.Database;

public class DatabaseContext : Db
{
    public Store<Client, int> Clients { get; set; } = new($"++{nameof(Client.Id)}", nameof(Client.Name),
        nameof(Client.CardNumber), nameof(Client.DiagnosisIds), nameof(Client.Sex), nameof(Client.BirthDate));

    public Store<Diagnosis, int> DiagnosisNames { get; set; } =
        new($"++{nameof(Diagnosis.Id)}", nameof(Diagnosis.Name), nameof(Diagnosis.Code));

    public Store<CalendarEvent, int> Events { get; set; } =
        new($"++{nameof(CalendarEvent.Id)}", nameof(CalendarEvent.EventType), nameof(CalendarEvent.Date),
            nameof(CalendarEvent.ClientId), nameof(CalendarEvent.Note), nameof(CalendarEvent.State),
            nameof(CalendarEvent.Drugs),
                nameof(CalendarEvent.Urls));

    public Store<Drug, int> Drugs { get; set; } =
        new($"++{nameof(Drug.Id)}", nameof(Drug.Name),nameof(Drug.UpName), nameof(Drug.Dosage));

    public Store<AppLog, int> Logs { get; set; } =
        new($"++{nameof(AppLog.Id)}", nameof(AppLog.Event), nameof(AppLog.Date));

    public DatabaseContext(IModuleFactory moduleFactory)
        : base("MainDatabase", 1, new DbVersion[] { }, moduleFactory)
    {
    }

    public static async Task MockClientsDatabase(IJSRuntime js)
    { 
        var moduleFactory = new EsModuleFactory(js);
        var db = new DatabaseContext(moduleFactory);
        
        
        Random rnd = new Random(DateTime.Now.Nanosecond);

        DateTime RandomDay(DateTime from, DateTime to)
        {
            var range = (to - from).TotalHours;
            return from.AddHours(rnd.NextDouble() * range).AddMinutes(rnd.Next(0, 3600));
        }

        async Task<string> GetRandomDrug(int count)
        {
            var id = rnd.Next(1, count-1);
            var drug = await db.Drugs.Where("Id").IsEqual(id).ToList();
            if (drug.First().Dosage.Count() == 0)
                return string.Empty;
            else
                return drug.First().Id + "|" + drug.First().Name + "|" + drug.First().Dosage.Shuffle().FirstOrDefault() + "|" + "1"  + "|" + rnd.Next(0, 3);

        }

       

        var diagnos = (await db.DiagnosisNames.ToList()).Select(v => v.Id);

        var faker = new Faker("ru");

        var mockUsers = Enumerable.Range(1, 200).Select(index => new Client()
        {
            CardNumber = index.ToString(),
            BirthDate = RandomDay(new DateTime(1960, 1, 1), DateTime.Today.AddYears(-18)).ToString(),
            DiagnosisIds = diagnos.Shuffle().Take(2).Select(var=>var.ToString()).ToArray(),
            Name = faker.Name.FullName()
        }).ToArray();

        await db.Clients.BulkAdd(mockUsers);

        var clients = await db.Clients.ToList();

        var drugsCount = await db.Drugs.Count();
        
        
        foreach (var client in clients)
        {
            await db.Events.Add(new CalendarEvent()
            {
                ClientId = client.Id,
                Note = "Test srtesa asdf asd as das d asd" + Environment.NewLine + "asdas das f sdgf 4tg 5h  jh6 j ",
                Urls = new string[]
                    { "https://mudblazor.com/components/iconbutton#simple-icon-buttons", "https://mudblazor.com/" },
                State = rnd.Next(0, 3),
                EventType = 0,
                Date = RandomDay(DateTime.Today, DateTime.Today).ToString()
            });

            for (int i = 0; i < 10; i++)
            {
                var evnType = rnd.Next(0, 7);
                
                List<string> drugs = new();
                    
                if (evnType == 1 || evnType == 2 || evnType == 3 || evnType == 5)
                {
                    var drugToAdd = rnd.Next(0, 4);
                    for (int j = 0; j < drugToAdd; j++)
                    {
                        var drugString = await GetRandomDrug(drugsCount);
                        if (drugString != string.Empty)
                        {
                            drugs.Add(drugString);
                        }
                    }
                }
                
                await db.Events.Add(new CalendarEvent()
                {
                    ClientId = client.Id,
                    Note = "Test srtesa asdf asd as das d asd" + Environment.NewLine +
                           "asdas das f sdgf 4tg 5h  jh6 j ",
                    Urls = new string[]
                        { "https://mudblazor.com/components/iconbutton#simple-icon-buttons", "https://mudblazor.com/" },
                    State = rnd.Next(0, 3),
                    EventType = evnType,
                    Drugs = drugs.ToArray(),
                    Date = RandomDay(DateTime.Today.AddDays(-200), DateTime.Today.AddDays(-1)).ToString()
                });
            }

            for (int i = 0; i < 10; i++)
            {
                var evnType = rnd.Next(0, 7);
                
                
                List<string> drugs = new();
                    
                if (evnType == 1 || evnType == 2 || evnType == 3 || evnType == 5)
                {
                    var drugToAdd = rnd.Next(0, 4);
                    for (int j = 0; j < drugToAdd; j++)
                    {
                        var drugString = await GetRandomDrug(drugsCount);
                        if (drugString != string.Empty)
                        {
                            drugs.Add(drugString);
                        }
                    }
                }
                
                await db.Events.Add(new CalendarEvent()
                {
                    ClientId = client.Id,
                    Note = "Test srtesa asdf asd as das d asd" + Environment.NewLine +
                           "asdas das f sdgf 4tg 5h  jh6 j ",
                    Urls = new string[]
                        { "https://mudblazor.com/components/iconbutton#simple-icon-buttons", "https://mudblazor.com/" },
                    State = rnd.Next(0, 3),
                    EventType = evnType,
                    Drugs = drugs.ToArray(),
                    Date = RandomDay(DateTime.Today.AddDays(1), DateTime.Today.AddDays(200)).ToString()
                });
            }
        }
    }
}