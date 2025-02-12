﻿using Xunit;
using Domain.Models;
using Domain.Services;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using Moq;
using Application.Services;

namespace Tests
{
    public class BoardGameNightTests
    {
        private readonly Mock<IBoardGameNightRepository> _mockRepo;
        private readonly BoardGameNightValidationService _validator;
        private Person defaultOrganiser;
        private Person defaultParticipant;
        private BoardGameNight defaultBoardGameNight;

        public BoardGameNightTests()
        {
            _mockRepo = new Mock<IBoardGameNightRepository>();
            _validator = new BoardGameNightValidationService(_mockRepo.Object);
            defaultOrganiser = CreateDefaultOrganizer(DateTime.Now.AddYears(-20));
            defaultParticipant = CreateDefaultParticipant(DateTime.Now.AddYears(-20));
            defaultBoardGameNight = CreateDefaultBoardGameNight(defaultOrganiser, DateTime.Now.AddDays(1), 5);

        }

        // Helper Method to Create Address (as it's required)
        private Address CreateDefaultAddress()
        {
            return new Address
            {
                Street = "123 Main St",
                City = "Sample City",
                HouseNumber = "12B"
            };
        }

        // Helper Method to Create Organizer
        private Person CreateDefaultOrganizer(DateTime birthDate)
        {
            return new Person
            {
                PersonId = 1,
                BirthDate = birthDate,
                Name = "Default Organizer",
                Address = CreateDefaultAddress(),
                Gender = Gender.M,
                Email = "organizer@example.com"
            };
        }

        // Helper Method to Create BoardGameNight with Defaults
        private BoardGameNight CreateDefaultBoardGameNight(Person organizer, DateTime dateTime, int maxPlayers)
        {
            return new BoardGameNight
            {
                Organizer = organizer,
                OrganizerId = organizer.PersonId,
                DateTime = dateTime,
                MaxPlayers = maxPlayers,
                Address = CreateDefaultAddress()
            };
        }

        private Person CreateDefaultParticipant(DateTime birthDate)
        {
            return new Person
            {
                Email = "Mail@mail.com",
                PersonId = 2,
                BirthDate = birthDate,
                Name = "Default Participant",
                Address = CreateDefaultAddress()
            };
        }


        [Fact]
        public void ValidateBoardGameNight_OrganizerUnder18_ReturnsError()
        {

            var underageOrganizer = CreateDefaultOrganizer(DateTime.Now.AddYears(-17));
            var boardGameNight = CreateDefaultBoardGameNight(underageOrganizer, DateTime.Now.AddDays(1), 5);


            // Act
            var result = _validator.ValidateBoardGameNight(boardGameNight);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Organizer must be at least 18 years old.", result.Errors);
        }

        [Fact]
        public void ValidateBoardGameNight_PastDate_ReturnsError()
        {
            // Arrange
            var organizer = CreateDefaultOrganizer(DateTime.Now.AddYears(-20));
            var boardGameNight = CreateDefaultBoardGameNight(organizer, DateTime.Now.AddDays(-1), 5);   

            // Act
            var result = _validator.ValidateBoardGameNight(boardGameNight);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Board game night date cannot be in the past.", result.Errors);
        }

        [Fact]
        public void ValidateBoardGameNight_MaxPlayersZero_ReturnsError()
        {
            // Arrange
            var organizer = CreateDefaultOrganizer(DateTime.Now.AddYears(-20));
            var boardGameNight = CreateDefaultBoardGameNight(organizer, DateTime.Now.AddDays(1), 0); // Max players 0

            // Act
            var result = _validator.ValidateBoardGameNight(boardGameNight);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Max players must be greater than 0.", result.Errors);
        }

        [Fact]
        public void ValidateParticipant_ParticipantUnder16_ReturnsError()
        {
            // Arrange
            var participant = CreateDefaultParticipant(DateTime.Now.AddYears(-15));
            


            // Act
            var result = _validator.ValidateParticipant(participant, defaultBoardGameNight);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Person must be at least 16 years old to participate.", result.Errors);
        }

        [Fact]
        public void ValidateParticipant_ParticipantIsOrganizer_ReturnsError()
        {
            // Arrange
            var organizer = CreateDefaultOrganizer(DateTime.Now.AddYears(-20));
            var boardGameNight = CreateDefaultBoardGameNight(organizer, DateTime.Now.AddDays(1), 5);

            // Act
            var result = _validator.ValidateParticipant(organizer, boardGameNight); // Organizer as participant

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Organizer cannot participate in their own board game night.", result.Errors);
        }

        [Fact]
        public void ValidateParticipant_AlreadyParticipating_ReturnsError()
        {
            // Arrange
            var participant = new Person
            {
                PersonId = 1,
                Email = "Mail@mail.com",
                BirthDate = DateTime.Now.AddYears(-20),
                Name = "Participant",
                Address = CreateDefaultAddress()
            };
          
            var boardGameNight = new BoardGameNight
            {
                Organizer = defaultOrganiser,
                Participants = new List<Person> { participant },
                Is18Plus = false,
                MaxPlayers = 5,
                Address = CreateDefaultAddress()
            };

            // Act
            var result = _validator.ValidateParticipant(participant, boardGameNight);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Person is already participating in this board game night.", result.Errors);
        }

