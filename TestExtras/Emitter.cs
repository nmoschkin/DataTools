using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DataTools.Extras.Expressions
{
    public class Emitter
    {

        private ExpressionSegment segment;


        public Emitter(ExpressionSegment segment)
        {
            this.segment = segment;
        }

        public ExpressionSegment Segment
        {
            get { return segment; }
        }

        public int GetVariableCount()
        {
            return GetVariableCount(Segment);
        }

        private int GetVariableCount(ExpressionSegment seg)
        {
            int c = 0;

            if ((seg.PartType & PartType.Variable) == PartType.Variable)
            {
                c = 1;
            }

            foreach (var item in seg.Components)
            {
                c += GetVariableCount(item);
            }

            return c;
        }

        //public object EmitExpression()
        //{

        //}


    }
}
