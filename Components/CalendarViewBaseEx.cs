

using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Heron.MudCalendar;

#nullable enable
namespace Tetramorph.Doctor.Components
{
    public abstract class CalendarViewBaseEx : ComponentBase
    {
        protected List<CalendarCell> Cells = new List<CalendarCell>();

        [CascadingParameter]
        public CustomCalendar Calendar { get; set; } = new CustomCalendar();

        protected override void OnParametersSet() => this.Cells = this.BuildCells();

        protected abstract List<CalendarCell> BuildCells();
    }
}