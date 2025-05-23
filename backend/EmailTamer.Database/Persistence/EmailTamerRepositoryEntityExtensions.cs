using System.Linq.Expressions;
using EmailTamer.Core.Extensions;
using EmailTamer.Database.Entities.Base;
using FastExpressionCompiler;

namespace EmailTamer.Database.Persistence;

public static class EmailTamerRepositoryEntityExtensions
{
    public static TLeaf AddOrUpdateEntityLeaf<TRoot, TLeaf>(this IEmailTamerRepository repository,
                                                            TRoot root,
                                                            TLeaf leafPrototype,
                                                            Expression<Func<TRoot, TLeaf?>> getterExpr)
        where TRoot : class, IEntity
        where TLeaf : class, IEntity
    {
        var getter = getterExpr.CompileFast();
        var rootLeaf = getter(root);
        if (rootLeaf is not null)
        {
            repository.Update(rootLeaf);
            return rootLeaf;
        }

        var setter = getterExpr.CreateSetter();
        setter(root, leafPrototype);
        repository.Add(leafPrototype);
        return leafPrototype;
    }
}