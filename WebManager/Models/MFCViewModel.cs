using Imms.Mes.Data.Domain;

namespace Imms.WebManager.Models
{
    public class LineProductSummaryDateSpanViewModel : LineProductSummaryDateSpan
    {
        public string SpanName { get; set; }
        public int QtyTotal { get; set; }
        public int UPH { get; set; }
        public float OTD { get; set; }
        public float Pass { get; set; }
        public float Fail { get; set; }
    }
}