        [Fact]
        public void ValidateParticipant_Under18For18PlusGame_ReturnsError()
        {
            // Arrange
            var participant = CreateDefaultParticipant(DateTime.Now.AddYears(-17));

            var boardGameNight = new BoardGameNight
            {
                Organizer = defaultOrganiser,
                Participants = new List<Person>(),
                Is18Plus = true, // 18+ game
                MaxPlayers = 5,
                Address = CreateDefaultAddress()
            };

            // Act
            var result = _validator.ValidateParticipant(participant, boardGameNight);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Person must be at least 18 years old to participate in this board game night.", result.Errors);
        }

        [Fact]
        public async Task ValidateSignUpAsync_AlreadySignedUpForSameDay_ReturnsError()
        {
            // Arrange
            var personId = 1;
            var gameNightDate = DateTime.Now.AddDays(1).Date;
            var gameNight = new BoardGameNight { DateTime = gameNightDate, Address = CreateDefaultAddress(), Organizer = defaultOrganiser };
            var existingGameNight = new BoardGameNight { DateTime = gameNightDate, Address= CreateDefaultAddress(), Organizer = defaultOrganiser };

            // Mock the repository to return a game night for the same date
            _mockRepo.Setup(repo => repo.GetByPersonAndDateAsync(personId, gameNightDate))
                     .ReturnsAsync(existingGameNight);

            // Act
            var result = await _validator.ValidateSignUpAsync(personId, gameNight);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("You are already signed up for a game night on this day.", result.Errors);
        }
         
        [Fact]
        public async Task ValidateSignUpAsync_NotSignedUpForSameDay_ReturnsValid()
        {
            // Arrange
            var personId = 1;
            var gameNightDate = DateTime.Now.AddDays(1).Date;
            var gameNight = CreateDefaultBoardGameNight(defaultOrganiser, gameNightDate, 5);

            // Mock the repository to return null (no existing game night)
            _mockRepo.Setup(repo => repo.GetByPersonAndDateAsync(personId, gameNightDate))
                     .ReturnsAsync((BoardGameNight)null);

            // Act
            var result = await _validator.ValidateSignUpAsync(personId, gameNight);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void CheckDietaryWarnings_NoMatchingFoodOptions_ReturnsWarnings()
        {
            // Arrange
            var person = CreateDefaultParticipant(DateTime.Now.AddYears(-20));
            person.DietaryPreferences = new List<DietaryPreference> { DietaryPreference.Vegetarian };
            
            var gameNight = CreateDefaultBoardGameNight(defaultOrganiser, DateTime.Now.AddDays(1), 5);
            gameNight.FoodOptions = new List<DietaryPreference> { DietaryPreference.LactoseFree };


            // Act
            var warnings = _validator.CheckDietaryWarnings(person, gameNight);

            // Assert
            Assert.NotEmpty(warnings);
            Assert.Contains("Warning: No food options match your dietary preference: Vegetarian.", warnings);
        }

        [Fact]
        public void CheckDietaryWarnings_MatchingFoodOptions_ReturnsNoWarnings()
        {
            // Arrange
            var person = CreateDefaultParticipant(DateTime.Now.AddYears(-20));
            person.DietaryPreferences = new List<DietaryPreference> { DietaryPreference.Vegetarian };


            var gameNight = CreateDefaultBoardGameNight(defaultOrganiser, DateTime.Now.AddDays(1), 5);
            gameNight.FoodOptions = new List<DietaryPreference> { DietaryPreference.Vegetarian };


            // Act
            var warnings = _validator.CheckDietaryWarnings(person, gameNight);

            // Assert
            Assert.Empty(warnings);
        }

        //Edit test
        // Test for CanEdit method
        [Fact]
        public void CanEdit_BoardGameNightInFutureWithNoParticipants_ReturnsTrue()
        {
            // Arrange
            var futureDate = DateTime.Now.AddDays(5);
            var boardGameNight = CreateDefaultBoardGameNight(defaultOrganiser, futureDate, 5);

            // Act
            var result = _validator.CanEdit(boardGameNight);

            // Assert
            Assert.True(result, "Board game night should be editable when it's in the future and has no participants.");
        }

        [Fact]
        public void CanEdit_BoardGameNightInFutureWithParticipants_ReturnsFalse()
        {
            // Arrange
            var futureDate = DateTime.Now.AddDays(5);
            var boardGameNight = CreateDefaultBoardGameNight(defaultOrganiser, futureDate, 5);
            boardGameNight.Participants.Add(defaultParticipant);

            // Act
            var result = _validator.CanEdit(boardGameNight);

            // Assert
            Assert.False(result, "Board game night should not be editable when it has participants.");
        }

        [Fact]
        public void CanEdit_BoardGameNightInPast_ReturnsFalse()
        {
            // Arrange
            var pastDate = DateTime.Now.AddDays(-1);
            var boardGameNight = CreateDefaultBoardGameNight(defaultOrganiser, pastDate, 5);

            // Act
            var result = _validator.CanEdit(boardGameNight);

            // Assert
            Assert.False(result, "Board game night should not be editable when it's in the past.");
        }

        [Fact]
        public void CanEdit_BoardGameNightInPastWithParticipants_ReturnsFalse()
        {
            // Arrange
            var pastDate = DateTime.Now.AddDays(-1);
            var boardGameNight = CreateDefaultBoardGameNight(defaultOrganiser, pastDate, 5);
            boardGameNight.Participants.Add(defaultParticipant);


            // Act
            var result = _validator.CanEdit(boardGameNight);

            // Assert
            Assert.False(result, "Board game night should not be editable when it's in the past and has participants.");
        }
    }
}
