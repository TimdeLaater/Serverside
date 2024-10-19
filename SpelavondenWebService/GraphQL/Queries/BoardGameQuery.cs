using Application.Interfaces;
using Domain.Models;
using GraphQL;
using GraphQL.Types;
using SpelavondenWebService.GraphQL.Types;

namespace SpelavondenWebService.GraphQL.Queries
{
    public class BoardGameQuery : ObjectGraphType
    {
        public BoardGameQuery(IBoardGameRepository boardGameRepository)
        {
            // Query om alle BoardGames op te halen
            Field<ListGraphType<BoardGameType>>("boardGames")
                .ResolveAsync(async context => await boardGameRepository.GetAllAsync()).Description("Retrieve all board games.");

            // Query om een specifiek BoardGame op te halen op basis van ID
            Field<BoardGameType>("boardGame")
                .Arguments(new QueryArguments(new QueryArgument<IdGraphType> { Name = "id" }))
                .ResolveAsync(async context =>
                {
                    var id = context.GetArgument<int>("id");
                    return await boardGameRepository.GetByIdAsync(id);
                }).Description("Retrieve a board game by its ID.");
        }
    }
}