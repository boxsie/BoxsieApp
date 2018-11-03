using AutoMapper;

namespace BoxsieApp.Core
{
    public static class DataUtils
    {
        public static IMappingExpression<TSource, TDest> IgnoreAllUnmapped<TSource, TDest>(this IMappingExpression<TSource, TDest> expression)
        {
            expression.ForAllMembers(opt => opt.Ignore());
            
            return expression;
        }
    }
}
