using GraphQL.Types;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using SpelavondenWebService.GraphQL.Queries;
using SpelavondenWebService.GraphQL.Types;

namespace SpelavondenWebService.GraphQL
{
    public class AppSchema : Schema
    {
        public AppSchema(IServiceProvider provider) : base(provider)
        {
            Query = provider.GetRequiredService<RootQuery>();
        }
    }

    public class RootQuery : ObjectGraphType
    {
        public RootQuery(BoardGameNightQuery boardGameNightQuery, BoardGameQuery boardGameQuery)
        {
        
            AddField(boardGameNightQuery.GetField("boardGameNights"));
            AddField(boardGameNightQuery.GetField("boardGameNight"));

            AddField(boardGameQuery.GetField("boardGames"));
            AddField(boardGameQuery.GetField("boardGame"));
        }
    }
}
