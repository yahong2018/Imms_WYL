using System;

namespace Imms.Mes.Services.Kanban.Line
{
    public class KanbanLineData
    {
        public string line_code { get; set; }
        public int plan_qty { get; set; }
        public bool is_break { get; set; }

        public DateTime current_time { get; set; }

        public DateTime time_start_plan { get; set; }
        public DateTime time_end_plan { get; set; }

        public Summary line_summary_data { get; set; }
        public Detail[] line_detail_data { get; set; }
    }

    public class Summary
    {
        public string production_code { get; set; }
        public string production_name { get; set; }
        public string production_order_no { get; set; }
        public int uph { get; set; }
        public int person_qty { get; set; }
    }

    public class Detail
    {
        public string time_begin { get; set; }
        public string time_end { get; set; }
        public int qty_plan { get; set; }
        public int qty_good { get; set; }
        public int qty_bad { get; set; }
        public bool is_current_item { get; set; }
        public bool is_break_item { get; set; }
        public bool shown_in_detail_table { get; set; }
        public int delay_time { get; set; }
        public int seq { get; set; }
        public long span_id { get; set; }
    }
}