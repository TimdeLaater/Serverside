using Application.Interfaces;
using Domain.Models;
using GraphQL;
using GraphQL.Types;
using SpelavondenWebService.GraphQL.Types;

namespace SpelavondenWebService.GraphQL.Queries
{
    public class BoardGameNightQuery : ObjectGraphType
    {
        public BoardGameNightQuery(IBoardGameNightRepository boardGameNightRepository)
        {
            // Query om alle BoardGameNights op te halen
            Field<ListGraphType<BoardGameNightType>>("boardGameNights")
                .ResolveAsync(async context => await boardGameNightRepository.GetAllAsync()).Description("Retrieve all board game nights.");

            // Query om een specifieke BoardGameNight op te halen op basis van ID
            Field<BoardGameNightType>("boardGameNight")
                .Arguments(new QueryArguments(new QueryArgument<IdGraphType> { Name = "id" }))
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    return await boardGameNightRepository.GetByIdAsync(id);
                }).Description("Retrieve a board game night by its ID.");
        }
    }
}
