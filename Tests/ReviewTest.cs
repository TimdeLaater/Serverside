using Domain.Models;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class ReviewTest
    {
        private readonly ReviewValidationService _reviewValidationService;
        public ReviewTest()
        {
            _reviewValidationService = new ReviewValidationService();
        }
        [Fact]
        public void ValidateReview_PersonNotParticipating_ReturnsError()
        {
            // Arrange
            var person = new Person { PersonId = 1 };
            var boardGameNight = new BoardGameNight
            {
                DateTime = DateTime.Now.AddDays(-1), // Past date
                Participants = new List<Person> { new Person { PersonId = 2 } } // Person not in the participants list
            };

            // Act
            var result = _reviewValidationService.ValidateReview(boardGameNight, person);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Person is not participating in this board game night.", result.Errors);
        }

        [Fact]
        public void ValidateReview_BoardGameNightNotOccurred_ReturnsError()
        {
            // Arrange
            var person = new Person { PersonId = 1 };
            var boardGameNight = new BoardGameNight
            {
                DateTime = DateTime.Now.AddDays(1), // Future date
                Participants = new List<Person> { person } // Person is a participant
            };

            // Act
            var result = _reviewValidationService.ValidateReview(boardGameNight, person);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Board game night has not yet happened.", result.Errors);
        }

        [Fact]
        public void ValidateReview_PersonAlreadyReviewed_ReturnsError()
        {
            // Arrange
            var person = new Person { PersonId = 1 };
            var boardGameNight = new BoardGameNight
            {
                DateTime = DateTime.Now.AddDays(-1), // Past date
                Participants = new List<Person> { person },
                Reviews = new List<Review> { new Review { ReviewerId = person.PersonId } } // Person already reviewed
            };

            // Act
            var result = _reviewValidationService.ValidateReview(boardGameNight, person);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Person has already reviewed this board game night.", result.Errors);
        }

        [Fact]
        public void ValidateReview_ValidReview_ReturnsValid()
        {
            // Arrange
            var person = new Person { PersonId = 1 };
            var boardGameNight = new BoardGameNight
            {
                DateTime = DateTime.Now.AddDays(-1), // Past date
                Participants = new List<Person> { person },
                Reviews = new List<Review>() // No previous reviews by this person
            };

            // Act
            var result = _reviewValidationService.ValidateReview(boardGameNight, person);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}