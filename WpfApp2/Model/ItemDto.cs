using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2.Model
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public int LengthMm { get; set; }
        public string LengthInch
        {
            get
            {
                return ChangeToInch(LengthMm);
            }
            private set { }

        }
        public string Category { get; set; } = string.Empty;


        private string ChangeToInch(int lengthMm)
        {
            var inchNumber = (lengthMm / (decimal)25.4);
            var inchByEight = Math.Round(inchNumber * 8, MidpointRounding.AwayFromZero);
            var wholeInch = Math.Truncate(inchByEight / 8);
            decimal remainderInch = inchByEight % 8;

            if (remainderInch == 0) return $"{wholeInch}\"";
            if (remainderInch == 4) return $"{wholeInch} 1/2\"";
            if (remainderInch == 2) return $"{wholeInch} 1/4\"";
            if (remainderInch == 6) return $"{wholeInch} 3/4\"";


            
            return $"{wholeInch} {remainderInch}/8\"";
        }

    }



}
