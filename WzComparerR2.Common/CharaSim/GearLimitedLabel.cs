using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WzComparerR2.CharaSim
{
    public class GearLimitedLabel
    {
        public GearLimitedLabel()
        {
            this.TooltipName = "限定标签";
            this.TooltipNameColor = unchecked((int)0xFFF8C481);
            this.IconLabelNum = 15;
        }

        public string TooltipName;
        public int TooltipNameColor;
        public int IconLabelNum;
        public string GradeTooltip;
    }
}
