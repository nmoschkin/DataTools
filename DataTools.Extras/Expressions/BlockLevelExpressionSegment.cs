using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DataTools.Extras.Expressions
{
    public class BlockLevelExpressionSegment : ExpressionSegment
    {

        protected override void Initialize(string value, ExpressionSegment parent, CultureInfo ci, StorageMode mode, string varSym)
        {


            base.Initialize(value, parent, ci, mode, varSym);
        }

    }
}
