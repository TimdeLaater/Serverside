using Domain.Models;
using GraphQL.Types;

namespace SpelavondenWebService.GraphQL.Types
{
    public class ReviewType : ObjectGraphType<Review>
    {
        public ReviewType()
        {
            // ReviewId
            Field(x => x.ReviewId).Description("The ID of the review.");

            // Rating
            Field(x => x.Rating).Description("The rating given in the review.");

            // ReviewText
            Field(x => x.ReviewText).Description("The text of the review.");

            // ReviewerId (foreign key)
            Field(x => x.ReviewerId).Description("The ID of the reviewer.");

            // Reviewer (volledige persoon ophalen)
            Field<PersonType>("reviewer").Resolve(context => context.Source.Reviewer).Description("The person who wrote the review.");

            // BoardGameNightId (foreign key)
            Field(x => x.BoardGameNightId).Description("The ID of the board game night.");

            // BoardGameNight (volledige BoardGameNight ophalen)
            Field<BoardGameNightType>("boardGameNight").Resolve(context => context.Source.BoardGameNight).Description("The board game night that this review is related to.");
        }
    }
}

