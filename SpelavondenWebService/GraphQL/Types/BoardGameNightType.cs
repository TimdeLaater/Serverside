using Domain.Models;
using GraphQL.Types;

namespace SpelavondenWebService.GraphQL.Types
{
    public class BoardGameNightType : ObjectGraphType<BoardGameNight>
    {
        public BoardGameNightType()
        {
            Field(x => x.BoardGameNightId).Description("The ID of the board game night.");
            Field(x => x.MaxPlayers).Description("The maximum number of players.");
            Field(x => x.Date).Description("The date of the board game night.");
            Field(x => x.Is18Plus).Description("Is the board game night restricted to 18+?");
            Field(x => x.OrganizerId).Description("The ID of the organizer.");
            Field<PersonType>("organizer").Resolve(context => context.Source.Organizer);
            Field<ListGraphType<PersonType>>("participants").Resolve(context => context.Source.Participants);
            Field<ListGraphType<BoardGameType>>("boardGames").Resolve(context => context.Source.BoardGames);
            Field<ListGraphType<DietaryPreferenceEnumType>>("foodOptions").Resolve(context => context.Source.FoodOptions);
            Field<AddressType>("address").Resolve(context => context.Source.Address);
        }
    }
}