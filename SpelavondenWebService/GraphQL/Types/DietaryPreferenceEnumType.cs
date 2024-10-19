using Domain.Models;
using GraphQL.Types;

namespace SpelavondenWebService.GraphQL.Types
{
    public class DietaryPreferenceEnumType : EnumerationGraphType<DietaryPreference>
    {
        public DietaryPreferenceEnumType()
        {
            Name = "DietaryPreference";
            Description = "The dietary preferences a person can have.";
            // The values of the enum are the same as the enum values in the Domain project.
        }
    }
}
