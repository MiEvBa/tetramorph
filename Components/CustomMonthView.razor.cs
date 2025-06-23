using System.Runtime.CompilerServices;
using Heron.MudCalendar;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace Tetramorph.Doctor.Components;

public partial class CustomMonthView : CalendarViewBaseEx
{
    private MudDropContainer<CalendarItem>? _dropContainer;

    protected virtual int Columns => 7;
    
    
    
    
    
    protected virtual string Classname
    {
        get
        {
            CssBuilder cssBuilder = CssBuilder.Default("mud-cal-month-table-body");
            cssBuilder = cssBuilder.AddClass("mud-cal-month-fixed-height", this.Calendar.MonthCellMinHeight == 0);
            return cssBuilder.Build();
        }
    }
    
    protected virtual string DayStyle(CalendarCell calendarCell, int index)
    {
        StyleBuilder styleBuilder = StyleBuilder.Empty();
        styleBuilder = styleBuilder.AddStyle("border-right", "none", (index + 1) % this.Columns == 0 && (!calendarCell.Today || !this.Calendar.HighlightToday));
        styleBuilder = styleBuilder.AddStyle("border", "1px solid var(--mud-palette-" + this.Calendar.Color.ToDescriptionString() + ")", calendarCell.Today && this.Calendar.HighlightToday);
        styleBuilder = styleBuilder.AddStyle("min-height", this.Calendar.MonthCellMinHeight.ToString() + "px", this.Calendar.MonthCellMinHeight > 0);
        return styleBuilder.Build();
    }

    protected virtual string CellClassname
    {
        get
        {
            CssBuilder cssBuilder = CssBuilder.Empty();
            cssBuilder = cssBuilder.AddClass("mud-cal-month-cell");
            cssBuilder = cssBuilder.AddClass("mud-cal-month-link", this.Calendar.CellClicked.HasDelegate);
            return cssBuilder.Build();
        }
    }

    protected virtual string DayClassname(CalendarCell calendarCell)
    {
        CssBuilder cssBuilder =  CssBuilder.Empty();
        cssBuilder = cssBuilder.AddClass("mud-cal-month-cell-title");
        cssBuilder = cssBuilder.AddClass("mud-cal-month-outside", calendarCell.Outside);
        return cssBuilder.Build();
    }
    
    protected virtual string CellStyle
    {
        get
        {
            StyleBuilder styleBuilder = StyleBuilder.Empty();
            styleBuilder = styleBuilder.AddStyle("overflow-y", "scroll", this.Calendar.MonthCellMinHeight == 0);
            return styleBuilder.Build();
        }
    }
    protected virtual string GridStyle
    {
        get
        {
            StyleBuilder styleBuilder = StyleBuilder.Empty();
            ref StyleBuilder local1 = ref styleBuilder;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
            interpolatedStringHandler.AppendLiteral("repeat(");
            interpolatedStringHandler.AppendFormatted<int>(this.Columns);
            interpolatedStringHandler.AppendLiteral(", minmax(10px, 1fr))");
            string stringAndClear1 = interpolatedStringHandler.ToStringAndClear();
            styleBuilder = local1.AddStyle("grid-template-columns", stringAndClear1);
            ref StyleBuilder local2 = ref styleBuilder;
            interpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 2);
            interpolatedStringHandler.AppendLiteral("repeat(");
            interpolatedStringHandler.AppendFormatted<int>(this.Cells.Count / this.Columns);
            interpolatedStringHandler.AppendLiteral(", ");
            interpolatedStringHandler.AppendFormatted<int>(100 / (this.Cells.Count / this.Columns));
            interpolatedStringHandler.AppendLiteral("%)");
            string stringAndClear2 = interpolatedStringHandler.ToStringAndClear();
            int num = this.Calendar.MonthCellMinHeight == 0 ? 1 : 0;
            styleBuilder = local2.AddStyle("grid-template-rows", stringAndClear2, num != 0);
            return styleBuilder.Build();
        }
    }
    

    /// <summary>
    /// Method invoked when the user clicks on the hyperlink in the cell.
    /// </summary>
    /// <param name="cell">The cell that was clicked.</param>
    /// <returns></returns>
    protected virtual async Task OnCellLinkClicked(CalendarCell cell)
    {
        if (Calendar.CellClicked.HasDelegate)
        {
            await Calendar.CellClicked.InvokeAsync(cell.Date);
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        // Ensure that the order of items is correct
        _dropContainer?.Refresh();
    }
    
    /// <summary>
    /// Method invoked when the user clicks on the calendar item.
    /// </summary>
    /// <param name="item">The calendar item that was clicked.</param>
    /// <returns></returns>
    protected virtual Task OnItemClicked(CalendarItem item)
    {
        return Calendar.ItemClicked.InvokeAsync(item);
    }
    
    protected override List<CalendarCell> BuildCells()
    {
        var cells = new List<CalendarCell>();
        var monthStart = new DateTime(Calendar.CurrentDay.Year, Calendar.CurrentDay.Month, 1);
        var monthEnd = new DateTime(Calendar.CurrentDay.AddMonths(1).Year, Calendar.CurrentDay.AddMonths(1).Month, 1).AddDays(-1);

        var range = new CalendarDateRange(Calendar.CurrentDay.Date, CalendarView.Month);
        if (range.Start == null || range.End == null) return cells;
        
        var date = range.Start.Value;
        var lastDate = range.End.Value;
        while (date <= lastDate)
        {
            var cell = BuildCell(date, monthStart, monthEnd);
            cells.Add(cell);
            
            // Next day
            date = date.AddDays(1);
        }
        
        return cells;
    }
    
    /// <summary>
    /// Builds a cell for the month view.
    /// </summary>
    /// <param name="date">The date of the cell.</param>
    /// <param name="monthStart">The first day of the month being shown.</param>
    /// <param name="monthEnd">The last day of the month being shown.</param>
    /// <returns></returns>
    protected virtual CalendarCell BuildCell(DateTime date, DateTime monthStart, DateTime monthEnd)
    {
        var cell = new CalendarCell { Date = date };
        if (date.Date == DateTime.Today) cell.Today = true;
        if (date < monthStart || date > monthEnd)
        {
            cell.Outside = true;
        }
        cell.Items = Calendar.Items.Where(i =>
                (i.Start.Date < date && i.End.HasValue && i.End.Value.Date >= date))
            .OrderBy(i => i.Start)
            .ToList();

        return cell;
    }

    private async Task ItemDropped(MudItemDropInfo<CalendarItem> dropItem)
    {
        if (dropItem.Item == null) return;
        var item = dropItem.Item;
        
        // Update start and end time
        var duration = item.End?.Subtract(item.Start) ?? TimeSpan.Zero;
        item.Start = DateTime.Parse(dropItem.DropzoneIdentifier).Add(item.Start.TimeOfDay);
        if (item.End.HasValue)
        {
            item.End = item.Start.Add(duration);
        }
        
        Calendar.Refresh();

        await Calendar.ItemChanged.InvokeAsync(item);
    }

    private RenderFragment<CalendarItem> CellTemplate => Calendar.MonthTemplate ?? Calendar.CellTemplate;
}