using Domain.Models;
using GraphQL.Types;

namespace SpelavondenWebService.GraphQL.Types
{
    public class PersonType : ObjectGraphType<Person>
    {
        public PersonType()
        {
            // PersonId
            Field(x => x.PersonId).Description("The ID of the person.");

            // Name
            Field(x => x.Name).Description("The name of the person.");

            // Email
            Field(x => x.Email).Description("The email of the person.");

            // Gender (enum)
            Field<GenderEnumType>("gender").Resolve(context => context.Source.Gender).Description("The gender of the person.");

            // BirthDate
            Field(x => x.BirthDate).Description("The birth date of the person.");

            // Address (nested object)
            Field<AddressType>("address").Resolve(context => context.Source.Address).Description("The address of the person.");

            // Participations (list of BoardGameNights)
            Field<ListGraphType<BoardGameNightType>>("participations").Resolve(context => context.Source.Participations).Description("The board game nights the person has participated in.");

            // Reviews (list of Reviews written by the person)
            Field<ListGraphType<ReviewType>>("reviews").Resolve(context => context.Source.Reviews).Description("The reviews written by the person.");

            // Dietary Preferences (enum list)
            Field<ListGraphType<DietaryPreferenceEnumType>>("dietaryPreferences").Resolve(context => context.Source.DietaryPreferences).Description("The dietary preferences of the person.");
        }
    }
}
