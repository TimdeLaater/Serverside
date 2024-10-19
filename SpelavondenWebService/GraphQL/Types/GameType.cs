using Domain.Models;
using GraphQL.Types;

namespace SpelavondenWebService.GraphQL.Types
{
    public class BoardGameType : ObjectGraphType<BoardGame>
    {
        public BoardGameType()
        {
            // BoardGameId
            Field(x => x.BoardGameId).Description("The ID of the board game.");

            // Name
            Field(x => x.Name).Description("The name of the board game.");

            // Description
            Field(x => x.Description).Description("The description of the board game.");

            // Genre (enum)
            Field<GameGenreEnumType>("genre").Resolve(context => context.Source.Genre).Description("The genre of the board game.");

            // Is18Plus
            Field(x => x.Is18Plus).Description("Indicates if the board game is restricted to people aged 18 and above.");

            // GameType (enum)
            Field<GameTypeEnumType>("gameType").Resolve(context => context.Source.GameType).Description("The type of the game (e.g., CardGame, BoardGame, DiceGame).");

            // Image (optional field)
            Field<StringGraphType>("image")
                .Resolve(context => context.Source.Image != null ? Convert.ToBase64String(context.Source.Image) : null).Description("The image of the board game as a base64 string.");

            // BoardGameNights (list of BoardGameNights)
            Field<ListGraphType<BoardGameNightType>>("boardGameNights")
                .Resolve(context => context.Source.BoardGameNights)
                .Description("The game nights where this board game is played.");
        }
    }
    public class GameTypeEnumType : EnumerationGraphType<GameType>
    {
        public GameTypeEnumType()
        {
            Name = "GameType";
            Description = "The type of the game (e.g., CardGame, BoardGame, DiceGame).";
        }
    }
    public class GameGenreEnumType : EnumerationGraphType<GameGenre>
    {
        public GameGenreEnumType()
        {
            Name = "GameGenre";
            Description = "The genre of the board game.";
        }
    }
}
